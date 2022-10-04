namespace SecretNET.Tx;

/// <summary>
///TxOptions defaults.
/// </summary>
public static class TxOptionsDefaults
{
    /// <summary>
    /// Defaults GasLimit = `25_000`.
    /// </summary>
    public static ulong GasLimit = 25_000;

    /// <summary>
    /// Defaults GasPriceInFeeDenom = `0.25`.
    /// </summary>
    public static float GasPriceInFeeDenom = 0.25F;

    /// <summary>
    /// Defaults FeeDenom = `uscrt`.
    /// </summary>
    public static string FeeDenom = "uscrt";

    /// <summary>
    /// Defaults BroadcastTimeoutMs = `60_000`.
    /// </summary>
    public static int BroadcastTimeoutMs = 60_000;

    /// <summary>
    /// Defaults BroadcastCheckIntervalMs = 6_000`.
    /// </summary>
    public static int BroadcastCheckIntervalMs = 6_000;
}

/// <summary>
/// Class TxOptions.
/// </summary>
public class TxOptions
{
    /// <summary>
    /// Defaults to `25_000`.
    /// </summary>
    /// <value>The gas limit.</value>
    public ulong GasLimit { get; set; } = TxOptionsDefaults.GasLimit;

    /// <summary>
    /// If AlwaysSimulateTransactions is set on the SecretNetworkClient, the transaction is simulate to get the estimated gas fee.
    /// If SkipSimulate is set in the TxOptions, the simulation is skipped.
    /// </summary>
    /// <value><c>true</c> if [skip simulate]; otherwise, <c>false</c>.</value>
    public bool SkipSimulate { get; set; } = false;

    /// <summary>
    /// E.g. gasPriceInFeeDenom=0.25 and feeDenom="uscrt" => Total fee for tx is `0.25 * gasLimit`uscrt. Defaults to `0.25`.
    /// </summary>
    /// <value>The gas price in fee denom.</value>
    public float GasPriceInFeeDenom { get; set; } = TxOptionsDefaults.GasPriceInFeeDenom;

    /// <summary>
    /// Defaults to "uscrt".
    /// </summary>
    /// <value>The fee denom.</value>
    public string FeeDenom { get; set; } = TxOptionsDefaults.FeeDenom;

    /// <summary>
    /// Address of the fee granter from which to charge gas fees.
    /// </summary>
    /// <value>The fee granter.</value>
    public string FeeGranter { get; set; }

    /// <summary>
    /// Defaults to "".
    /// </summary>
    /// <value>The memo.</value>
    public string Memo { get; set; } = "";

    /// <summary>
    /// If `false` returns immediately with only the `transactionHash` field set. Defaults to `true`.
    /// </summary>
    /// <value><c>true</c> if [wait for commit]; otherwise, <c>false</c>.</value>
    public bool WaitForCommit { get; set; } = true;

    /// <summary>
    /// How much time (in milliseconds) to wait for tx to commit on-chain.
    /// Defaults to `60.000`. Ignored if `waitForCommit = false`.
    /// </summary>
    /// <value>The broadcast timeout ms.</value>
    public int BroadcastTimeoutMs { get; set; } = TxOptionsDefaults.BroadcastTimeoutMs;

    /// <summary>
    /// When waiting for the tx to commit on-chain, how much time (in milliseconds) to wait between checks.
    /// Smaller intervals will cause more load on your node provider. Keep in mind that blocks on Secret Network take about 6 seconds to finilize.
    /// Defaults to `6.000`. Ignored if `waitForCommit = false`.
    /// </summary>
    /// <value>The broadcast check interval ms.</value>
    public int BroadcastCheckIntervalMs { get; set; } = TxOptionsDefaults.BroadcastCheckIntervalMs;

    /// <summary>
    /// If `BroadcastMode.Sync` - Broadcast transaction to mempool and wait for CheckTx response.
    /// @see https://docs.tendermint.com/master/rpc/#/Tx/broadcast_tx_sync
    /// 
    /// If `BroadcastMode.Async` Broadcast transaction to mempool and do not wait for CheckTx response.
    /// @see https://docs.tendermint.com/master/rpc/#/Tx/broadcast_tx_async
    /// </summary>
    /// <value>The broadcast mode.</value>
    public BroadcastMode BroadcastMode { get; set; } = BroadcastMode.Sync;

    /// <summary>
    /// ExplicitSignerData can be used to override `chainId`, `accountNumber` and `accountSequence`.
    /// This is usefull when using BroadcastMode.Async or when you don't want secretNET to query for `accountNumber` and `accountSequence` from the chain. (smoother in UIs, less load on your node provider).
    /// </summary>
    /// <value>The explicit signer data.</value>
    public SignerData ExplicitSignerData { get; set; }


}
