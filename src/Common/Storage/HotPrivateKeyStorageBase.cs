namespace SecretNET.Common.Storage;

/// <summary>
/// Base class for hot private key storages like SecureStore or file encrypted stores.
/// Implements the <see cref="SecretNET.Common.Storage.IPrivateKeyStorage" />
/// </summary>
/// <seealso cref="SecretNET.Common.Storage.IPrivateKeyStorage" />
public abstract class HotPrivateKeyStorageBase : IPrivateKeyStorage
{
    /// <summary>
    /// Gets or sets the encryption method.
    /// </summary>
    /// <value>The encryption method.</value>
    public PrivateKeyStorageEncryptionEnum EncryptionMethod { get; set; }

    /// <summary>
    /// The private key store key pattern
    /// </summary>
    protected string _privateKeyStoreKeyPattern = "PK_{0}";

    /// <summary>
    /// The mnemonic store key pattern
    /// </summary>
    protected string _mnemonicStoreKeyPattern = "M_{0}";

    /// <summary>
    /// The TxEncryptionKey store key pattern
    /// </summary>
    protected string _txEncryptionKeyStoreKeyPattern = "TK_{0}";

    /// <summary>
    /// The stored keys list name
    /// </summary>
    protected string _storedKeysListName = "storedKeys";

    /// <summary>
    /// The stored mnemonic list name
    /// </summary>
    protected string _storedMnemonicListName = "storedMnemonic";    

    /// <summary>
    /// The stored key adress list
    /// </summary>
    protected List<string> _storedKeyAdressList = null;

    /// <summary>
    /// The stored mnemonic adress list
    /// </summary>
    protected List<string> _storedMnemonicAdressList = null;


    /// <summary>
    /// Initializes a new instance of the <see cref="HotPrivateKeyStorageBase"/> class.
    /// </summary>
    /// <param name="encryptionMethod">The encryption method.</param>
    public HotPrivateKeyStorageBase(PrivateKeyStorageEncryptionEnum encryptionMethod)
    {
        this.EncryptionMethod = encryptionMethod;        
    }

    // PrivateKey

    /// <summary>
    /// Gets data from storage.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>Task&lt;System.String&gt;.</returns>
    abstract protected Task<string> GetFromStorage(string key);

    /// <summary>
    /// Saves data to storage.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="data">The data.</param>
    /// <returns>Task.</returns>
    abstract protected Task SaveToStorage(string key, string data);

    /// <summary>
    /// Removes data from storage.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>Task.</returns>
    abstract protected Task RemoveFromStorage(string key);

    /// <inheritdoc/>
    public virtual async Task<byte[]> GetPrivateKey(string address)
    {
        if (String.IsNullOrWhiteSpace(address)) 
            throw new ArgumentOutOfRangeException("address is null or empty!");

        string base64PivateKey = await GetFromStorage(HashStorageKey(string.Format(_privateKeyStoreKeyPattern, address)));
        if (!String.IsNullOrWhiteSpace(base64PivateKey))
        {
            return Convert.FromBase64String(base64PivateKey);
        }

        return null;
    }

    /// <inheritdoc/>
    public virtual async Task<byte[]> GetFirstKey()
    {
        if (await HasPrivateKey())
        {
            var firstAddress = (await GetStoredKeyAdressList()).First();
            return await GetPrivateKey(firstAddress);
        }
        return null;
    }

    /// <inheritdoc/>
    public virtual async Task SavePrivateKey(string address, byte[] data)
    {
        if (String.IsNullOrWhiteSpace(address)) 
            throw new ArgumentOutOfRangeException("address is null or empty!");

        if (data.Length != 32)
            throw new ArgumentOutOfRangeException("private key length is not 32!");

        try
        {
            var keyAddressList = await GetStoredKeyAdressList();

            await SaveToStorage(HashStorageKey(string.Format(_privateKeyStoreKeyPattern, address)), Convert.ToBase64String(data));

            // Update stored keys JSON => for HasPrivateKey
            if (!keyAddressList.Contains(address))
            {
                keyAddressList.Add(address);
            }

            await SaveToStorage(HashStorageKey(_storedKeysListName), JsonConvert.SerializeObject(keyAddressList));
            _storedKeyAdressList = keyAddressList;            
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }        
    }

