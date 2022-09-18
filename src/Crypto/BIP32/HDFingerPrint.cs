﻿using SecretNET.Crypto;

namespace SecretNET.Crypto.BIP32;

internal readonly struct HDFingerprint
{
    readonly uint _Value;
    internal HDFingerprint(byte[] bytes)
    {
        if (bytes == null)
            throw new ArgumentNullException(nameof(bytes));
        if (bytes.Length != 4)
            throw new ArgumentException(paramName: nameof(bytes), message: "Bytes should be of length 4");
        _Value = Utils.ToUInt32(bytes, true);
    }

    internal static bool TryParse(string str, out HDFingerprint result)
    {
        if (str == null)
            throw new ArgumentNullException(nameof(str));
        result = default;
        if (!HexEncoder.IsWellFormed(str) || str.Length != 4 * 2)
            return false;
        result = new HDFingerprint(Encoders.Hex.DecodeData(str));
        return true;
    }

    internal static HDFingerprint Parse(string str)
    {
        if (!TryParse(str, out var result))
            throw new FormatException("Invalid HD Fingerprint");
        return result;
    }

    internal HDFingerprint(ReadOnlySpan<byte> bytes)
	{
		if (bytes.Length != 4)
			throw new ArgumentException(paramName: nameof(bytes), message: "Bytes should be of length 4");
		_Value = Utils.ToUInt32(bytes, true);
	}

    internal static HDFingerprint FromKeyId(KeyId id)
    {
        return new HDFingerprint(id.ToBytes().Take(4).ToArray());
    }

    internal HDFingerprint(byte[] bytes, int index)
    {
        if (bytes == null)
            throw new ArgumentNullException(nameof(bytes));
        _Value = Utils.ToUInt32(bytes, index, true);
    }

    internal HDFingerprint(uint value)
    {
        _Value = value;
    }

    internal byte[] ToBytes()
    {
        return Utils.ToBytes(_Value, true);
    }

    internal void ToBytes(Span<byte> output)
	{
		Utils.ToBytes(_Value, true, output);
	}

    public override bool Equals(object obj)
    {
        return obj is HDFingerprint && this == (HDFingerprint)obj;
    }
    public override int GetHashCode()
    {
        return _Value.GetHashCode();
    }
    public static bool operator ==(HDFingerprint x, HDFingerprint y)
    {
        return x._Value == y._Value;
    }
    public static bool operator !=(HDFingerprint x, HDFingerprint y)
    {
        return !(x == y);
    }

    public override string ToString()
    {
        return Encoders.Hex.EncodeData(ToBytes());
    }
}