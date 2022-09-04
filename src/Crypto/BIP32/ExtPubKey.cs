#nullable enable
using System.Collections;
using SecretNET.Crypto;

namespace SecretNET.Crypto.BIP32;

/// <summary>
/// A public HD key
/// </summary>
internal class ExtPubKey : IHDKey, IEquatable<ExtPubKey>
{

    private const int ChainCodeLength = 32;

    internal readonly byte nDepth;
    internal readonly HDFingerprint parentFingerprint;
    internal readonly uint nChild;

    internal readonly PubKey pubkey;
    internal readonly byte[] vchChainCode = new byte[ChainCodeLength];

    internal byte Depth
    {
        get
        {
            return nDepth;
        }
    }

    internal uint Child
    {
        get
        {
            return nChild;
        }
    }

    internal bool IsHardened
    {
        get
        {
            return (nChild & 0x80000000u) != 0;
        }
    }
    internal PubKey PubKey
    {
        get
        {
            return pubkey;
        }
    }
    internal byte[] ChainCode
    {
        get
        {
            return vchChainCode;
        }
    }

    /// <summary>
    /// Constructor. Creates a new extended public key from the specified extended public key bytes.
    /// </summary>
    internal ExtPubKey(byte[] bytes) : this(bytes, 0, bytes.Length)
    {
    }

    /// <summary>
    /// Creates a new extended public key from the specified extended public key bytes.
    /// </summary>
    internal ExtPubKey(byte[] bytes, int offset, int length)
    {
        if (bytes == null)
            throw new ArgumentNullException(nameof(bytes));
        if (bytes.Length - offset != Length)
            throw new FormatException($"An extpubkey should be {Length} bytes");
        int i = offset;
        nDepth = bytes[i];
        i++;
        parentFingerprint = new HDFingerprint(bytes, i);
        i += 4;
        nChild = Utils.ToUInt32(bytes, i, false);
        i += 4;
        vchChainCode = new byte[32];
        Array.Copy(bytes, i, vchChainCode, 0, 32);
        i += 32;
        var pk = new byte[33];
        Array.Copy(bytes, i, pk, 0, 33);
        pubkey = new PubKey(pk);
    }

    /// <summary>
    /// Creates a new extended public key from the specified extended public key bytes.
    /// </summary>
    internal ExtPubKey(ReadOnlySpan<byte> bytes)
	{
		if (bytes == null)
			throw new ArgumentNullException(nameof(bytes));
		if (bytes.Length != Length)
			throw new FormatException($"An extpubkey should be {Length} bytes");
		int i = 0;
		nDepth = bytes[i];
		i++;
		parentFingerprint = new HDFingerprint(bytes.Slice(i, 4));
		i += 4;
		nChild = Utils.ToUInt32(bytes.Slice(i, 4), false);
		i += 4;
		vchChainCode = new byte[32];
		bytes.Slice(i, 32).CopyTo(vchChainCode);
		i += 32;
		Span<byte> pk = stackalloc byte[33];
		bytes.Slice(i, 33).CopyTo(pk);
		pubkey = new PubKey(pk);
	}

    /// <summary>
    /// Constructor. Creates a new extended public key from the specified extended public key bytes, from the given hex string.
    /// </summary>
    internal ExtPubKey(string hex)
        : this(Encoders.Hex.DecodeData(hex))
    {
    }

    internal ExtPubKey(PubKey pubkey, byte[] chainCode, byte depth, HDFingerprint fingerprint, uint child)
    {
        if (pubkey == null)
            throw new ArgumentNullException(nameof(pubkey));
        if (chainCode == null)
            throw new ArgumentNullException(nameof(chainCode));
        if (chainCode.Length != ChainCodeLength)
            throw new ArgumentException(string.Format("The chain code must be {0} bytes.", ChainCodeLength), "chainCode");
        this.pubkey = pubkey;
        nDepth = depth;
        nChild = child;
        parentFingerprint = fingerprint;
        Buffer.BlockCopy(chainCode, 0, vchChainCode, 0, ChainCodeLength);
    }

    internal ExtPubKey(PubKey masterKey, byte[] chainCode)
    {
        if (masterKey == null)
            throw new ArgumentNullException(nameof(masterKey));
        if (chainCode == null)
            throw new ArgumentNullException(nameof(chainCode));
        if (chainCode.Length != ChainCodeLength)
            throw new ArgumentException(string.Format("The chain code must be {0} bytes.", ChainCodeLength), "chainCode");
        pubkey = masterKey;
        Buffer.BlockCopy(chainCode, 0, vchChainCode, 0, ChainCodeLength);
    }


    internal bool IsChildOf(ExtPubKey parentKey)
    {
        if (Depth != parentKey.Depth + 1)
            return false;
        return parentKey.PubKey.GetHDFingerPrint() == ParentFingerprint;
    }
    internal bool IsParentOf(ExtPubKey childKey)
    {
        return childKey.IsChildOf(this);
    }

    internal HDFingerprint ParentFingerprint
    {
        get
        {
            return parentFingerprint;
        }
    }

    internal ExtPubKey Derive(uint index)
    {
        var childPubKey = pubkey.Derivate(vchChainCode, index, out var chainCode);
        var result = new ExtPubKey(childPubKey, chainCode, (byte)(nDepth + 1), PubKey.GetHDFingerPrint(), index);
        return result;
    }

    internal ExtPubKey Derive(KeyPath derivation)
    {
        ExtPubKey result = this;
        return derivation.Indexes.Aggregate(result, (current, index) => current.Derive(index));
    }

    internal ExtPubKey Derive(int index, bool hardened)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException("index", "the index can't be negative");
        uint realIndex = (uint)index;
        realIndex = hardened ? realIndex | 0x80000000u : realIndex;
        return Derive(realIndex);
    }

    internal const int Length = 1 + 4 + 4 + 32 + 33;
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
        Array.Copy(pubkey.ToBytes(), 0, b, i, 33);
        return b;
    }

    public override bool Equals(object? obj)
    {
        if (obj is ExtPubKey other)
        {
            return Depth == other.Depth &&
                   ParentFingerprint == other.ParentFingerprint &&
                   PubKey == other.PubKey &&
                   Child == other.Child &&
                   StructuralComparisons.StructuralEqualityComparer.Equals(vchChainCode, other.vchChainCode);
        }
        return false;
    }
    public static bool operator ==(ExtPubKey? a, ExtPubKey? b)
    {
        if (a is ExtPubKey && b is ExtPubKey)
            return a.Equals(b);
        return a is null && b is null;
    }

    public static bool operator !=(ExtPubKey? a, ExtPubKey? b)
    {
        return !(a == b);
    }

    int hashcode = 0;
    public override int GetHashCode()
    {
        if (hashcode != 0)
            return hashcode;
        hashcode = Encoders.Hex.EncodeData(ToBytes()).GetHashCode();
        return hashcode;
    }

    IHDKey IHDKey.Derive(KeyPath keyPath)
    {
        return Derive(keyPath);
    }

    public PubKey GetPublicKey()
    {
        return pubkey;
    }

    bool IHDKey.CanDeriveHardenedPath()
    {
        return false;
    }

    public bool Equals(ExtPubKey? other)
    {
        if (other is null)
            return false;
        return Depth == other.Depth &&
                   ParentFingerprint == other.ParentFingerprint &&
                   PubKey == other.PubKey &&
                   Child == other.Child &&
                   StructuralComparisons.StructuralEqualityComparer.Equals(vchChainCode, other.vchChainCode);
    }

}
