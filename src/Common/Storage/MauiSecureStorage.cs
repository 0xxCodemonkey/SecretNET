namespace SecretNET.Common.Storage;


/// <summary>
/// MauiSecureStorage utilized the Microsoft.Maui.Storage.SecureStorage to securely save data (can only be used in MAUI apps).
/// Implements the <see cref="SecretNET.Common.Storage.HotPrivateKeyStorageBase" />
/// </summary>
/// <seealso cref="SecretNET.Common.Storage.HotPrivateKeyStorageBase" />
public class MauiSecureStorage : HotPrivateKeyStorageBase
{

    /// <summary>
    /// Initializes a new instance of the <see cref="MauiSecureStorage" /> class.
    /// </summary>
    public MauiSecureStorage() : base(PrivateKeyStorageEncryptionEnum.SecureStorage)
    {

    }

    /// <summary>
    /// Gets data from the MAUI SecureStorage.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>Task&lt;System.String&gt;.</returns>
    protected override async Task<string> GetFromStorage(string key)
    {
        return await SecureStorage.Default.GetAsync(key);
    }

    /// <summary>
    /// Saves data the MAUI SecureStorage.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="data">The data.</param>
    /// <returns>Task.</returns>
    protected override async Task SaveToStorage(string key, string data)
    {
        await SecureStorage.Default.SetAsync(key, data);
    }

    /// <summary>
    /// Removes data from the MAUI SecureStorage.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>Task.</returns>
    protected override Task RemoveFromStorage(string key)
    {
        SecureStorage.Default.Remove(key);
        return Task.CompletedTask;
    }

}
