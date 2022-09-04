namespace SecretNET.Tx;

public class EvidenceTxSimulate
{
    private TxClient _tx;

    internal EvidenceTxSimulate(TxClient tx)
    {
        _tx = tx;
    }

    /// <summary>
    /// Simulates "MsgSubmitEvidence represents a message that supports submitting arbitrary 
    /// Evidence of misbehavior such as equivocation or counterfactual signing."
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> SubmitEvidence(MsgSubmitEvidence msg, TxOptions? txOptions = null)
    {
        return await _tx.Simulate(msg, txOptions);
    }

}
    