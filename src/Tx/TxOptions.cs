using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretNET.Tx
{
    /// <summary>
    /// Class TxOptions.
    /// </summary>
    public class TxOptions
    {
        /// <summary>
        /// Defaults to `25_000`.
        /// </summary>
        /// <value>The gas limit.</value>
        public int GasLimit { get; set; } = 25000;

        /// <summary>
        /// E.g. gasPriceInFeeDenom=0.25 & feeDenom="uscrt" => Total fee for tx is `0.25 * gasLimit`uscrt. Defaults to `0.25`.
        /// </summary>
        /// <value>The gas price in fee denom.</value>
        public float GasPriceInFeeDenom { get; set; } = 0.25F;

        /// <summary>
        /// Defaults to "uscrt".
        /// </summary>
        /// <value>The fee denom.</value>
        public string FeeDenom { get; set; } = "uscrt";

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
        public int BroadcastTimeoutMs { get; set; } = 60_000;

        /// <summary>
        /// When waiting for the tx to commit on-chain, how much time (in milliseconds) to wait between checks.
        /// Smaller intervals will cause more load on your node provider. Keep in mind that blocks on Secret Network take about 6 seconds to finilize.
        /// Defaults to `6.000`. Ignored if `waitForCommit = false`.
        /// </summary>
        /// <value>The broadcast check interval ms.</value>
        public int BroadcastCheckIntervalMs { get; set; } = 6_000;

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
        /// explicitSignerData can be used to override `chainId`, `accountNumber` & `accountSequence`.
        /// This is usefull when using {@link BroadcastMode.Async} or when you don't want secretNET to query for `accountNumber` & `accountSequence` from the chain. (smoother in UIs, less load on your node provider).
        /// </summary>
        /// <value>The explicit signer data.</value>
        public SignerData ExplicitSignerData { get; set; }


    }
}
