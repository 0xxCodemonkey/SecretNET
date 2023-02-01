using SecretNET.Crypto;
using SecretNET.Crypto.Miscreant;
using SecretNET.Query;
using System.Security.Cryptography;


namespace SecretNET.Common;

/// <summary>
/// SecretEncryptionUtils.
/// </summary>
public partial class SecretEncryptionUtils
{
    private readonly string _chainId;
    private readonly IRegistrationQueryClient _registrationQuerier;

    private List<string> _mainnetChainIds = new List<string>() { "secret-2", "secret-3", "secret-4" };

    private byte[] _hkdfSalt = Convert.FromHexString("000000000000000000024bead8df69990852c202db0e0097c1a12ea637d7e96d");
    private byte[] _mainnetConsensusIoPubKey = Convert.FromHexString("083b1a03661211d5a4cc8d39a77795795862f7730645573b2bcc2c1920c53c04");
    
    private byte[] _seed = null;
    private byte[] _privateKey = null;
    private byte[] _publicKey = null;

    private byte[] _consensusIoPubKey = new byte[0];


    /// <summary>
    /// Initializes a new instance of the <see cref="SecretEncryptionUtils" /> class.
    /// </summary>
    /// <param name="chainId">The chain identifier.</param>
    /// <param name="registrationQuerier">The registration querier.</param>
    /// <param name="seed">The tx encryption key / seed from which the encryption key for encrypting the transactions is generated.</param>
    public SecretEncryptionUtils(string chainId, IRegistrationQueryClient registrationQuerier, byte[] seed = null)
    {
        _chainId = chainId;
        _registrationQuerier = registrationQuerier;

        if (!string.IsNullOrWhiteSpace(_chainId) && _mainnetChainIds.Contains(_chainId))
        {
            // Major speedup
            // TODO: not sure if this is the best approach for detecting mainnet
            _consensusIoPubKey = _mainnetConsensusIoPubKey;
        }

        SetEncryptionSeed(seed);
    }

    /// <summary>
    /// Sets the tx encryption key / seed from which the encryption key for encrypting the transactions is generated.
    /// </summary>
    /// <param name="seed">the tx encryption key / seed</param>
    public void SetEncryptionSeed(byte[] seed = null)
    {
        if (seed == null)
        {
            this._seed = GenerateNewSeed();
        }
        else
        {
            this._seed = seed;
        }

        var generatedKeyPair = GenerateNewKeyPairFromSeed(this._seed);
        _privateKey = generatedKeyPair.PrivateKey;
        _publicKey = generatedKeyPair.PublicKey;        
    }


    /// <summary>
    /// Decrypts the specified ciphertext.
    /// </summary>
    /// <param name="ciphertext">The ciphertext.</param>
    /// <param name="nonce">The nonce.</param>
    /// <returns>System.Byte[].</returns>
    public async Task<byte[]> Decrypt(byte[] ciphertext, byte[] nonce)
    {
        if ((ciphertext?.Length).GetValueOrDefault() == 0)
        {
            return new byte[0];
        }

        var txEncryptionKey = await GetTxEncryptionSeed(nonce);

        var siv = Siv.ImportKey(txEncryptionKey);
        var plaintext = siv.Open(ciphertext);

        return plaintext;
    }

    /// <summary>
    /// Encrypts the specified contract code hash.
    /// </summary>
    /// <param name="contractCodeHash">The contract code hash.</param>
    /// <param name="contractMsg">The contract MSG.</param>
    /// <returns>System.Byte[].</returns>
    public async Task<byte[]> Encrypt(string contractCodeHash, object contractMsg)
    {
        var nonce = GenerateNewSeed();
        var txEncryptionKey = await GetTxEncryptionSeed(nonce);

        var siv = Siv.ImportKey(txEncryptionKey);
        var contractMsgAsString = (contractMsg is string) ? contractMsg : JsonConvert.SerializeObject(contractMsg);

        var plaintext = Encoding.UTF8.GetBytes(contractCodeHash + contractMsgAsString);
        var sealedText = siv.Seal(plaintext);

        var ciphertext = nonce.Concat(_publicKey, sealedText);

        return ciphertext;
    }

