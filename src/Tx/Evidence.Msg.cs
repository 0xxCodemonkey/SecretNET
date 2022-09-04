namespace SecretNET.Tx;

/// <summary>
/// MsgSubmitEvidence represents a message that supports submitting arbitrary Evidence of misbehavior such as equivocation or counterfactual signing.
/// </summary>
public class MsgSubmitEvidence : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgSubmitEvidence;

    public string Submitter { get; set; }

    public Any Evidence { get; set; }

    public MsgSubmitEvidence() { }

    public MsgSubmitEvidence(string submitter, Any evidence)
    {
        Submitter = submitter;
        Evidence = evidence;
    }

    public override async Task<IMessage> ToProto(SecretEncryptionUtils utils)
    {
        throw new NotImplementedException("MsgSubmitEvidence ToProto is not implemented.");
    }

    public override async Task<AminoMsg> ToAmino(SecretEncryptionUtils utils)
    {
        throw new NotImplementedException("MsgSubmitEvidence ToAmino is not implemented.");
    }
}
