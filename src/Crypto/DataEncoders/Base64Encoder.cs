using System;

namespace SecretNET.Crypto.DataEncoders
{
    internal class Base64Encoder : DataEncoder
    {
        internal override byte[] DecodeData(string encoded)
        {
            return Convert.FromBase64String(encoded);
        }

        internal override string EncodeData(byte[] data, int offset, int count)
        {
            return Convert.ToBase64String(data, offset, count);
        }
    }
}
