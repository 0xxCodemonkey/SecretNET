namespace SecretNET.Common;

/// <summary>
/// Class CreateClientOptions.
/// </summary>
public class CreateClientOptions
{
    /// <summary>
    /// A gRPC-web url, by default on port 9091
    /// </summary>
    /// <value>The GRPC web URL.</value>
    public string GrpcWebUrl { get; private set; }

    /// <summary>
    /// The chain-id is used in encryption code and when signing txs.
    /// </summary>
    /// <value>The chain identifier.</value>
    public string ChainId { get; private set; }

    /// <summary>
    /// A wallet for signing transactions and permits. When `wallet` is supplied, `walletAddress` and `chainId` must be supplied too.
    /// </summary>
    /// <value>The wallet.</value>
    public Wallet Wallet { get; set; }

    /// <summary>
    /// WalletAddress is the specific account address in the wallet that is permitted to sign transactions and permits.
    /// </summary>
    /// <value>The wallet address.</value>
    public string WalletAddress { get; set; }

    /// <summary>
    /// The tx encryption key / seed is used to encrypt transactions and will allow tx decryption at a later time.
    /// If no value is set (and no EncryptionUtils) a new tx encryption key / seed will be generated (Keplr style) by signing a message with the private key and deriving the key / seed from it..
    /// If EncryptionUtils are manually provided this value gets ignored.
    /// </summary>
    /// <value>The encryption seed.</value>
    public byte[] EncryptionSeed { get; set; }

    /// <summary>
    /// Secret Network encription utils which should be created with a privatly known tx encryption key / seed which used to encrypt transactions and will allow tx decryption at a later time.
    /// </summary>
    /// <value>The encryption utils.</value>
    public SecretEncryptionUtils EncryptionUtils { get; set; }

    /// <summary>
    /// Transaction approval callback for a user approval of an transaction.
    /// </summary>
    /// <value>The transaction approval callback.</value>
    public Func<TransactionApprovalData, Task<UserApprovalDecision>> TransactionApprovalCallback { get; set; }

    /// <summary>
    /// WARNING: On mainnet it's recommended to not simulate every transaction as this can burden your node provider. 
    /// Instead, use this while testing to determine the gas limit for each of your app's transactions (use TxOptions.GasLimit), then in production use hard-coded.
    /// </summary>
    /// <value><c>true</c> if [always simulate transactions]; otherwise, <c>false</c>.</value>
    public bool AlwaysSimulateTransactions { get; set; }

    /// <summary>
    /// Gas estimation is known to be a bit off, so you might need to adjust it a bit before broadcasting (default is 1.1 / 10%).
    /// </summary>
    /// <value>The gas estimation mltiplier.</value>
    public float GasEstimationMultiplier { get; set; } = 1.1f;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateClientOptions" /> class.
    /// </summary>
    /// <param name="grpcWebUrl">The GRPC web URL.</param>
    /// <param name="chainId">The chain identifier.</param>
    /// <param name="wallet">The wallet.</param>
    /// <param name="walletAddress">The wallet address.</param>
    public CreateClientOptions(string grpcWebUrl, string chainId, Wallet wallet = null, string walletAddress = null)
    {
        this.GrpcWebUrl = grpcWebUrl;
        this.ChainId = chainId;
        this.Wallet = wallet;
        this.WalletAddress = walletAddress;
        if (string.IsNullOrWhiteSpace(walletAddress) && this.Wallet != null)
        {
            this.WalletAddress = this.Wallet.Address;
        }
        
    }
}
