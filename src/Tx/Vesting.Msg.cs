namespace SecretNET.Tx;

/// <summary>
/// RaAuthenticate defines a message to register an new node.
/// </summary>
public class MsgCreateVestingAccount : MsgBase
{
    public override string MsgType { get; } = MsgTypeNames.MsgCreateVestingAccount;

    public string FromAddress { get; set; }

    public string ToAddress { get; set; }

    public Coin[] Amount { get; set; }

    public long EndTime { get; set; }

    public bool Delayed { get; set; }

    public MsgCreateVestingAccount(){}

    public MsgCreateVestingAccount(string fromAddress, string toAddress, Coin[] amount, long endTime, bool delayed)
    {
        FromAddress = fromAddress;
        ToAddress = toAddress;
        Amount = amount;
        EndTime = endTime;
        Delayed = delayed;
    }

    public override async Task<IMessage> ToProto(SecretEncryptionUtils utils)
    {
        var msgUnjail = new Cosmos.Vesting.V1Beta1.MsgCreateVestingAccount()
        {
            FromAddress = FromAddress,
            ToAddress = ToAddress,
            EndTime = EndTime,
            Delayed = Delayed
        };

        if (Amount != null)
        {
            msgUnjail.Amount.Add(Amount.Select(c => new Cosmos.Base.V1Beta1.Coin() { Amount = c.Amount, Denom = c.Denom }).ToArray());
        }       

        return msgUnjail;
    }
    public override async Task<AminoMsg> ToAmino(SecretEncryptionUtils utils)
    {
        throw new NotImplementedException("MsgCreateVestingAccount ToAmino is not implemented.");
    }
}





