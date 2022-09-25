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

}
