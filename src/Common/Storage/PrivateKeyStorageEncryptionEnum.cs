namespace SecretNET.Common.Storage;

/// <summary>
/// Enum PrivateKeyStorageEncryptionEnum
/// </summary>
public enum PrivateKeyStorageEncryptionEnum : byte
{
    /// <summary>
    /// Utilized Microsoft.Maui.Storage.SecureStorage to securely save data.
    /// see https://docs.microsoft.com/en-us/dotnet/maui/platform-integration/storage/secure-storage 
    /// </summary>
    SecureStorage = 0,

    /// <summary>
    /// Encryptes data with a password (not recommended in MAUI Apps since password is stored in memory)
    /// </summary>
    Password = 1,

    /// <summary>
    /// Saves the data in memory (potentially insecure, as data may be extractable via dump)
    /// </summary>
    InMemory = 2
}
