
namespace SecretNET.Tx;

public abstract class MsgBase
{    
    public abstract string MsgType { get; }   

    public abstract Task<IMessage> ToProto(SecretEncryptionUtils utils);

    public abstract Task<AminoMsg> ToAmino(SecretEncryptionUtils utils);

}
