namespace SecretNET.Tx;

public class EvidenceTx
{
    private TxClient _tx;
    private EvidenceTxSimulate _txSimulte;

    internal EvidenceTx(TxClient tx)
    {
        _tx = tx;
        _txSimulte = new EvidenceTxSimulate(tx);
    }

    public EvidenceTxSimulate Simulate
    {
        get { return _txSimulte; }
    }

    /// <summary>
    /// MsgSubmitEvidence represents a message that supports submitting arbitrary 
    /// Evidence of misbehavior such as equivocation or counterfactual signing.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SecretTx> SubmitEvidence(MsgSubmitEvidence msg, TxOptions? txOptions = null)
    {
        return await _tx.Broadcast(msg, txOptions);
    }

}
    