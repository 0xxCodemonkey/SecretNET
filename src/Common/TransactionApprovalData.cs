using SecretNET.Tx;
namespace SecretNET.Common;

/// <summary>
/// Infos for usage in the TransactionApproval callback.
/// </summary>
public class TransactionApprovalData
{
    /// <summary>
    /// Gets or sets the signer data.
    /// </summary>
    /// <value>The signer data.</value>
    public SignerData SignerData { get; set; }

    /// <summary>
    /// Estimated gas fee for this transaction (uscrt).
    /// </summary>
    /// <value>The estimated gas fee.</value>
    public ulong EstimatedGasFee { get; set; }

    /// <summary>
    /// Defaults to "uscrt".
    /// </summary>
    /// <value>The fee denom.</value>
    public string FeeDenom { get; set; } = "uscrt";

    /// <summary>
    /// Address of the fee granter from which to charge gas fees.
    /// </summary>
    /// <value>The fee granter.</value>
    public string FeeGranter { get; set; }

    /// <summary>
    /// List of the Messages in the transaction.
    /// </summary>
    /// <value>The messages.</value>
    public ApprovalMsgInfo[] Messages { get; set; }

    /// <summary>
    /// Defaults to "".
    /// </summary>
    /// <value>The memo.</value>
    public string Memo { get; set; } = "";

    /// <summary>
    /// Initializes a new instance of the <see cref="TransactionApprovalData"/> class.
    /// </summary>
    /// <param name="messages">The messages.</param>
    public TransactionApprovalData(MsgBase[] messages)
    {
        var msgList = new List<ApprovalMsgInfo>();
        foreach (MsgBase msg in messages)
        {
            msgList.Add(new ApprovalMsgInfo(msg));
        }
        Messages = msgList.ToArray();
    }
}

/// <summary>
/// Message format for approval. 
/// </summary>
public class ApprovalMsgInfo
{
    /// <summary>
    /// Gets the type of the message.
    /// </summary>
    /// <value>The type of the message.</value>
    public string Type { get; set; }

    /// <summary>
    /// Gets the payload of the message.
    /// </summary>
    /// <value>The payload of the message.</value>
    public object Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApprovalMsgInfo"/> class.
    /// </summary>
    /// <param name="msg">The MSG.</param>
    public ApprovalMsgInfo(MsgBase msg)
    {
        Type = msg.MsgType;
        Value = msg.GrpcMsg;
    }
}
