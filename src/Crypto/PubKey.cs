#nullable enable
using SecretNET.Crypto.BIP32;
using System.Diagnostics.CodeAnalysis;

namespace SecretNET.Crypto;


public class PubKey : IComparable<PubKey>, IEquatable<PubKey>, IPubKey
{
    /// <summary>
    /// Create a new Public key from string
    /// </summary>
    public PubKey(string hex)
        : this(Encoders.Hex.DecodeData(hex))
    {

    }


    bool compressed;
    internal PubKey(ECPubKey pubkey, bool compressed)
    {
        if (pubkey == null)
            throw new ArgumentNullException(nameof(pubkey));
        _ECKey = pubkey;
        this.compressed = compressed;
    }
    internal static bool TryCreatePubKey(byte[] bytes, [MaybeNullWhen(false)] out PubKey pubKey)
    {
        if (bytes is null)
            throw new ArgumentNullException(nameof(bytes));
        return TryCreatePubKey(bytes.AsSpan(), out pubKey);
    }
    internal static bool TryCreatePubKey(ReadOnlySpan<byte> bytes, [MaybeNullWhen(false)] out PubKey pubKey)
    {
        if (SecretContext.Instance.TryCreatePubKey(bytes, out var compressed, out var p))
        {
            pubKey = new PubKey(p, compressed);
            return true;
        }
        pubKey = null;
        return false;
    }


    /// <summary>
    /// Create a new Public key from byte array
    /// </summary>
    /// <param name="bytes">byte array</param>
    public PubKey(byte[] bytes)
    {
        if (bytes is null)
            throw new ArgumentNullException(nameof(bytes));

        if (SecretContext.Instance.TryCreatePubKey(bytes, out compressed, out var p))
        {
            _ECKey = p;
        }
        else
        {
            throw new FormatException("Invalid public key");
        }
    }


    /// <summary>
    /// Create a new Public key from byte array
    /// </summary>
    /// <param name="bytes">byte array</param>
    public PubKey(ReadOnlySpan<byte> bytes)
    {
        if (SecretContext.Instance.TryCreatePubKey(bytes, out compressed, out var p) && p is ECPubKey)
        {
            _ECKey = p;
        }
        else
        {
            throw new FormatException("Invalid public key");
        }
    }

    ECPubKey _ECKey;
    internal ref readonly ECPubKey ECKey => ref _ECKey;


    public int CompareTo(PubKey? other) => other is null ? 1 : BytesComparer.Instance.Compare(ToBytes(), other.ToBytes());

    public PubKey Compress()
    {
        if (IsCompressed)
            return this;

        return new PubKey(_ECKey, true);
    }

    public PubKey Decompress()
    {
        if (!IsCompressed)
            return this;

        return new PubKey(_ECKey, false);
    }

    /// <summary>
    /// Quick sanity check on public key format. (size + first byte)
    /// </summary>
    /// <param name="data">bytes array</param>
    public static bool SanityCheck(byte[] data)
    {
        return SanityCheck(data, 0, data.Length);
    }

    public static bool SanityCheck(byte[] data, int offset, int count)
    {
        if (data is null)
            throw new ArgumentNullException(nameof(data));
        return

                    count == 33 && (data[offset + 0] == 0x02 || data[offset + 0] == 0x03) ||
                    count == 65 && (data[offset + 0] == 0x04 || data[offset + 0] == 0x06 || data[offset + 0] == 0x07)
                ;
    }

    KeyId? _ID;
    internal KeyId Hash
    {
        get
        {
            if (_ID is null)
            {
                Span<byte> tmp = stackalloc byte[65];
                _ECKey.WriteToSpan(compressed, tmp, out int len);
                tmp = tmp.Slice(0, len);
                _ID = new KeyId(Hashes.Hash160(tmp));
            }
            return _ID;
        }
    }

    public bool IsCompressed
    {
        get
        {
            return compressed;
        }
    }

    HDFingerprint? fp;
    internal HDFingerprint GetHDFingerPrint()
    {
        if (fp is HDFingerprint f)
            return f;
        f = new HDFingerprint(Hash.ToBytes(), 0);
        fp = f;
        return f;
    }

