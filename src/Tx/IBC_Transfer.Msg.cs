using Ibc.Core.Client.V1;

namespace SecretNET.Tx;

/// <summary>
/// MsgTransfer defines a msg to transfer fungible tokens (i.e Coins) between
/// ICS20 enabled chains. See ICS Spec here:
/// https://github.com/cosmos/ics/tree/master/spec/ics-020-fungible-token-transfer#data-structures
/// </summary>
public class MsgTransfer : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgTransfer;

    /// <summary>
    /// the port on which the packet will be sent
    /// </summary>
    public string SourcePort { get; set; }

    /// <summary>
    /// the channel by which the packet will be sent
    /// </summary>
    public string SourceChannel { get; set; }

    /// <summary>
    /// the tokens to be transferred
    /// </summary>
    public Coin Token { get; set; }

    /// <summary>
    /// the sender address
    /// </summary>
    public string Sender { get; set; }

    /// <summary>
    /// the recipient address on the destination chain
    /// </summary>
    public string Receiver { get; set; }

    /// <summary>
    /// Timeout height relative to the current block height.
    /// The timeout is disabled when undefined or set to 0.
    /// 
    /// Height is a monotonically increasing data type
    /// that can be compared against another Height for the purposes of updating and
    /// freezing clients.
    /// 
    /// Normally the RevisionHeight is incremented at each height while keeping
    /// RevisionNumber the same. However some consensus algorithms may choose to
    /// reset the height in certain conditions e.g. hard forks, state-machine
    /// breaking changes In these cases, the RevisionNumber is incremented so that
    /// height continues to be monitonically increasing even as the RevisionHeight gets reset
    /// </summary>
    public Height? TimeoutHeight { get; set; }

    /// <summary>
    /// Timeout timestamp (in seconds) since Unix epoch.
    /// The timeout is disabled when undefined or set to 0.
    /// </summary>
    public string? TimeoutTimestampSec { get; set; }

    public MsgTransfer() { }

    public MsgTransfer(string sourcePort, string sourceChannel, Coin token, string sender, string receiver, Height? timeoutHeight = null, string? timeoutTimestampSec = null)
    {
        SourcePort = sourcePort;
        SourceChannel = sourceChannel;
        Token = token;
        Sender = sender;
        Receiver = receiver;
        TimeoutHeight = timeoutHeight;
        TimeoutTimestampSec = timeoutTimestampSec;
    }

    public override async Task<IMessage> ToProto()
    {
        var msgTransfer = new Ibc.Applications.Transfer.V1.MsgTransfer()
        {
            SourcePort = SourcePort,
            SourceChannel = SourceChannel,
            Token = new Cosmos.Base.V1Beta1.Coin() { Amount = Token.Amount, Denom = Token.Denom },
            Sender = Sender,
            Receiver = Receiver,
            TimeoutHeight = TimeoutHeight,
            TimeoutTimestamp = ulong.Parse(!String.IsNullOrEmpty(TimeoutTimestampSec) ? $"{TimeoutTimestampSec}000000000" /* sec -> ns */ : "0")            
        };

        return msgTransfer;
    }

    public override Task<AminoMsg> ToAmino()
    {
        throw new NotImplementedException("MsgTransfer ToAmino is not implemented.");
    }
}





