namespace SecretNET.Query;

public class SecretContractInfo
{
    public string Address { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ulong CodeId { get; set; }

    /// <summary>
    /// The address of the creator
    /// </summary>
    public string CreatorAddress { get; set; }

    /// <summary>
    /// bytes admin = 3 [(gogoproto.casttype) = "github.com/cosmos/cosmos-sdk/types.AccAddress"];
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// AbsoluteTxPosition can be used to sort contracts
    /// </summary>
    public AbsoluteTxPosition? Created { get; set; }

}

public class AbsoluteTxPosition
{
    /// <summary>
    /// BlockHeight is the block the contract was created at
    /// </summary>
    public long BlockHeight { get; set; }

    /// <summary>
    /// TxIndex is a monotonic counter within the block (actual transaction index, or gas consumed)
    /// </summary>
    public ulong TxIndex { get; set; }
}
