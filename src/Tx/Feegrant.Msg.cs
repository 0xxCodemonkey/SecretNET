namespace SecretNET.Tx;

/// <summary>
/// MsgGrantAllowance adds permission for Grantee to spend up to Allowance of fees from the account of Granter.
/// </summary>
public class MsgGrantAllowance : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgGrantAllowance;

    /// <summary>
    /// granter is the address of the user granting an allowance of their funds.
    /// </summary>
    public string Granter { get; set; }

    /// <summary>
    /// grantee is the address of the user being granted an allowance of another user's funds.
    /// </summary>
    public string Grantee { get; set; }

    /// <summary>
    /// allowance can be any of basic and filtered fee allowance.
    /// </summary>
    public Any Allowance { get; set; }

    public MsgGrantAllowance() { }

    public MsgGrantAllowance(string granter, string grantee, Any allowance)
    {
        Granter = granter;
        Grantee = grantee;
        Allowance = allowance;
    }


    public override async Task<IMessage> ToProto()
    {
        var msgGrantAllowance = new Cosmos.Feegrant.V1Beta1.MsgGrantAllowance()
        {
            Granter = Granter,
            Grantee = Grantee,
            Allowance = Allowance,
        };

        return msgGrantAllowance;
    }

    public override async Task<AminoMsg> ToAmino()
    {
        var aminoMsg = new AminoMsg("cosmos-sdk/MsgGrantAllowance");
        // order of properties must be sorted for amino signing!!
        aminoMsg.Value = new
        {
            granter = Granter,
            grantee = Grantee,
        };

        return aminoMsg;
    }
}

/// <summary>
/// MsgRevokeAllowance removes any existing Allowance from Granter to Grantee.
/// </summary>
public class MsgRevokeAllowance : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgRevokeAllowance;

    /// <summary>
    /// granter is the address of the user granting an allowance of their funds.
    /// </summary>
    public string Granter { get; set; }

    /// <summary>
    /// grantee is the address of the user being granted an allowance of another user's funds.
    /// </summary>
    public string Grantee { get; set; }

    public MsgRevokeAllowance() { }

    public MsgRevokeAllowance(string granter, string grantee, Any allowance)
    {
        Granter = granter;
        Grantee = grantee;
    }


    public override async Task<IMessage> ToProto()
    {
        var msgRevokeAllowance = new Cosmos.Feegrant.V1Beta1.MsgRevokeAllowance()
        {
            Granter = Granter,
            Grantee = Grantee,
        };

        return msgRevokeAllowance;
    }

    public override async Task<AminoMsg> ToAmino()
    {
        var aminoMsg = new AminoMsg("cosmos-sdk/MsgRevokeAllowance");
        // order of properties must be sorted for amino signing!!
        aminoMsg.Value = new
        {
            granter = Granter,
            grantee = Grantee,
        };

        return aminoMsg;
    }
}