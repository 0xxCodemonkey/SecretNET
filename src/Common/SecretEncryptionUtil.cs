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
    
    private readonly byte[] _seed = null;
    private readonly byte[] _privateKey = null;
    private readonly byte[] _publicKey = null;

    private byte[] _consensusIoPubKey = new byte[0];


    /// <summary>
    /// Initializes a new instance of the <see cref="SecretEncryptionUtils" /> class.
    /// </summary>
    /// <param name="chainId">The chain identifier.</param>
    /// <param name="registrationQuerier">The registration querier.</param>
    /// <param name="seed">The seed.</param>
    public SecretEncryptionUtils(string chainId, IRegistrationQueryClient registrationQuerier, byte[] seed = null)
    {
        _chainId = chainId;
        _registrationQuerier = registrationQuerier;

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

        if (!string.IsNullOrWhiteSpace(_chainId) && _mainnetChainIds.Contains(_chainId))
        {
            // Major speedup
            // TODO: not sure if this is the best approach for detecting mainnet
            _consensusIoPubKey = _mainnetConsensusIoPubKey;
        }
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

        var txEncryptionKey = await GetTxEncryptionKey(nonce);

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
        var txEncryptionKey = await GetTxEncryptionKey(nonce);

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
    public async Task<byte[]> GetTxEncryptionKey(byte[] nonce)
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

        _consensusIoPubKey = ExtractPubkey(registrationResult.Key_.ToByteArray());

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

    /// <summary>
    /// extractPubkey ported from https://github.com/enigmampc/SecretNetwork/blob/8ab20a273570bfb3d55d67e0300ecbdc67e0e739/x/registration/remote_attestation/remote_attestation.go#L25
    /// </summary>
    /// <param name="cert">The cert.</param>
    /// <returns>System.Byte[].</returns>
    private byte[]? ExtractPubkey(byte[] cert)
    {
        var nsCmtOid = new byte[] { 0x06, 0x09, 0x60, 0x86, 0x48, 0x01, 0x86, 0xf8, 0x42, 0x01, 0x0d}; // Netscape Comment OID

        var payload = ExtractAsn1Value(cert, nsCmtOid);

        try
        {
            // Try HW mode
            // Ported from https://github.com/scrtlabs/SecretNetwork/blob/8ab20a273570bfb3d55d67e0300ecbdc67e0e739/x/registration/remote_attestation/remote_attestation.go#L110
            var jsonPayloadString = Encoding.UTF8.GetString(payload);
            var jsonPayload = JsonConvert.DeserializeObject<RemoteAttestationPayload>(jsonPayloadString);

            if (jsonPayload != null && jsonPayload.report != null)
            {
                var palyoadBase64Bytes = Convert.FromBase64String(Convert.ToString(jsonPayload.report));
                var payloadString = Encoding.UTF8.GetString(palyoadBase64Bytes);
                var payloadJson = JsonConvert.DeserializeObject<AttestationReportPayload>(payloadString);
                if (payloadJson!= null && payloadJson.isvEnclaveQuoteBody != null)
                {
                    var quoteHex = (byte[])payloadJson.isvEnclaveQuoteBody;
                    var reportData = new ArraySegment<byte>(quoteHex, 368, 32).ToArray();
                    return reportData;
                }
            }
        }
        catch (Exception ex) 
        {
            //throw new Exception("Cannot extract tx io pubkey: error parsing certificate - malformed certificate");
        }

        try
        {
            // Try SW mode
            var payloadString = Encoding.UTF8.GetString(payload);
            var pubkey = Convert.FromBase64String(payloadString);
            if (pubkey.Length == 32)
            {
                return pubkey;
            }
        }
        catch
        {
            // Not SW mode
            throw new Exception("Cannot extract tx io pubkey: error parsing certificate - malformed certificate");
        }

        return null;
    }

    private byte[] ExtractAsn1Value(byte[] cert, byte[] oid)
    {
        var offset = Convert.ToHexString(cert).IndexOf(Convert.ToHexString(oid)) / 2;

        offset += 12; // 11 + TAG (0x04)

        // we will be accessing offset + 2, so make sure it's not out-of-bounds
        if (offset + 2 >= cert.Length)
        {
            throw new Exception("Error parsing certificate - malformed certificate");
        }

        int length = cert[offset];
        if (length > 0x80)
        {
            length = (cert[offset + 1] * 0x100) + cert[offset + 2];
            offset += 2;
        }

        if (offset + length + 1 >= cert.Length)
        {
            throw new Exception("Error parsing certificate - malformed certificate");
        }

        // Obtain Netscape Comment
        offset += 1;
        var payload = new ArraySegment<byte>(cert, offset, length);

        return payload.ToArray();
    }
}
