#nullable enable

namespace SecretNET.Crypto;

internal class KeyId
{
    internal KeyId()
        : this(0)
    {

    }
    readonly uint160 v;
    internal KeyId(byte[] value)
    {
        if (value.Length != 20)
            throw new ArgumentException("value should be 20 bytes", "value");
        v = new uint160(value);
    }
    internal KeyId(uint160 value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        v = value;
    }

    internal KeyId(string value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        var bytes = Encoders.Hex.DecodeData(value);
        v = new uint160(bytes);
    }

    public override bool Equals(object? obj)
    {
        if (obj is KeyId id)
            return v == id.v;
        return false;
    }
    public static bool operator ==(KeyId? a, KeyId? b)
    {
        if (a is KeyId && b is KeyId)
            return a.Equals(b);
        return a is null && b is null;
    }

    public static bool operator !=(KeyId? a, KeyId? b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        return v.GetHashCode();
    }

    public byte[] ToBytes()
    {
        return v.ToBytes();
    }

    public override string ToString()
    {
        return Encoders.Hex.EncodeData(v.ToBytes());
    }
}

