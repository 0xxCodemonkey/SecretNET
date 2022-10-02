namespace SecretNET.Common;

/// <summary>
/// User decision for an transaction approval.
/// </summary>
public class UserApprovalDecision
{
    /// <summary>
    /// User decision to approval the transaction.
    /// </summary>
    /// <value><c>true</c> if [approve transaction]; otherwise, <c>false</c>.</value>
    public bool ApproveTransaction { get; set; }

    /// <summary>
    /// User decision of the gas limit for this transaction.
    /// </summary>
    /// <value>The gas limit.</value>
    public ulong GasLimit { get; set; }

    /// <summary>
    /// User decision for an transaction approval.
    /// </summary>
    /// <param name="approveTransaction">User decision to approval the transaction.</param>
    /// <param name="gasLimit">User decision of the gas limit for this transaction.</param>
    public UserApprovalDecision(bool approveTransaction, ulong gasLimit = 0)
    {
        ApproveTransaction = approveTransaction;
        GasLimit = gasLimit;
    }
}
