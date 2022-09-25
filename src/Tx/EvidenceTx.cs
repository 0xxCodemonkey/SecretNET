namespace SecretNET.Tx;

public class EvidenceTx
{
    private TxClient _tx;

    internal EvidenceTx(TxClient tx)
    {
        _tx = tx;
    }

    /// <summary>
    /// MsgSubmitEvidence represents a message that supports submitting arbitrary 
    /// Evidence of misbehavior such as equivocation or counterfactual signing.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SingleSecretTx<Cosmos.Evidence.V1Beta1.MsgSubmitEvidenceResponse>> SubmitEvidence(Cosmos.Evidence.V1Beta1.MsgSubmitEvidence msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return new SingleSecretTx<Cosmos.Evidence.V1Beta1.MsgSubmitEvidenceResponse>(txResult);
    }

}
    