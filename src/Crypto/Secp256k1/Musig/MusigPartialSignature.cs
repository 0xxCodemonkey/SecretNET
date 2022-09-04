#nullable enable

namespace SecretNET.Crypto.Secp256k1.Musig;

class MusigPartialSignature
{

    internal readonly Scalar E;

    public MusigPartialSignature(Scalar e)
    {
        this.E = e;
    }

    public MusigPartialSignature(ReadOnlySpan<byte> in32)
    {
        this.E = new Scalar(in32, out var overflow);
        if (overflow != 0)
            throw new ArgumentOutOfRangeException(nameof(in32), "in32 is overflowing");
    }

    public void WriteToSpan(Span<byte> in32)
    {
        E.WriteToSpan(in32);
    }
    public byte[] ToBytes()
    {
        byte[] b = new byte[32];
        WriteToSpan(b);
        return b;
    }
}

