using System.Security.Cryptography;

namespace SecretNET.Crypto;

internal class AesWrapper
{
    private Aes _inner;
    private ICryptoTransform _transformer;
    private AesWrapper(Aes aes)
    {
        _inner = aes;
    }

    internal static AesWrapper Create()
    {
        var aes = Aes.Create();
        return new AesWrapper(aes);
    }

    internal byte[] Process(byte[] inputBuffer, int inputOffset, int inputCount)
    {
        return _transformer.TransformFinalBlock(inputBuffer, inputOffset, inputCount);
    }

    internal void Initialize(byte[] key, byte[] iv, bool forEncryption)
    {
        if (_transformer != null)
            return;
        _inner.IV = iv;
        _inner.KeySize = key.Length * 8;
        _inner.Key = key;
        _transformer = forEncryption ? _inner.CreateEncryptor() : _inner.CreateDecryptor();
    }
}

internal class AesBuilder
{
    private byte[] _key;
    private bool? _forEncryption;

    private byte[] _iv = new byte[16];


    internal AesBuilder SetKey(byte[] key)
    {
        _key = key;
        return this;
    }

    internal AesBuilder IsUsedForEncryption(bool forEncryption)
    {
        _forEncryption = forEncryption;
        return this;
    }

    internal AesBuilder SetIv(byte[] iv)
    {
        _iv = iv;
        return this;
    }

    internal AesWrapper Build()
    {
        var aes = AesWrapper.Create();
        var encrypt = !_forEncryption.HasValue || _forEncryption.Value;
        aes.Initialize(_key, _iv, encrypt);
        return aes;
    }
}