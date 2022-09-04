using SecretNET.Crypto;
using Utils = SecretNET.Common.Utils;

namespace SecretNET.Crypto.BIP32;

internal class ExtKey
{
    static readonly byte[] hashkey = Encoders.ASCII.DecodeData("Bitcoin seed");

    private const int ChainCodeLength = 32;

    readonly Key key;
    readonly byte[] vchChainCode;
    readonly uint nChild;
    readonly byte nDepth;
    readonly HDFingerprint parentFingerprint = default;

    internal const int Length = 1 + 4 + 4 + 32 + 33;

    /// <summary>
    /// Get the private key of this extended key.
    /// </summary>
    internal Key PrivateKey
    {
        get
        {
            return key;
        }
    }

    internal PubKey GetPublicKey()
    {
        return PrivateKey.PubKey;
    }

    internal byte[] ToBytes()
    {
        var b = new byte[Length];
        int i = 0;
        b[i++] = nDepth;
        Array.Copy(parentFingerprint.ToBytes(), 0, b, i, 4);
        i += 4;
        Array.Copy(Utils.ToBytes(nChild, false), 0, b, i, 4);
        i += 4;
        Array.Copy(vchChainCode, 0, b, i, 32);
        i += 32;
        b[i++] = 0;
        Array.Copy(key.ToBytes(), 0, b, i, 32);
        return b;
    }

    /// <summary>
    /// Constructor. Creates an extended key from the private key, and specified values for
    /// chain code, depth, fingerprint, and child number.
    /// </summary>
    internal ExtKey(Key key, byte[] chainCode, byte depth, HDFingerprint fingerprint, uint child)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        if (chainCode == null)
            throw new ArgumentNullException(nameof(chainCode));
        if (chainCode.Length != ChainCodeLength)
            throw new ArgumentException(string.Format("The chain code must be {0} bytes.", ChainCodeLength), "chainCode");
        this.key = key;
        this.nDepth = depth;
        this.nChild = child;
        parentFingerprint = fingerprint;
        vchChainCode = new byte[32];
        Buffer.BlockCopy(chainCode, 0, vchChainCode, 0, ChainCodeLength);
    }

    internal static ExtKey CreateFromSeed(ReadOnlySpan<byte> seed)
    {
        return new ExtKey(seed, true);
    }

    private ExtKey(ReadOnlySpan<byte> bytes, bool isSeed)
    {
        if (isSeed)
        {
            key = CalculateKey(bytes, out var cc);
            vchChainCode = cc;
        }
        else
        {
            if (bytes.Length != Length)
                throw new FormatException($"An extpubkey should be {Length} bytes");
            int i = 0;
            nDepth = bytes[i];
            i++;
            parentFingerprint = new HDFingerprint(bytes.Slice(1, 4));
            i += 4;
            nChild = Utils.ToUInt32(bytes.Slice(i, 4), false);
            i += 4;
            vchChainCode = new byte[32];
            bytes.Slice(i, 32).CopyTo(vchChainCode);
            i += 32;
            if (bytes[i++] != 0)
                throw new FormatException($"Invalid ExtKey");
            Span<byte> pk = stackalloc byte[32];
            bytes.Slice(i, 32).CopyTo(pk);
            key = new Key(pk);
        }
    }

    private static Key CalculateKey(ReadOnlySpan<byte> seed, out byte[] chainCode)
    {
        Span<byte> hashMAC = stackalloc byte[64];
        if (Hashes.HMACSHA512(hashkey, seed, hashMAC, out int len) &&
            len == 64 &&
            SecretContext.Instance.TryCreateECPrivKey(hashMAC.Slice(0, 32), out var k) && k is ECPrivKey)
        {
            var key = new Key(k, true);
            chainCode = new byte[32];
            hashMAC.Slice(32, ChainCodeLength).CopyTo(chainCode);
            hashMAC.Clear();
            return key;
        }
        else
        {
            throw new InvalidOperationException("Invalid ExtKey (this should never happen)");
        }
    }

    internal ExtKey Derive(RootedKeyPath rootedKeyPath)
    {
        if (rootedKeyPath == null)
            throw new ArgumentNullException(nameof(rootedKeyPath));
        if (rootedKeyPath.MasterFingerprint != GetPublicKey().GetHDFingerPrint())
            throw new ArgumentException(paramName: nameof(rootedKeyPath), message: "The rootedKeyPath's fingerprint does not match this ExtKey");
        return Derive(rootedKeyPath.KeyPath);
    }

    internal ExtKey Derive(KeyPath keyPath)
    {
        if (keyPath == null)
            throw new ArgumentNullException(nameof(keyPath));
        ExtKey result = this;
        return keyPath.Indexes.Aggregate(result, (current, index) => current.Derive(index));
    }

    /// <summary>
    /// Derives a new extended key in the hierarchy as the given child number.
    /// </summary>
    internal ExtKey Derive(uint index)
    {
        var childkey = key.Derivate(this.vchChainCode, index, out var childcc);
        return new ExtKey(childkey, childcc, (byte)(nDepth + 1), this.key.PubKey.GetHDFingerPrint(), index);
    }
}
