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
    /// Passing `encryptionSeed` will allow tx decryption at a later time. Ignored if `encryptionUtils` is supplied.
    /// </summary>
    /// <value>The encryption seed.</value>
    public byte[] EncryptionSeed { get; set; }

    /// <summary>
    /// Secret Network encription utils
    /// </summary>
    /// <value>The encryption utils.</value>
    public SecretEncryptionUtils EncryptionUtils { get; set; }

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
        if (String.IsNullOrEmpty(walletAddress) && this.Wallet != null)
        {
            this.WalletAddress = this.Wallet.Address;
        }
        
    }
}
