using SecretNET.Tx;

namespace SecretNET;

/// <summary>
/// Interface ISecretNetworkClient
/// </summary>
public interface ISecretNetworkClient
{
    /// <summary>
    /// Gets the GRPC web URL.
    /// </summary>
    /// <value>The GRPC web URL.</value>
    public string GrpcWebUrl { get; }

    /// <summary>
    /// Gets the chain identifier.
    /// </summary>
    /// <value>The chain identifier.</value>
    public string ChainId { get; }

    /// <summary>
    /// Gets or sets the wallet.
    /// </summary>
    /// <value>The wallet.</value>
    public IWallet Wallet { get; set; }

    /// <summary>
    /// Gets the address of the attached wallet.
    /// </summary>
    /// <value>The wallet address.</value>
    public string WalletAddress { get; }

    /// <summary>
    /// Gets the encryption utils.
    /// </summary>
    /// <value>The encryption utils.</value>
    public SecretEncryptionUtils EncryptionUtils { get; }

    /// <summary>
    /// Transaction approval callback for a user approval of an transaction.
    /// </summary>
    /// <value>The transaction approval callback.</value>
    public Func<TransactionApprovalData, Task<UserApprovalDecision>> TransactionApprovalCallback { get; }

    /// <summary>
    /// WARNING: On mainnet it's recommended to not simulate every transaction as this can burden your node provider. 
    /// Instead, use this while testing to determine the gas limit for each of your app's transactions (use TxOptions.GasLimit), then in production use hard-coded.
    /// </summary>
    /// <value><c>true</c> if [always simulate transactions]; otherwise, <c>false</c>.</value>
    public bool AlwaysSimulateTransactions { get; }

    /// <summary>
    /// Gas estimation is known to be a bit off, so you might need to adjust it a bit before broadcasting (default is 1.1 / 10%).
    /// </summary>
    /// <value>The gas estimation mltiplier.</value>
    public float GasEstimationMltiplier { get; }

    // Signing

    /// <summary>
    /// Prepares the and signs the specified messages.
    /// </summary>
    /// <param name="messages">The messages.</param>
    /// <param name="txOptions">The tx options.</param>
    /// <returns>Task&lt;System.ValueTuple&lt;System.Byte[], ComputeMsgToNonce&gt;&gt;.</returns>
    public Task<byte[]> PrepareAndSign(Tx.MsgBase[] messages, TxOptions txOptions = null);

    /// <summary>
    /// Signs the specified messages.
    /// </summary>
    /// <param name="messages">The messages.</param>
    /// <param name="fee">The fee.</param>
    /// <param name="memo">The memo.</param>
    /// <param name="explicitSignerData">The explicit signer data.</param>
    /// <returns>Task&lt;System.ValueTuple&lt;TxRaw, ComputeMsgToNonce&gt;&gt;.</returns>
    public Task<TxRaw> Sign(Tx.MsgBase[] messages, StdFee fee, string memo = null, SignerData explicitSignerData = null);

    /// <summary>
    /// Signs the transaction (direct).
    /// </summary>
    /// <param name="messages">The messages.</param>
    /// <param name="fee">The fee.</param>
    /// <param name="signerData">The signer data.</param>
    /// <param name="memo">The memo.</param>
    /// <returns>Task&lt;System.ValueTuple&lt;TxRaw, ComputeMsgToNonce&gt;&gt;.</returns>
    public Task<TxRaw> SignDirect(Tx.MsgBase[] messages, StdFee fee, SignerData signerData, string memo = null);

    /// <summary>
    /// Gets the signer data for the given wallet address.
    /// </summary>
    /// <param name="walletAddress">The wallet address.</param>
    /// <returns>SignerData.</returns>
    public Task<SignerData> GetSignerData(string walletAddress);

}
