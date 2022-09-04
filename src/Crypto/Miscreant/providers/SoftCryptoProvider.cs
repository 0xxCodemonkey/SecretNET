namespace SecretNET.Crypto.Miscreant.providers;

internal class SoftCryptoProvider : ICryptoProvider
{
    IBlockCipher ICryptoProvider.ImportBlockCipherKey(byte[] keyData)
    {
        return new SoftAes(keyData);
    }

    ICTRLike ICryptoProvider.ImportCTRKey(byte[] keyData)
    {
        return new SoftAesCtr(new SoftAes(keyData));
    }
}