    /// <summary>
    /// Gets the tx encryption key.
    /// </summary>
    /// <param name="nonce">The nonce.</param>
    /// <returns>System.Byte[].</returns>
    public async Task<byte[]> GetTxEncryptionSeed(byte[] nonce)
    {
        var consensusIoPubKey = await GetConsensusIoPubKey();
        var sharedKey = Curve25519.GetSharedSecret(_privateKey, consensusIoPubKey);
        var ikmCombined = sharedKey.Concat(nonce);

#if !ANDROID
        var txEncryptionKey = HKDF.DeriveKey(HashAlgorithmName.SHA256, ikmCombined, 32, _hkdfSalt);
#else
        var txEncryptionKey = SecretNET.Crypto.Hkdf.DeriveKey(HashAlgorithmName.SHA256, ikmCombined, 32, _hkdfSalt);
#endif

        return txEncryptionKey;
    }

    /// <summary>
    /// Uses RandomNumberGenerator which is derived from RNGCryptoServiceProvider and is supposed to be secure
    /// https://blogs.siliconorchid.com/post/coding-inspiration/randomness-in-dotnet/
    /// </summary>
    /// <returns>System.Byte[].</returns>
    public static byte[] GenerateNewSeed(int count = 32) {
        return RandomUtils.GetBytes(count);
    }

    /// <summary>
    /// Generates a new key pair from the given 32-byte secret seed (which should be generated with a CSPRNG) and returns it as object.
    /// The returned keys can be used for signing and key agreement.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>System.ValueTuple&lt;System.Byte[], System.Byte[]&gt;.</returns>
    /// <exception cref="System.ArgumentNullException">key</exception>
    /// <exception cref="System.ArgumentException"></exception>
    public static (byte[] PublicKey, byte[] PrivateKey) GenerateNewKeyPairFromSeed(byte[] key)
    {
        if (key == null) throw new ArgumentNullException("key");
        if (key.Length != 32) throw new ArgumentException(String.Format("key must be 32 bytes long (but was {0} bytes long)", key.Length));


        byte[] privateKey = Curve25519.ClampPrivateKey(key);
        byte[] publicKey = Curve25519.GetPublicKey(privateKey);

        return (publicKey, privateKey);
    }

    /// <summary>
    /// Generates the new key pair.
    /// </summary>
    /// <returns>System.ValueTuple&lt;System.Byte[], System.Byte[]&gt;.</returns>
    public static (byte[] PublicKey, byte[] PrivateKey) GenerateNewKeyPair(){
        return GenerateNewKeyPairFromSeed(GenerateNewSeed());
    }

    // private methods

    private async Task<byte[]> GetConsensusIoPubKey() 
    {
        if (_consensusIoPubKey.Length == 32) {
            return _consensusIoPubKey;
        }

        var registrationResult = await _registrationQuerier.TxKey();
        if (registrationResult == null)
        {
            throw new ArgumentNullException("Could not retrieve key used for transactions");
        }

        _consensusIoPubKey = registrationResult.Key_.ToByteArray();

        return _consensusIoPubKey;
    }

    internal class RemoteAttestationPayload
    {
        public string report { get; set; }
        public string signature { get; set; }
        public string signing_cert { get; set; }
    }

    internal class AttestationReportPayload
    {
        public string id { get; set; }
        public DateTime timestamp { get; set; }
        public int version { get; set; }
        public string advisoryURL { get; set; }
        public List<string> advisoryIDs { get; set; }
        public string isvEnclaveQuoteStatus { get; set; }
        public byte[] isvEnclaveQuoteBody { get; set; }
    }

}
