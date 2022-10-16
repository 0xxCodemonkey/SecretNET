namespace SecretNET.Common.Storage;

/// <summary>
/// Interface IPrivateKeyStorage
/// </summary>
public interface IPrivateKeyStorage
{
    /// <summary>
    /// Gets or sets the encryption method.
    /// </summary>
    /// <value>The encryption method.</value>
    public PrivateKeyStorageEncryptionEnum EncryptionMethod { get; set; }

    /// <summary>
    /// Gets the private key.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <returns>Task&lt;System.Byte[]&gt;.</returns>
    public Task<byte[]> GetPrivateKey(string address);

    /// <summary>
    /// Gets the first key.
    /// </summary>
    /// <returns>Task&lt;System.Byte[]&gt;.</returns>
    public Task<byte[]> GetFirstKey();

    /// <summary>
    /// Saves the private key.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <param name="data">The data.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public Task SavePrivateKey(string address, byte[] data);

    /// <summary>
    /// Removes the private key.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public Task<bool> RemovePrivateKey(string address);

    /// <summary>
    /// Gets the stored key adress list.
    /// </summary>
    /// <returns>Task&lt;List&lt;System.String&gt;&gt;.</returns>
    public Task<List<string>> GetStoredKeyAdressList();

    /// <summary>
    /// Determines whether [has private key] [the specified address].
    /// </summary>
    /// <param name="address">The address.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public Task<bool> HasPrivateKey(string address = null);

    /// <summary>
    /// Gets the mnemonic.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <returns>Task&lt;System.String&gt;.</returns>
    public Task<string> GetMnemonic(string address);

    /// <summary>
    /// Gets the first mnemonic.
    /// </summary>
    /// <returns>Task&lt;System.String&gt;.</returns>
    public Task<string> GetFirstMnemonic();

    /// <summary>
    /// Saves the mnemonic.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <param name="mnemonic">The mnemonic.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public Task SaveMnemonic(string address, string mnemonic);

    /// <summary>
    /// Removes the mnemonic.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public Task<bool> RemoveMnemonic(string address);

    /// <summary>
    /// Gets the stored mnemonic adress list.
    /// </summary>
    /// <returns>Task&lt;List&lt;System.String&gt;&gt;.</returns>
    public Task<List<string>> GetStoredMnemonicAdressList();

    /// <summary>
    /// Determines whether the specified address has mnemonic.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public Task<bool> HasMnemonic(string address = null);

    /// <summary>
    /// Gets the tx encryption key. By default, this is derived from a signed message (Keplr style) and gets stored. 
    /// </summary>
    /// <param name="address">The address.</param>
    /// <returns>Task&lt;System.Byte[]&gt;.</returns>
    public Task<byte[]> GetTxEncryptionKey(string address);

    /// <summary>
    /// Sets the tx encryption key and saves it to the storage.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <param name="txEncryptionKey">The tx encryption key.</param>
    /// <returns>Task.</returns>
    public Task SetTxEncryptionKey(string address, byte[] txEncryptionKey);

    /// <summary>
    /// Removes the tx encryption key from the storage.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <returns>Task.</returns>
    public Task RemoveTxEncryptionKey(string address);

    /// <summary>
    /// Removes the private key and mnemonic for the given address.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <returns>Task&lt;System.ValueTuple&lt;System.Boolean, System.Boolean&gt;&gt;.</returns>
    public Task<(bool removedPrivateKey, bool removedMnemonic)> Remove(string address);

}