    internal bool Verify(uint256 hash, ECDSASignature sig)
    {
        if (sig == null)
            throw new ArgumentNullException(nameof(sig));
        if (hash == null)
            throw new ArgumentNullException(nameof(hash));

        Span<byte> msg = stackalloc byte[32];
        hash.ToBytes(msg);
        return _ECKey.SigVerify(sig.ToSecpECDSASignature(), msg);
    }

    public bool Verify(uint256 hash, byte[] sig)
    {
        return Verify(hash, ECDSASignature.FromDER(sig));
    }

    public string ToHex()
    {
        Span<byte> tmp = stackalloc byte[65];
        _ECKey.WriteToSpan(compressed, tmp, out var l);
        tmp = tmp.Slice(0, l);
        return Encoders.Hex.EncodeData(tmp);
    }

    public byte[] ToBytes()
    {
        return _ECKey.ToBytes(compressed);
    }


    public void ToBytes(Span<byte> output, out int length)
    {
        _ECKey.WriteToSpan(compressed, output, out length);
    }

    public byte[] ToBytes(bool @unsafe)
    {
        return ToBytes();
    }
    public override string ToString()
    {
        return ToHex();
    }

    public PubKey Derivate(byte[] cc, uint nChild, out byte[] ccChild)
    {
        if (!IsCompressed)
            throw new InvalidOperationException("The pubkey must be compressed");
        if (nChild >> 31 != 0)
            throw new InvalidOperationException("A public key can't derivate an hardened child");

        Span<byte> vout = stackalloc byte[64];
        vout.Clear();
        Span<byte> pubkey = stackalloc byte[33];
        ToBytes(pubkey, out _);
        Hashes.BIP32Hash(cc, nChild, pubkey[0], pubkey.Slice(1), vout);
        ccChild = new byte[32]; ;
        vout.Slice(32, 32).CopyTo(ccChild);
        return new PubKey(ECKey.AddTweak(vout.Slice(0, 32)), true);
    }

    public override bool Equals(object? obj)
    {
        if (obj is PubKey pk)
            return Equals(pk);
        return false;
    }

    public bool Equals(PubKey? pk) => this == pk;
    public static bool operator ==(PubKey? a, PubKey? b)
    {
        if (a is PubKey aa && b is PubKey bb)
        {
            return aa.ECKey == bb.ECKey && aa.compressed == bb.compressed;
        }
        return a is null && b is null;
    }

    public static bool operator !=(PubKey? a, PubKey? b)
    {
        return !(a == b);
    }

    int? hashcode;
    public override int GetHashCode()
    {
        if (hashcode is int h)
            return h;

        unchecked
        {
            h = _ECKey.GetHashCode();
            h = h * 23 + (compressed ? 0 : 1);
            hashcode = h;
            return h;
        }

    }

    #region IDestination Members

    /// <summary>
    /// Exchange shared secret through ECDH
    /// </summary>
    /// <param name="key">Private key</param>
    /// <returns>Shared pubkey</returns>
    public PubKey GetSharedPubkey(Key key)
    {

        if (key == null)
            throw new ArgumentNullException(nameof(key));
        return new PubKey(ECKey.GetSharedPubkey(key._ECKey), true);
    }

    public string Encrypt(string message)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));

        var bytes = Encoding.UTF8.GetBytes(message);
        return Encoders.Base64.EncodeData(Encrypt(bytes));
    }

    public byte[] Encrypt(byte[] message)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));
        var ephemeral = new Key();
        var sharedKey = Hashes.SHA512(GetSharedPubkey(ephemeral).ToBytes());
        var iv = sharedKey.SafeSubarray(0, 16);
        var encryptionKey = sharedKey.SafeSubarray(16, 16);
        var hashingKey = sharedKey.SafeSubarray(32);

        var aes = new AesBuilder().SetKey(encryptionKey).SetIv(iv).IsUsedForEncryption(true).Build();
        var cipherText = aes.Process(message, 0, message.Length);
        var ephemeralPubkeyBytes = ephemeral.PubKey.ToBytes();
        var encrypted = Encoders.ASCII.DecodeData("BIE1").Concat(ephemeralPubkeyBytes, cipherText);
        var hashMAC = Hashes.HMACSHA256(hashingKey, encrypted);
        return encrypted.Concat(hashMAC);
    }

    #endregion
}
