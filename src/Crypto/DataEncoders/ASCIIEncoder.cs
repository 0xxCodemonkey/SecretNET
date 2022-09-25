namespace SecretNET.Crypto.DataEncoders;

internal class ASCIIEncoder : DataEncoder
{
    //Do not using Encoding.ASCII (not portable)
    internal override byte[] DecodeData(string encoded)
    {
        if (string.IsNullOrWhiteSpace(encoded))
            return new byte[0];

		Span<byte> r = encoded.Length is int v && v > 256 ? new byte[v] : stackalloc byte[v];

        for (int i = 0; i < r.Length; i++)
        {
            r[i] = (byte)encoded[i];
        }
        return r.ToArray();
    }

    internal void DecodeData(string encoded, Span<byte> output)
	{
		var l = encoded.Length;
		for (int i = 0; i < l; i++)
		{
			output[i] = (byte)encoded[i];
		}
	}

    internal override string EncodeData(byte[] data, int offset, int count)
    {
        return new string(data.Skip(offset).Take(count).Select(o => (char)o).ToArray()).Replace("\0", "");
    }
}