    /// <inheritdoc/>
    public virtual async Task<bool> RemovePrivateKey(string address)
    {
        if (String.IsNullOrWhiteSpace(address))
            throw new ArgumentOutOfRangeException("address is null or empty!");

        var keyAddressList = await GetStoredKeyAdressList();

        await RemoveFromStorage(HashStorageKey(string.Format(_privateKeyStoreKeyPattern, address)));

        // Update stored keys JSON => for HasPrivateKey
        if (keyAddressList.Contains(address))
        {
            keyAddressList.Remove(address);
        }

        await SaveToStorage(HashStorageKey(_storedKeysListName), JsonConvert.SerializeObject(keyAddressList));
        _storedKeyAdressList = keyAddressList;

        return false;
    }

    /// <inheritdoc/>
    public virtual async Task<List<string>> GetStoredKeyAdressList()
    {
        if (_storedKeyAdressList != null)
        {
            return _storedKeyAdressList;
        }

        List<string> keyAddressList = null;

        // Use stored adresses JSON
        var storedKeys = await GetFromStorage(HashStorageKey(_storedKeysListName));
        if (!String.IsNullOrWhiteSpace(storedKeys))
        {
            keyAddressList = JsonConvert.DeserializeObject<List<string>>(storedKeys);
        }

        keyAddressList = keyAddressList ?? new List<string>();
        return keyAddressList;
    }

