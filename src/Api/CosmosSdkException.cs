namespace SecretNET.Api;

/// <summary>
/// Class CosmosSdkException.
/// Implements the <see cref="System.Exception" />
/// </summary>
/// <seealso cref="System.Exception" />
public class CosmosSdkException : Exception
{
    /// <summary>
    /// Gets the cosmos SDK error enum.
    /// </summary>
    /// <value>The cosmos SDK error enum.</value>
    public CosmosSdkErrorEnum CosmosSdkErrorEnum { get; private set; }

    /// <summary>
    /// Gets the broadcast tx response.
    /// </summary>
    /// <value>The broadcast tx response.</value>
    public BroadcastTxResponse BroadcastTxResponse { get; private set; }

    /// <summary>
    /// Gets the get tx response.
    /// </summary>
    /// <value>The get tx response.</value>
    public GetTxResponse GetTxResponse { get; private set; }

    /// <summary>
    /// Gets the raw log.
    /// </summary>
    /// <value>The raw log.</value>
    public string RawLog 
    { 
        get {
            if (BroadcastTxResponse?.TxResponse?.RawLog != null)
            {
                return BroadcastTxResponse?.TxResponse?.RawLog;
            }
            if (GetTxResponse?.TxResponse?.RawLog != null)
            {
                return GetTxResponse?.TxResponse?.RawLog;
            }
            return null;
        } 
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosSdkException"/> class.
    /// </summary>
    /// <param name="cosmosSdkErrorEnum">The cosmos SDK error enum.</param>
    /// <param name="response">The response.</param>
    public CosmosSdkException(CosmosSdkErrorEnum cosmosSdkErrorEnum, BroadcastTxResponse response) :base(SetErrorMessage(cosmosSdkErrorEnum))
    {
        CosmosSdkErrorEnum = cosmosSdkErrorEnum;
        BroadcastTxResponse = response;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosSdkException"/> class.
    /// </summary>
    /// <param name="cosmosSdkErrorEnum">The cosmos SDK error enum.</param>
    /// <param name="response">The response.</param>
    public CosmosSdkException(CosmosSdkErrorEnum cosmosSdkErrorEnum, GetTxResponse response) : base(SetErrorMessage(cosmosSdkErrorEnum))
    {
        CosmosSdkErrorEnum = cosmosSdkErrorEnum;
        GetTxResponse = response;
    }

    // https://github.com/cosmos/cosmos-sdk/blob/main/types/errors/errors.go

    private static string SetErrorMessage(CosmosSdkErrorEnum cosmosSdkErrorEnum)
    {
        switch (cosmosSdkErrorEnum)
        {
            case CosmosSdkErrorEnum.ErrTxDecode: return "ErrTxDecode is returned if we cannot parse a transaction";
            case CosmosSdkErrorEnum.ErrInvalidSequence: return "ErrInvalidSequence is used the sequence number (nonce) is incorrect for the signature";
            case CosmosSdkErrorEnum.ErrUnauthorized: return "ErrUnauthorized is used whenever a request without sufficient authorization is handled.";
            case CosmosSdkErrorEnum.ErrInsufficientFunds: return "ErrInsufficientFunds is used when the account cannot pay requested amount.";
            case CosmosSdkErrorEnum.ErrUnknownRequest: return "ErrUnknownRequest to doc";
            case CosmosSdkErrorEnum.ErrInvalidAddress: return "ErrInvalidAddress to doc";
            case CosmosSdkErrorEnum.ErrInvalidPubKey: return "ErrInvalidPubKey to doc";
            case CosmosSdkErrorEnum.ErrUnknownAddress: return "ErrUnknownAddress to doc";
            case CosmosSdkErrorEnum.ErrInvalidCoins: return "ErrInvalidCoins to doc";
            case CosmosSdkErrorEnum.ErrOutOfGas: return "ErrOutOfGas to doc";
            case CosmosSdkErrorEnum.ErrMemoTooLarge: return "ErrMemoTooLarge to doc";
            case CosmosSdkErrorEnum.ErrInsufficientFee: return "ErrInsufficientFee to doc";
            case CosmosSdkErrorEnum.ErrTooManySignatures: return "ErrTooManySignatures to doc";
            case CosmosSdkErrorEnum.ErrNoSignatures: return "ErrNoSignatures to doc";
            case CosmosSdkErrorEnum.ErrJSONMarshal: return "ErrJSONMarshal defines an ABCI typed JSON marshalling error";
            case CosmosSdkErrorEnum.ErrJSONUnmarshal: return "ErrJSONUnmarshal defines an ABCI typed JSON unmarshalling error";
            case CosmosSdkErrorEnum.ErrInvalidRequest: return "ErrInvalidRequest defines an ABCI typed error where the request contains invalid data.";
            case CosmosSdkErrorEnum.ErrTxInMempoolCache: return "ErrTxInMempoolCache defines an ABCI typed error where a tx already exists in the mempool.";
            case CosmosSdkErrorEnum.ErrMempoolIsFull: return "ErrMempoolIsFull defines an ABCI typed error where the mempool is full.";
            case CosmosSdkErrorEnum.ErrTxTooLarge: return "ErrTxTooLarge defines an ABCI typed error where tx is too large.";
            case CosmosSdkErrorEnum.ErrKeyNotFound: return "ErrKeyNotFound defines an error when the key doesn't exist";
            case CosmosSdkErrorEnum.ErrWrongPassword: return "ErrWrongPassword defines an error when the key password is invalid.";
            case CosmosSdkErrorEnum.ErrorInvalidSigner: return "ErrorInvalidSigner defines an error when the tx intended signer does not match the given signer.";
            case CosmosSdkErrorEnum.ErrorInvalidGasAdjustment: return "ErrorInvalidGasAdjustment defines an error for an invalid gas adjustment.";
            case CosmosSdkErrorEnum.ErrInvalidHeight: return "ErrInvalidHeight defines an error for an invalid height.";
            case CosmosSdkErrorEnum.ErrInvalidVersion: return "ErrInvalidVersion defines a general error for an invalid version.";
            case CosmosSdkErrorEnum.ErrInvalidChainID: return "ErrInvalidChainID defines an error when the chain-id is invalid.";
            case CosmosSdkErrorEnum.ErrInvalidType: return "ErrInvalidType defines an error an invalid type.";
            case CosmosSdkErrorEnum.ErrTxTimeoutHeight: return "ErrTxTimeoutHeight defines an error for when a tx is rejected out due to an explicitly set timeout height.";
            case CosmosSdkErrorEnum.ErrUnknownExtensionOptions: return "ErrUnknownExtensionOptions defines an error for unknown extension options.";
            case CosmosSdkErrorEnum.ErrWrongSequence: return "ErrWrongSequence defines an error where the account sequence defined in the signer info doesn't match the account's actual sequence number.";
            case CosmosSdkErrorEnum.ErrUnpackAny: return "ErrPackAny defines an error when packing a protobuf message to Any fails.";
            case CosmosSdkErrorEnum.ErrLogic: return "ErrUnpackAny defines an error when unpacking a protobuf message from Any fails.";
            case CosmosSdkErrorEnum.ErrConflict: return "ErrLogic defines an internal logic error, e.g. an invariant or assertion that is violated. It is a programmer error, not a user-facing error.";
            case CosmosSdkErrorEnum.ErrNotSupported: return "ErrConflict defines a conflict error, e.g. when two goroutines try to access the same resource and one of them fails.";
            case CosmosSdkErrorEnum.ErrNotFound: return "ErrNotSupported is returned when we call a branch of a code which is currently not supported.";
            case CosmosSdkErrorEnum.ErrIO: return "ErrIO should be used to wrap internal errors caused by external operation. Examples: not DB domain error, file writing etc...";
            case CosmosSdkErrorEnum.ErrAppConfig: return "ErrAppConfig defines an error occurred if min-gas-prices field in BaseConfig is empty.";
            default: return "Unknown Error";
        }
    }


}
