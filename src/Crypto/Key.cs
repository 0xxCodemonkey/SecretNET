#nullable enable
using SecretNET.Crypto.Secp256k1;

namespace SecretNET.Crypto;

public class Key : IDisposable
{
    private const int KEY_SIZE = 32;
    private readonly static uint256 N = uint256.Parse("fffffffffffffffffffffffffffffffebaaedce6af48a03bbfd25e8cd0364141");

    internal readonly ECPrivKey _ECKey;

    public bool IsCompressed
    {
        get;
        internal set;
    }

    public Key()
        : this(true)
    {

    }

    internal Key(ECPrivKey ecKey, bool compressed)
    {
        if (ecKey == null)
            throw new ArgumentNullException(nameof(ecKey));
        IsCompressed = compressed;
        _ECKey = ecKey;
    }

    public Key(ReadOnlySpan<byte> bytes, bool compressed = true)
    {
        IsCompressed = compressed;
        if (bytes.Length != KEY_SIZE)
        {
            throw new ArgumentException(paramName: "data", message: $"The size of an EC key should be {KEY_SIZE}");
        }
        if (SecretContext.Instance.TryCreateECPrivKey(bytes, out var key) && key is ECPrivKey)
        {
            _ECKey = key;
        }
        else
            throw new ArgumentException(paramName: "data", message: "Invalid EC key");
    }


    public Key(bool fCompressedIn)
    {
        IsCompressed = fCompressedIn;

        Span<byte> data = stackalloc byte[KEY_SIZE];
        while (true)
        {
            RandomUtils.GetBytes(data);
            if (SecretContext.Instance.TryCreateECPrivKey(data, out var key) && key is ECPrivKey)
            {
                _ECKey = key;
                return;
            }
        }

    }
    public Key(byte[] data, int count = -1, bool fCompressedIn = true)
    {
        if (count == -1)
            count = data.Length;
        if (count != KEY_SIZE)
        {
            throw new ArgumentException(paramName: "data", message: $"The size of an EC key should be {KEY_SIZE}");
        }

        if (SecretContext.Instance.TryCreateECPrivKey(data.AsSpan().Slice(0, KEY_SIZE), out var key) && key is ECPrivKey)
        {
            IsCompressed = fCompressedIn;
            _ECKey = key;
        }
        else
            throw new ArgumentException(paramName: "data", message: "Invalid EC key");
    }

    private static bool Check(byte[] vch)
    {
        var candidateKey = new uint256(vch.SafeSubarray(0, KEY_SIZE), false);
        return candidateKey > 0 && candidateKey < N;
    }

    PubKey? _PubKey;

    public PubKey PubKey
    {
        get
        {
            AssertNotDisposed();
            if (_PubKey is PubKey pubkey)
                return pubkey;

            pubkey = new PubKey(_ECKey.CreatePubKey(), IsCompressed);
            _PubKey = pubkey;
            return pubkey;
        }
    }

    internal ECDSASignature Sign(uint256 hash, bool useLowR)
    {
        AssertNotDisposed();
        return _ECKey.Sign(hash, useLowR);
    }

    internal ECDSASignature Sign(uint256 hash)
    {
        AssertNotDisposed();
        return _ECKey.Sign(hash, true);
    }

    public KeyPair CreateKeyPair()
    {
        return new KeyPair(this, PubKey);
    }

    internal CompactSignature SignCompact(uint256 hash)
    {
        return SignCompact(hash, true);
    }

    internal CompactSignature SignCompact(uint256 hash, bool forceLowR)
    {
        if (hash is null)
            throw new ArgumentNullException(nameof(hash));
        if (!IsCompressed)
            throw new InvalidOperationException("This operation is only supported on compressed pubkey");
        AssertNotDisposed();

        byte[] sigBytes = new byte[64];
        var sig = new SecpRecoverableECDSASignature(_ECKey.Sign(hash, forceLowR, out var rec), rec);
        sig.WriteToSpanCompact(sigBytes, out _);
        return new CompactSignature(rec, sigBytes);

    }

