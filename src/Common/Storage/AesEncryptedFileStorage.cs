namespace SecretNET.Common.Storage;

/// <summary>
/// AesEncryptedFileStorage encryptes data to the local file system.
/// Implements the <see cref="SecretNET.Common.Storage.HotPrivateKeyStorageBase" />
/// </summary>
/// <seealso cref="SecretNET.Common.Storage.HotPrivateKeyStorageBase" />
public class AesEncryptedFileStorage : HotPrivateKeyStorageBase
{

    private byte[] _aesEncryptionKey = null;
    private string _dataFolderPath = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="AesEncryptedFileStorage" /> class.
    /// </summary>
    /// <param name="dataFolderPath">The data folder path.</param>
    /// <param name="password">The password.</param>
    /// <exception cref="System.InvalidOperationException">The password must not be empty!</exception>
    /// <exception cref="System.InvalidOperationException">The password must have at least a length >= 8 !</exception>
    public AesEncryptedFileStorage(string dataFolderPath, string password) : base(PrivateKeyStorageEncryptionEnum.Password)
    {
        if (String.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("The password must not be empty!");
        }
        if (password.Length < 8)
        {
            throw new InvalidOperationException("The password must have at least a length >= 8 !");
        }

        _dataFolderPath = dataFolderPath ?? "";

        _aesEncryptionKey = SecretNET.Crypto.Hashes.DoubleSHA256(Encoding.UTF8.GetBytes(password)).ToBytes();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AesEncryptedFileStorage"/> class.
    /// </summary>
    /// <param name="aesEncryptionKey">The aes encryption key.</param>
    /// <exception cref="System.InvalidOperationException">The AES-EncryptionKey must not be empty!</exception>
    /// <exception cref="System.InvalidOperationException">The AES-EncryptionKey must have at least a length >= 32 !</exception>
    public AesEncryptedFileStorage(byte[] aesEncryptionKey) : base(PrivateKeyStorageEncryptionEnum.Password)
    {
        if (aesEncryptionKey == null || aesEncryptionKey.Length == 0)
        {
            throw new InvalidOperationException("The AES-EncryptionKey must not be empty!");
        }
        if (aesEncryptionKey.Length < 32)
        {
            throw new InvalidOperationException("The AES-EncryptionKey must have at least a length >= 32 !");
        }

        _aesEncryptionKey = aesEncryptionKey;
    }

    /// <summary>
    /// Gets data from an AES encrypted file.
    /// </summary>
    /// <param name="filePath">Name of the file.</param>
    /// <returns>Task&lt;System.String&gt;.</returns>
    protected override async Task<string> GetFromStorage(string filePath)
    {
        var fileInfo = GetFileInfo(filePath);
        if (fileInfo.fileExists)
        {
            string result = await Utils.AesDecryptFile(filePath, _aesEncryptionKey);
            if (result.Length > 0)
            {
                return result;
            }
        }
        return null;
    }

    /// <summary>
    /// Saves data AES encrypted to a file.
    /// </summary>
    /// <param name="filePath">Name of the file.</param>
    /// <param name="data">The data.</param>
    /// <returns>Task.</returns>
    protected override Task SaveToStorage(string filePath, string data)
    {
        Utils.AesEncryptToFile(filePath, data, _aesEncryptionKey);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Removes data from the AES encrypted file.
    /// </summary>
    /// <param name="filePath">Name of the file.</param>
    /// <returns>Task.</returns>
    protected override Task RemoveFromStorage(string filePath)
    {
        var fileInfo = GetFileInfo(filePath);
        if (fileInfo.fileExists)
        {
            File.Delete(filePath);
        }
        
        return Task.CompletedTask;
    }

    // helper methods

    /// <summary>
    /// Gets the file information.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>(string filePath, bool fileExists).</returns>
    protected (string filePath, bool fileExists) GetFileInfo(string fileName)
    {
        var filePath = Path.Combine(_dataFolderPath, fileName);
        var fileExists = File.Exists(filePath);

        return (filePath, fileExists);
    }
}
