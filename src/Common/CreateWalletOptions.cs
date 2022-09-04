namespace SecretNET.Common;

/// <summary>
/// Options for the wallet creation
/// </summary>
public class CreateWalletOptions
{
    /// <summary>
    /// This should not be changed and secret network has the number 529 (see https://github.com/satoshilabs/slips/blob/master/slip-0044.md)
    /// </summary>
    /// <value>The type of the coin.</value>
    public int CoinType { get; set; } = 529;

    /// <summary>
    /// Gets or sets the account number (default = 0)
    /// </summary>
    /// <value>The index of the hd account.</value>
    public byte HdAccountIndex { get; set; } = 0;

    /// <summary>
    /// Gets or sets the key storage provider.
    /// </summary>
    /// <value>The key storage provider.</value>
    public IPrivateKeyStorage KeyStorageProvider { get; set; } = new MauiSecureStorage();

    /// <summary>
    /// Gets the key derivation path ($"m/44'/{CoinType}'/0'/0/{HdAccountIndex}").
    /// </summary>
    /// <value>The key derivation path.</value>
    public string KeyDerivationPath { 
        get {
            return $"m/44'/{CoinType}'/0'/0/{HdAccountIndex}";
        }
    }

    public CreateWalletOptions(IPrivateKeyStorage? keyStorageProvider = null)
    {
        if (keyStorageProvider != null)
        {
            KeyStorageProvider = keyStorageProvider;
        }
    }
}


