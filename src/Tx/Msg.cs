
namespace SecretNET.Tx;

public abstract class MsgBase
{    
    public abstract string MsgType { get; }   

    public abstract Task<IMessage> ToProto();

    public abstract Task<AminoMsg> ToAmino();

}

/// <summary>
/// Message wrapper for all IMessage / Gprc Messages.
/// Implements the <see cref="SecretNET.Tx.MsgBase" />
/// </summary>
/// <seealso cref="SecretNET.Tx.MsgBase" />
public class Msg : MsgBase
{
    public override string MsgType { get; }

    public IMessage GrpcMsg { get; set; }

    public AminoMsg AminoMsg { get; set; }


    public Msg(IMessage grpcMsg, AminoMsg aminoMsg = null)
    {
        GrpcMsg = grpcMsg;
        AminoMsg = aminoMsg;
        if (GrpcMsg != null)
        {
            MsgType = grpcMsg?.Descriptor?.FullName;
        }
    }

    /// <summary>
    /// Returns the IMessage / Gprc Message
    /// </summary>
    /// <returns>Task&lt;IMessage&gt;.</returns>
    /// <exception cref="System.MissingMemberException">GrpcMsg is null / not available</exception>
    public override Task<IMessage> ToProto()
    {
        if (GrpcMsg != null)
        {
            return Task.FromResult(GrpcMsg);
        }
        throw new MissingMemberException("GrpcMsg is null / not available");
    }

    public override Task<AminoMsg> ToAmino()
    {
        if (AminoMsg != null)
        {
            return Task.FromResult(AminoMsg);
        }
        throw new MissingMemberException("AminoMsg is null / not available");
    }

    
}