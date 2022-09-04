namespace SecretNET.Crypto.Miscreant.providers;

/// <summary>
/// AES-CTR (counter) mode of operation.
/// 
/// Uses a non-constant-time (lookup table-based) software AES implementation.
/// See soft/aes.ts for more information on the security impact.
/// 
/// Note that CTR mode is malleable and generally should not be used without
/// authentication. Instead, use an authenticated encryption mode, like AES-SIV!
/// Implements the <see cref="ICTRLike" />
/// </summary>
/// <seealso cref="ICTRLike" />
internal class SoftAesCtr : ICTRLike
{
    private Block _counter = new Block();
    private Block _buffer = new Block();
    private SoftAes _cipher;

    /// <summary>
    /// Initializes a new instance of the <see cref="SoftAesCtr"/> class.
    /// </summary>
    /// <param name="cipher">The cipher.</param>
    internal SoftAesCtr(SoftAes cipher)
    {
        _cipher = cipher;
    }

    /// <summary>
    /// Clears this instance.
    /// </summary>
    /// <returns>ICTRLike.</returns>
    ICTRLike ICTRLike.Clear()
    {
        _buffer.Clear();
        _counter.Clear();
        ((IBlockCipher)_cipher).Clear();
        return this;
    }

    byte[] ICTRLike.EncryptCtr(byte[] iv, byte[] plaintext)
    {
        if (iv.Length != Block.SIZE)
        {
            throw new Exception("CTR: iv length must be equal to cipher block size");
        }

        // Copy IV to counter, overwriting it.
        Buffer.BlockCopy(iv, 0, _counter.Data, 0, Block.SIZE);

        // Set buffer position to length of buffer
        // so that the first cipher block is generated.
        var bufferPos = Block.SIZE;

        var result = new byte[plaintext.Length];

        for (var i = 0; i < plaintext.Length; i++)
        {
            if (bufferPos == Block.SIZE)
            {
                _buffer.Copy(_counter);
                ((IBlockCipher)_cipher).EncryptBlock(_buffer);
                bufferPos = 0;
                IncrementCounter(_counter);
            }
            result[i] = (byte)(plaintext[i] ^ _buffer.Data[bufferPos++]);
        }

        return result;
    }

    // Increment an AES-CTR mode counter, intentionally wrapping/overflowing
    private void IncrementCounter(Block counter)
    {
        var carry = 1;

        for (var i = Block.SIZE - 1; i >= 0; i--)
        {
            carry += counter.Data[i] & 0xff | 0;
            counter.Data[i] = (byte)(carry & 0xff);
            carry >>= 8;
        }
    }
}
