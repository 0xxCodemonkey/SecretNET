namespace SecretNET.Crypto.Miscreant;

/// <summary>
/// A block cipher (with 128-bit blocks, i.e. AES)
/// 
/// This interface should only be used to implement a cipher mode.
/// This library uses it to implement AES-SIV.
/// </summary>
internal interface IBlockCipher
{
    IBlockCipher Clear();
    IBlockCipher EncryptBlock(Block block);
}

/// <summary>
/// A backend which provides an implementation of cryptographic primitives
/// </summary>
internal interface ICryptoProvider
{
    IBlockCipher ImportBlockCipherKey(byte[] keyData);
    ICTRLike ImportCTRKey(byte[] keyData);
}

/// <summary>
/// A cipher which provides CTR (counter mode) encryption
/// </summary>
internal interface ICTRLike
{
    byte[] EncryptCtr(byte[] iv, byte[] plaintext);
    ICTRLike Clear();
}

/// <summary>
/// An implementation of a message authentication code (MAC)
/// </summary>
internal interface IMACLike
{
    IMACLike Reset();
    IMACLike Clear();
    IMACLike Update(byte[] data);
    byte[] Finish();
}

/// <summary>
/// A cipher which provides a SIV-like interface and properties
/// </summary>
public interface ISIVLike
{
    public byte[] Seal(byte[] plaintext, byte[][] associatedData);
    public byte[] Open(byte[] plaintext, byte[][] associatedData);
    public ISIVLike Clear();
}

/// <summary>
/// A cipher which provides an Authenticated Encryption with Associated Data (AEAD) interface
/// </summary>
internal interface IAEADLike
{
    byte[] Seal(byte[] plaintext, byte[][] associatedData);
    byte[] Open(byte[] plaintext, byte[][] associatedData);
    IAEADLike Clear();
}