    /// <inheritdoc/>
    public virtual async Task<bool> HasPrivateKey(string address = null)
    {
        try
        {
            var keyAddressList = await GetStoredKeyAdressList();

            if (!String.IsNullOrWhiteSpace(address))
            {
                return keyAddressList.Contains(address);
            }
            else
            {
                return keyAddressList.Any();
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }        
    }


    // Mnemonic

    /// <inheritdoc/>
    public virtual async Task<string> GetMnemonic(string address)
    {
        if (String.IsNullOrWhiteSpace(address))
            throw new ArgumentOutOfRangeException("address is null or empty!");

        try
        {
            return await GetFromStorage(HashStorageKey(string.Format(_mnemonicStoreKeyPattern, address)));
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    /// <inheritdoc/>
    public virtual async Task<string> GetFirstMnemonic()
    {
        if (await HasMnemonic())
        {
            var firstAddress = (await GetStoredMnemonicAdressList()).First();
            return await GetMnemonic(firstAddress);
        }
        return null;
    }

    /// <inheritdoc/>
    public virtual async Task SaveMnemonic(string address, string mnemonic)
    {
        if (String.IsNullOrWhiteSpace(address))
            throw new ArgumentOutOfRangeException("address is null or empty!");

        if (String.IsNullOrWhiteSpace(mnemonic))
            throw new ArgumentOutOfRangeException("mnemonic is null or empty!");

        try
        {
            var mnemonicAddressList = await GetStoredMnemonicAdressList();

            await SaveToStorage(HashStorageKey(string.Format(_mnemonicStoreKeyPattern, address)), mnemonic);

            // Update stored mnemonic JSON => for HasMnemonic
            if (!mnemonicAddressList.Contains(address))
            {
                mnemonicAddressList.Add(address);
            }

            await SaveToStorage(HashStorageKey(_storedMnemonicListName), JsonConvert.SerializeObject(mnemonicAddressList));
            _storedMnemonicAdressList = mnemonicAddressList;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    /// <inheritdoc/>
    public virtual async Task<bool> RemoveMnemonic(string address)
    {
        if (String.IsNullOrWhiteSpace(address))
            throw new ArgumentOutOfRangeException("address is null or empty!");

        try
        {
            var mnemonicAddressList = await GetStoredMnemonicAdressList();

            await RemoveFromStorage(HashStorageKey(string.Format(_mnemonicStoreKeyPattern, address)));

            // Update stored keys JSON => for HasPrivateKey
            if (mnemonicAddressList.Contains(address))
            {
                mnemonicAddressList.Remove(address);
            }

            await SaveToStorage(HashStorageKey(_storedMnemonicListName), JsonConvert.SerializeObject(mnemonicAddressList));
            _storedMnemonicAdressList = mnemonicAddressList;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        
        return false;
    }

    /// <inheritdoc/>
    public virtual async Task<List<string>> GetStoredMnemonicAdressList()
    {
        if (_storedMnemonicAdressList != null)
        {
            return _storedMnemonicAdressList;
        }

        List<string> mnemonicAddressList = null;

        // Use stored adresses JSON
        var storedKeys = await GetFromStorage(HashStorageKey(_storedMnemonicListName));
        if (!String.IsNullOrWhiteSpace(storedKeys))
        {
            mnemonicAddressList = JsonConvert.DeserializeObject<List<string>>(storedKeys);
        }

        mnemonicAddressList = mnemonicAddressList ?? new List<string>();
        return mnemonicAddressList;
    }

    /// <inheritdoc/>
    public virtual async Task<bool> HasMnemonic(string address = null)
    {
        try
        {
            var mnemonicAddressList = await GetStoredMnemonicAdressList();

            if (!String.IsNullOrWhiteSpace(address))
            {
                return mnemonicAddressList.Contains(address);
            }
            else
            {
                return mnemonicAddressList.Any();
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    /// <inheritdoc/>
    public virtual async Task<(bool removedPrivateKey, bool removedMnemonic)> Remove(string address)
    {
        if (String.IsNullOrWhiteSpace(address))
            throw new ArgumentOutOfRangeException("address is null or empty!");

        var removedPrivateKey = await RemovePrivateKey(address);
        var removedMnemonic = await RemoveMnemonic(address);

        return (removedPrivateKey, removedMnemonic);
    }

    /// <inheritdoc/>
    public async Task<byte[]> GetTxEncryptionKey(string address)
    {
        string base64TxEncryptionKey = await GetFromStorage(HashStorageKey(string.Format(_txEncryptionKeyStoreKeyPattern, address)));
        if (!String.IsNullOrWhiteSpace(base64TxEncryptionKey))
        {
            return Convert.FromBase64String(base64TxEncryptionKey);
        }
        else
        {
            var privateKey = await GetPrivateKey(address);
            if (privateKey != null && privateKey.Length == 32)
            {
                return SecretNET.Crypto.Hashes.DoubleSHA256(privateKey).ToBytes();
            }
        }
        
        return null;
    }

    /// <inheritdoc/>
    public async Task SetTxEncryptionKey(string address, byte[] txEncryptionKey)
    {
        if (String.IsNullOrWhiteSpace(address))
            throw new ArgumentOutOfRangeException("address is null or empty!");

        if (txEncryptionKey.Length < 32)
            throw new ArgumentOutOfRangeException("tx encryption key length should have at least 32 byte!");

        try
        {
            var keyAddressList = await GetStoredKeyAdressList();

            // Check if address is in store
            if (keyAddressList.Contains(address))
            {
                await SaveToStorage(HashStorageKey(string.Format(_txEncryptionKeyStoreKeyPattern, address)), Convert.ToBase64String(txEncryptionKey));
            }
            else
            {
                throw new ArgumentOutOfRangeException($"The adress '{address}' is not in the storage.");
            }            
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    // helper methods 

    private string HashStorageKey(string storageKey)
    {
        return SecretNET.Crypto.Hashes.DoubleSHA256(Encoding.UTF8.GetBytes(storageKey)).ToBytes().ToHexString();
    }
    
}
