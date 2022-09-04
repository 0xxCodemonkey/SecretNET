namespace SecretNET.Api;

/// <summary>
/// Interface ISignerData
/// @see https://github.com/cosmos/cosmos-sdk/blob/v0.42.2/x/auth/signing/sign_mode_handler.go#L23-L37
/// </summary>
public interface ISignerData
{
    /// <summary>
    /// ChainID is the chain that this transaction is targeted
    /// </summary>
    /// <value>The chain identifier.</value>
    public string? ChainID { get; set; }

    /// <summary>
    /// AccountNumber is the account number of the signer.
    /// </summary>
    /// <value>The account number.</value>
    public ulong AccountNumber { get; set; }

    /// <summary>
    /// Sequence is the account sequence number of the signer that is used for replay protection. 
    /// This field is only useful for Legacy Amino signing, since in SIGN_MODE_DIRECT the account sequence is already in the signer info.
    /// </summary>
    /// <value>The sequence.</value>
    public ulong Sequence { get; set; }

}

/// <summary>
/// Class SignerData
/// @see https://github.com/cosmos/cosmos-sdk/blob/v0.42.2/x/auth/signing/sign_mode_handler.go#L23-L37
/// </summary>
/// <seealso cref="SecretNET.Api.ISignerData" />
public class SignerData : ISignerData
{
    /// <inheritdoc />
    public string? ChainID { get; set; }

    /// <inheritdoc />
    public ulong AccountNumber { get; set; }

    /// <inheritdoc />
    public ulong Sequence { get; set; }
}
