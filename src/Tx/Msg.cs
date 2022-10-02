
namespace SecretNET.Tx;

/// <summary>
/// Base class for SecretNET internal message type.
/// </summary>
public abstract class MsgBase
{
    /// <summary>
    /// Gets the type of the message.
    /// </summary>
    /// <value>The type of the message.</value>
    public abstract string MsgType { get; }

    /// <summary>
    /// Gets the GRPC message.
    /// </summary>
    /// <value>The GRPC MSG.</value>
    public IMessage GrpcMsg { get; internal set; }

    /// <summary>
    /// Returns the IMessage / Gprc Message
    /// </summary>
    /// <returns>Task&lt;IMessage&gt;.</returns>
    /// <exception cref="System.MissingMemberException">GrpcMsg is null / not available</exception>
    public abstract Task<IMessage> ToProto();

    /// <summary>
    /// Returns the message in amino format (if available)
    /// </summary>
    /// <returns>Task&lt;AminoMsg&gt;.</returns>
    /// <exception cref="System.MissingMemberException">AminoMsg is null / not available</exception>
    public abstract Task<AminoMsg> ToAmino();

}

/// <summary>
/// Message wrapper for all IMessage / Gprc Messages.
/// Implements the <see cref="SecretNET.Tx.MsgBase" />
/// </summary>
/// <seealso cref="SecretNET.Tx.MsgBase" />
public class Msg : MsgBase
{
    /// <summary>
    /// Gets the type of the message.
    /// </summary>
    /// <value>The type of the message.</value>
    public override string MsgType { get; }

    /// <summary>
    /// Gets or sets the amino message (if available).
    /// </summary>
    /// <value>The amino MSG.</value>
    public AminoMsg AminoMsg { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Msg"/> class.
    /// </summary>
    /// <param name="grpcMsg">The GRPC MSG.</param>
    /// <param name="aminoMsg">The amino MSG.</param>
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

    /// <summary>
    /// Returns the message in amino format (if available)
    /// </summary>
    /// <returns>Task&lt;AminoMsg&gt;.</returns>
    /// <exception cref="System.MissingMemberException">AminoMsg is null / not available</exception>
    public override Task<AminoMsg> ToAmino()
    {
        if (AminoMsg != null)
        {
            return Task.FromResult(AminoMsg);
        }
        throw new MissingMemberException("AminoMsg is null / not available");
    }

    
}