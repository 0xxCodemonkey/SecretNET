using Newtonsoft.Json.Linq;
using SecretNET.Crypto.BIP32;
using SecretNET.Crypto;

namespace SecretNET;

/// <summary>
/// Class Wallet.
/// Implements the <see cref="SecretNET.IWallet" />
/// </summary>
/// <seealso cref="SecretNET.IWallet" />
public class Wallet : IWallet
{
    /// <summary>
    /// Gets the secure storage provider.
    /// </summary>
    /// <value>The storage provider.</value>
    private IPrivateKeyStorage StorageProvider
    {
        get; set;
    }

    /// <inheritdoc/>
    public WalletSignType WalletSignType { get; private set; } = WalletSignType.DirectSigner;

    /// <summary>
    /// Gets the key derivation path.
    /// </summary>
    /// <value>The key derivation path.</value>
    public string KeyDerivationPath { get; private set; }

    /// <inheritdoc/>
    public string Address { get; private set; }

    /// <inheritdoc/>
    public PubKey PublicKey { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Wallet" /> class.
    /// </summary>
    /// <param name="options">The options.</param>
    private Wallet(CreateWalletOptions? options = null)
    {
        options = options ?? new CreateWalletOptions();
        StorageProvider = options.KeyStorageProvider;
        KeyDerivationPath = options.KeyDerivationPath;
    }

    /// <inheritdoc/>
    public async Task<byte[]> GetTxEncryptionKey()
    {
        return await StorageProvider.GetTxEncryptionKey(Address);
    }

    /// <inheritdoc/>
    public async Task SetTxEncryptionKey(byte[] txEncryptionKey)
    {
        await StorageProvider.SetTxEncryptionKey(Address, txEncryptionKey);
    }

    /// <inheritdoc/>
    public async Task RemoveTxEncryptionKey()
    {
        await StorageProvider.RemoveTxEncryptionKey(Address);
    }

    /// <summary>
    /// Creates the specified mnemonic.
    /// </summary>
    /// <param name="mnemonic">The mnemonic.</param>
    /// <param name="wordlist">The wordlist.</param>
    /// <param name="passphrase">The passphrase.</param>
    /// <param name="options">The options.</param>
    /// <returns>SecretNET.Wallet.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">No valid address</exception>
    public static async Task<Wallet> Create(string mnemonic, Wordlist wordlist = null, string passphrase = null, CreateWalletOptions options = null)
    {
        var fromMnemonic = new Mnemonic(mnemonic, wordlist);
        var seed = fromMnemonic.DeriveSeed(passphrase);

        var wallet = await InitWallet(seed, true, options);

        await wallet.StorageProvider.SaveMnemonic(wallet.Address, mnemonic);

        return wallet;
    }

    /// <summary>
    /// Creates a new random wallet with the specified wordlist.
    /// </summary>
    /// <param name="wordlist">The wordlist.</param>
    /// <param name="passphrase">The passphrase.</param>
    /// <param name="options">The options.</param>
    /// <returns>Wallet.</returns>
    /// <exception cref="Exception">Could not save mnemonic data</exception>
    public static async Task<Wallet> Create(Wordlist wordlist = null, string passphrase = null, CreateWalletOptions options = null)
    {
        wordlist = wordlist ?? Wordlist.English;
        var newMnemonic = new Mnemonic(wordlist);
        var seed = newMnemonic.DeriveSeed(passphrase);

        var wallet = await InitWallet(seed, true, options);

        await wallet.StorageProvider.SaveMnemonic(wallet.Address, newMnemonic.ToString());

        return wallet;
    }

    /// <summary>
    /// Creates the specified seed.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="options">The options.</param>
    /// <returns>Wallet.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">No valid address</exception>
    public static async Task<Wallet> Create(byte[] data, CreateWalletOptions options = null)
    {
        var wallet = await InitWallet(data, false, options);
        return wallet;
    }

    /// <summary>
    /// Initializes the wallet.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="fromSeed">if set to <c>true</c> [from seed].</param>
    /// <param name="options">The options.</param>
    /// <returns>Wallet.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">No valid address</exception>
    /// <exception cref="Exception">Could not save private key data</exception>
    public static async Task<Wallet> InitWallet(byte[] data, bool fromSeed, CreateWalletOptions options = null)
    {
        var wallet = new Wallet(options);

        Key? privateKey = null;
        if (fromSeed)
        {
            var seedKey = ExtKey.CreateFromSeed(data);
            var keyPath = SecretNET.Crypto.BIP32.KeyPath.Parse(wallet.KeyDerivationPath);
            var secretHD = seedKey.Derive(keyPath);

            privateKey = secretHD.PrivateKey;
        }
        else
        {
            privateKey = new Key(data);
        }

        var privateKeyBytes = privateKey.ToBytes();

        var pubkeyBytes = new byte[33];
        privateKey.PubKey.ToBytes(pubkeyBytes, out var l);

        var bech32encoder = Encoders.Bech32("secret");
        var sha256Hash = Hashes.SHA256(pubkeyBytes.ToArray());
        var ripemd160Hash = Hashes.RIPEMD160(sha256Hash, 0, sha256Hash.Length);

        var bech32words = bech32encoder.ConvertBits(ripemd160Hash, 8, 5);
        wallet.Address = bech32encoder.EncodeData(bech32words, Bech32EncodingType.BECH32);

        if (wallet.Address?.Length != 45) // "secret" (6) + 39
        {
            throw new ArgumentOutOfRangeException("No valid address");
        }

        wallet.PublicKey = privateKey.PubKey;

        if (fromSeed)
        {
            await wallet.StorageProvider.SavePrivateKey(wallet.Address, privateKeyBytes);
        }

        return wallet;
    }

    /// <summary>
    /// Gets a subaccount .
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>Wallet.</returns>
    public async Task<Wallet> GetSubaccount(byte index)
    {
        var options = new CreateWalletOptions();
        options.KeyStorageProvider = StorageProvider;
        options.HdAccountIndex = index;

        var privateKey = await StorageProvider.GetPrivateKey(Address);
        var subaccount = await InitWallet(privateKey, true, options);

        return subaccount;
    }

    /// <inheritdoc/>
    public async Task<StdSignature> SignDirect(SignDoc signDoc, string address = null)
    {
        address = address ?? Address;
        var privateKey = await GetPrivateKey(address);

        var serializedSignDoc = signDoc.Encode();
        var signature = SignMessage(privateKey, serializedSignDoc);

        return signature;
    }

    /// <inheritdoc/>
    public async Task<StdSignatureAmino> SignAmino(StdSignDoc signDoc, string address = null)
    {
        address = address ?? Address;
        var privateKey = await GetPrivateKey(address);

        var jObj = (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(signDoc));
        Utils.SortJObject(jObj);
        var sortedJson = jObj.ToString(Formatting.None);
        var serializedSignDoc = Encoding.UTF8.GetBytes(sortedJson);        
        var signature = SignMessage(privateKey, serializedSignDoc);

        var result = new StdSignatureAmino(signature.Signature, PublicKey.ToBytes());

        return result;
    }

    /// <inheritdoc/>
    public async Task<StdSignature> SignMessage(byte[] message, string address = null)
    {
        address = address ?? Address;
        var privateKey = await GetPrivateKey(address);

        return SignMessage(privateKey, message);
    }

    private StdSignature SignMessage(Key privateKey, byte[] message)
    {
        var sha256Hash = Hashes.SHA256(message);
        var messageHash = new uint256(sha256Hash);

        //TODO: Use JS extraEntropy: true => https://github.com/paulmillr/noble-secp256k1
        // extraEntropy: Uint8Array | string | true - additional entropy k' for deterministic signature, follows section 3.6 of RFC6979.
        // When true, it would automatically be filled with 32 bytes of cryptographically secure entropy.
        // **** Strongly recommended to pass true to improve security ******
        // https://www.nuget.org/packages/Secp256k1.ZKP/#readme-body-tab
        // https://stackoverflow.com/questions/72275544/how-do-i-sign-a-transactionhex-with-seedhex-using-secp256k1-in-c-sharp-with-boun
        var ecdsaSignature = privateKey.Sign(messageHash);

        var signatureBytes = ecdsaSignature.ToCompact();
        var signature = SecretNetworkClient.EncodeSecp256k1Signature(PublicKey, signatureBytes);

        return signature;
    }

    private async Task<Key> GetPrivateKey(string address)
    {
        Key privateKey = null;
        var privateKeyBytes = await StorageProvider.GetPrivateKey(address ?? Address);
        if (privateKeyBytes?.Length == 32)
        {
            privateKey = new Key(privateKeyBytes);
        }

        if (address != Address || privateKey == null)
        {
            throw new Exception($"Address ${address} not found in wallet");
        }

        return privateKey;
    }

}
