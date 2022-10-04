namespace SecretNET.Tx;

/// <summary>
/// Enum TxResultCode
/// </summary>
public enum TxResultCode : uint
{
    /// <summary>
    /// Success is returned if the transaction executed successfully
    /// </summary>
    Success = 0,

    /// <summary>
    /// ErrInternal should never be exposed, but we reserve this code for non-specified errors.
    /// </summary>
    ErrInternal = 1,

    /// <summary>
    /// ErrTxDecode is returned if we cannot parse a transaction.
    /// </summary>
    ErrTxDecode = 2,

    /// <summary>
    /// ErrInvalidSequence is used the sequence number (nonce) is incorrect for the signature.
    /// </summary>
    ErrInvalidSequence = 3,

    /// <summary>
    /// ErrUnauthorized is used whenever a request without sufficient authorization is handled.
    /// </summary>
    ErrUnauthorized = 4,

    /// <summary>
    /// ErrInsufficientFunds is used when the account cannot pay requested amount.
    /// </summary>
    ErrInsufficientFunds = 5,

    /// <summary>
    /// ErrUnknownRequest to doc.
    /// </summary>
    ErrUnknownRequest = 6,

    /// <summary>
    /// ErrInvalidAddress to doc.
    /// </summary>
    ErrInvalidAddress = 7,

    /// <summary>
    /// ErrInvalidPubKey to doc.
    /// </summary>
    ErrInvalidPubKey = 8,

    /// <summary>
    /// ErrUnknownAddress to doc.
    /// </summary>
    ErrUnknownAddress = 9,

    /// <summary>
    /// ErrInvalidCoins to doc.
    /// </summary>
    ErrInvalidCoins = 10,

    /// <summary>
    /// ErrOutOfGas to doc.
    /// </summary>
    ErrOutOfGas = 11,

    /// <summary>
    /// ErrMemoTooLarge to doc.
    /// </summary>
    ErrMemoTooLarge = 12,

    /// <summary>
    /// ErrInsufficientFee to doc.
    /// </summary>
    ErrInsufficientFee = 13,

    /// <summary>
    /// ErrTooManySignatures to doc.
    /// </summary>
    ErrTooManySignatures = 14,

    /// <summary>
    /// ErrNoSignatures to doc.
    /// </summary>
    ErrNoSignatures = 15,

    /// <summary>
    /// ErrJSONMarshal defines an ABCI typed JSON marshalling error.
    /// </summary>
    ErrJSONMarshal = 16,

    /// <summary>
    /// ErrJSONUnmarshal defines an ABCI typed JSON unmarshalling error.
    /// </summary>
    ErrJSONUnmarshal = 17,

    /// <summary>
    /// ErrInvalidRequest defines an ABCI typed error where the request contains invalid data.
    /// </summary>
    ErrInvalidRequest = 18,

    /// <summary>
    /// ErrTxInMempoolCache defines an ABCI typed error where a tx already exists in the mempool.
    /// </summary>
    ErrTxInMempoolCache = 19,

    /// <summary>
    /// ErrMempoolIsFull defines an ABCI typed error where the mempool is full.
    /// </summary>
    ErrMempoolIsFull = 20,

    /// <summary>
    /// ErrTxTooLarge defines an ABCI typed error where tx is too large.
    /// </summary>
    ErrTxTooLarge = 21,

    /// <summary>
    /// ErrKeyNotFound defines an error when the key doesn't exist.
    /// </summary>
    ErrKeyNotFound = 22,

    /// <summary>
    /// ErrWrongPassword defines an error when the key password is invalid.
    /// </summary>
    ErrWrongPassword = 23,

    /// <summary>
    /// ErrorInvalidSigner defines an error when the tx intended signer does not match the given signer.
    /// </summary>
    ErrorInvalidSigner = 24,

    /// <summary>
    /// ErrorInvalidGasAdjustment defines an error for an invalid gas adjustment.
    /// </summary>
    ErrorInvalidGasAdjustment = 25,

    /// <summary>
    /// ErrInvalidHeight defines an error for an invalid height.
    /// </summary>
    ErrInvalidHeight = 26,

    /// <summary>
    /// ErrInvalidVersion defines a general error for an invalid version.
    /// </summary>
    ErrInvalidVersion = 27,

    /// <summary>
    /// ErrInvalidChainID defines an error when the chain-id is invalid.
    /// </summary>
    ErrInvalidChainID = 28,

    /// <summary>
    /// ErrInvalidType defines an error an invalid type.
    /// </summary>
    ErrInvalidType = 29,

    /// <summary>
    /// ErrTxTimeoutHeight defines an error for when a tx is rejected out due to an explicitly set timeout height.
    /// </summary>
    ErrTxTimeoutHeight = 30,

    /// <summary>
    /// ErrUnknownExtensionOptions defines an error for unknown extension options.
    /// </summary>
    ErrUnknownExtensionOptions = 31,

    /// <summary>
    /// ErrWrongSequence defines an error where the account sequence defined in the signer info doesn't match the account's actual sequence number.
    /// </summary>
    ErrWrongSequence = 32,

    /// <summary>
    /// ErrPackAny defines an error when packing a protobuf message to Any fails.
    /// </summary>
    ErrPackAny = 33,

    /// <summary>
    /// ErrUnpackAny defines an error when unpacking a protobuf message from Any fails.
    /// </summary>
    ErrUnpackAny = 34,

    /// <summary>
    /// ErrLogic defines an internal logic error, e.g. an invariant or assertion that is violated. It is a programmer error, not a user-facing error.
    /// </summary>
    ErrLogic = 35,

    /// <summary>
    /// ErrConflict defines a conflict error, e.g. when two goroutines try to access the same resource and one of them fails.
    /// </summary>
    ErrConflict = 36,

    /// <summary>
    /// ErrNotSupported is returned when we call a branch of a code which is currently not supported.
    /// </summary>
    ErrNotSupported = 37,

    /// <summary>
    /// ErrNotFound defines an error when requested entity doesn't exist in the state.
    /// </summary>
    ErrNotFound = 38,

    /// <summary>
    /// ErrIO should be used to wrap internal errors caused by external operation. Examples: not DB domain error, file writing etc...
    /// </summary>
    ErrIO = 39,

    /// <summary>
    /// ErrAppConfig defines an error occurred if min-gas-prices field in BaseConfig is empty.
    /// </summary>
    ErrAppConfig = 40,

    /// <summary>
    /// ErrPanic is only set when we recover from a panic, so we know to redact potentially sensitive system info.
    /// </summary>
    ErrPanic = 111222
}
