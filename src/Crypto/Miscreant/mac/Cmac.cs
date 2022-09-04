namespace SecretNET.Crypto.Miscreant.mac;

internal class Cmac : IMACLike
{
    private Block _buffer = new Block();
    private int _bufferPos = 0;
    private bool _finished = false;

    private IBlockCipher _cipher;
    private Block _subkey1;
    private Block _subkey2;

    internal static IMACLike ImportKey(ICryptoProvider provider, byte[] keyData)
    {
        var cipher = provider.ImportBlockCipherKey(keyData);

        // Generate subkeys.
        var subkey1 = new Block();
        cipher.EncryptBlock(subkey1);
        subkey1.Dbl();

        var subkey2 = subkey1.Clone();
        subkey2.Dbl();

        return new Cmac(cipher, subkey1, subkey2);
    }

    private Cmac(IBlockCipher cipher, Block subkey1, Block subkey2)
    {
        _cipher = cipher;
        _subkey1 = subkey1;
        _subkey2 = subkey2;
    }

    IMACLike IMACLike.Update(byte[] data)
    {
        var left = Block.SIZE - _bufferPos;
        var dataPos = 0;
        var dataLength = data.Length;

        if (dataLength > left)
        {
            for (var i = 0; i < left; i++)
            {
                _buffer.Data[_bufferPos + i] ^= data[i];
            }
            dataLength -= left;
            dataPos += left;
            _cipher.EncryptBlock(_buffer);
            _bufferPos = 0;
        }

        // TODO: use AES-CBC with a span of multiple blocks instead of encryptBlock
        // to encrypt many blocks in a single call to the WebCrypto API
        while (dataLength > Block.SIZE)
        {
            for (var i = 0; i < Block.SIZE; i++)
            {
                _buffer.Data[i] ^= data[dataPos + i];
            }
            dataLength -= Block.SIZE;
            dataPos += Block.SIZE;
            _cipher.EncryptBlock(_buffer);
        }

        for (var i = 0; i < dataLength; i++)
        {
            _buffer.Data[_bufferPos++] ^= data[dataPos + i];
        }

        return this;
    }

    byte[] IMACLike.Finish()
    {
        if (!_finished)
        {
            // Select which subkey to use.
            var subkey = _bufferPos < Block.SIZE ? _subkey2 : _subkey1;

            // XOR in the subkey.
            ByteArrayExtensions.Xor(subkey.Data, _buffer.Data, Block.SIZE);

            // Pad if needed.
            if (_bufferPos < Block.SIZE)
            {
                _buffer.Data[_bufferPos] ^= 0x80;
            }

            // Encrypt buffer to get the final digest.
            _cipher.EncryptBlock(_buffer);

            // Set finished flag.
            _finished = true;
        }

        return _buffer.Clone().Data;
    }

    IMACLike IMACLike.Clear()
    {
        ((IMACLike)this).Reset();
        _subkey1.Clear();
        _subkey2.Clear();

        return this;
    }

    IMACLike IMACLike.Reset()
    {
        _buffer.Clear();
        _bufferPos = 0;
        _finished = false;
        return this;
    }


}
