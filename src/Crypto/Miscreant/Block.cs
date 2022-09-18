namespace SecretNET.Crypto.Miscreant;

internal class Block
{
    /// <summary>
    /// Size of a block as used by the AES cipher
    /// </summary>
    internal static readonly int SIZE = 16;

    /// <summary>
    /// Minimal irreducible polynomial for a 128-bit block size
    /// </summary>
    internal static readonly byte R = 0x87;

    internal byte[] Data { get; set; }


    internal Block()
    {
        Data = new byte[SIZE];
    }


    /// <summary>
    /// Clear the given array by setting its values to zero.
    /// 
    /// WARNING: The fact that it sets bytes to zero can be relied on.
    /// 
    /// There is no guarantee that this function makes data disappear from memory,
    /// as runtime implementation can, for example, have copying garbage collector
    /// that will make copies of sensitive data before we wipe it.Or that an
    /// operating system will write our data to swap or sleep image.Another thing
    /// is that an optimizing compiler can remove calls to this function or make it
    /// no-op.There's nothing we can do with it, so we just do our best and hope
    /// that everything will be okay and good will triumph over evil.        /// 
    /// </summary>
    internal void Clear()
    {
        ByteArrayExtensions.Wipe(Data);
    }

    /// <summary>
    /// Make a copy of this block, returning a new block
    /// </summary>
    /// <returns>Block.</returns>
    internal Block Clone()
    {
        var ret = new Block();
        ret.Copy(this);
        return ret;
    }

    /// <summary>
    /// Copy the contents of another block into this one
    /// </summary>
    internal void Copy(Block other)
    {
        Buffer.BlockCopy(other.Data, 0, Data, 0, SIZE);
    }

    /// <summary>
    /// Double a value over GF(2^128)
    /// </summary>
    internal void Dbl()
    {
        int carry = 0;

        for (var i = SIZE - 1; i >= 0; i--)
        {
            var b = Data[i] >> 7 & 0xff;
            Data[i] = (byte)(Data[i] << 1 | carry);
            carry = b;
        }

        Data[SIZE - 1] ^= Select(carry, R, 0);
        carry = 0;
    }

    /// <summary>
    /// Returns resultIfOne if subject is 1, or resultIfZero if subject is 0.
    /// 
    /// Supports only 32-bit integers, so resultIfOne or resultIfZero are not
    /// integers, they'll be converted to them with bitwise operations.
    /// </summary>
    /// <param name="subject">The subject.</param>
    /// <param name="resultIfOne">The result if one.</param>
    /// <param name="resultIfZero">The result if zero.</param>
    /// <returns>System.Byte.</returns>
    private byte Select(int subject, byte resultIfOne, byte resultIfZero)
    {
        return (byte)(~(subject - 1) & resultIfOne | subject - 1 & resultIfZero);
    }
}
