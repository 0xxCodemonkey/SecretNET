namespace SecretNET.Common.Storage;

/// <summary>
/// InMemoryOnlyStorage stores the data ONLY (unencrypted!) in memory.
/// Implements the <see cref="SecretNET.Common.Storage.HotPrivateKeyStorageBase" />
/// </summary>
/// <seealso cref="SecretNET.Common.Storage.HotPrivateKeyStorageBase" />
public class InMemoryOnlyStorage : HotPrivateKeyStorageBase
{
    private Dictionary<string, string> _inMemoryStorage { get; } = new Dictionary<string, string>();

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryOnlyStorage"/> class.
    /// </summary>
    public InMemoryOnlyStorage() : base(PrivateKeyStorageEncryptionEnum.InMemory)
    {

    }

    /// <summary>
    /// Gets data from the in memory storage.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>Task&lt;System.String&gt;.</returns>
    protected override Task<string> GetFromStorage(string key)
    {
        _inMemoryStorage.TryGetValue(key, out string data);
        return Task.FromResult(data);
    }

    /// <summary>
    /// Saves data the in memory storage.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="data">The data.</param>
    /// <returns>Task.</returns>
    protected override Task SaveToStorage(string key, string data)
    {
        if (_inMemoryStorage.ContainsKey(key))
        {
            _inMemoryStorage[key] = data;
        }
        else
        {
            _inMemoryStorage.Add(key, data);
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Removes data from the in memory storage.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>Task.</returns>
    protected override Task RemoveFromStorage(string key)
    {
        if (_inMemoryStorage.ContainsKey(key))
        {
            var result = _inMemoryStorage.Remove(key);
        }
        return Task.CompletedTask;
    }

}