    public string Decrypt(string encryptedText)
    {
        if (encryptedText is null)
            throw new ArgumentNullException(nameof(encryptedText));
        AssertNotDisposed();
        var bytes = Encoders.Base64.DecodeData(encryptedText);
        var decrypted = Decrypt(bytes);
        return Encoding.UTF8.GetString(decrypted, 0, decrypted.Length);
    }

    public byte[] Decrypt(byte[] encrypted)
    {
        if (encrypted is null)
            throw new ArgumentNullException(nameof(encrypted));
        if (encrypted.Length < 85)
            throw new ArgumentException("Encrypted text is invalid, it should be length >= 85.");
        AssertNotDisposed();
        var magic = encrypted.SafeSubarray(0, 4);
        var ephemeralPubkeyBytes = encrypted.SafeSubarray(4, 33);
        var cipherText = encrypted.SafeSubarray(37, encrypted.Length - 32 - 37);
        var mac = encrypted.SafeSubarray(encrypted.Length - 32);
        if (!Utils.ArrayEqual(magic, Encoders.ASCII.DecodeData("BIE1")))
            throw new ArgumentException("Encrypted text is invalid, Invalid magic number.");

        var ephemeralPubkey = new PubKey(ephemeralPubkeyBytes);

        var sharedKey = Hashes.SHA512(ephemeralPubkey.GetSharedPubkey(this).ToBytes());
        var iv = sharedKey.SafeSubarray(0, 16);
        var encryptionKey = sharedKey.SafeSubarray(16, 16);
        var hashingKey = sharedKey.SafeSubarray(32);

        var hashMAC = Hashes.HMACSHA256(hashingKey, encrypted.SafeSubarray(0, encrypted.Length - 32));
        if (!Utils.ArrayEqual(mac, hashMAC))
            throw new ArgumentException("Encrypted text is invalid, Invalid mac.");

        var aes = new AesBuilder().SetKey(encryptionKey).SetIv(iv).IsUsedForEncryption(false).Build();
        var message = aes.Process(cipherText, 0, cipherText.Length);
        return message;
    }

    public Key Derivate(byte[] cc, uint nChild, out byte[] ccChild)
    {
        AssertNotDisposed();

        if (!IsCompressed)
            throw new InvalidOperationException("The key must be compressed");
        Span<byte> vout = stackalloc byte[64];
        vout.Clear();
        if (nChild >> 31 == 0)
        {
            Span<byte> pubkey = stackalloc byte[33];
            PubKey.ToBytes(pubkey, out _);
            Hashes.BIP32Hash(cc, nChild, pubkey[0], pubkey.Slice(1), vout);
        }
        else
        {
            Span<byte> privkey = stackalloc byte[32];
            _ECKey.WriteToSpan(privkey);
            Hashes.BIP32Hash(cc, nChild, 0, privkey, vout);
            privkey.Fill(0);
        }
        ccChild = new byte[32];
        vout.Slice(32, 32).CopyTo(ccChild);
        ECPrivKey keyChild = _ECKey.TweakAdd(vout.Slice(0, 32));
        vout.Clear();
        return new Key(keyChild, true);
    }

    public override bool Equals(object? obj)
    {
        if (obj is Key item)
            return PubKey.Equals(item.PubKey);
        return false;
    }
    public static bool operator ==(Key? a, Key? b)
    {
        if (a?.PubKey is PubKey apk && b?.PubKey is PubKey bpk)
        {
            return apk.Equals(bpk);
        }
        return a is null && b is null;
    }

    public static bool operator !=(Key? a, Key? b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        return PubKey.GetHashCode();
    }

    public byte[] ToBytes()
    {
        AssertNotDisposed();

        var b = new byte[KEY_SIZE];
        _ECKey.WriteToSpan(b);
        return b;
    }

    public string ToHex()
    {
        AssertNotDisposed();

        Span<byte> tmp = stackalloc byte[KEY_SIZE];
        _ECKey.WriteToSpan(tmp);
        return Encoders.Hex.EncodeData(tmp);
    }

    bool disposed = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    void AssertNotDisposed()
    {
        if (disposed)
            throw new ObjectDisposedException(nameof(Key));
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;

        if (disposing)
        {
            if (_ECKey is IDisposable keyMaterial)
                keyMaterial.Dispose();
        }
        disposed = true;
    }

    ~Key()
    {
        Dispose(false);
    }
}
#nullable disable
