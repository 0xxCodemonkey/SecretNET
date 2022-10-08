namespace SecretNET.AccessControl;

/// <summary>
/// Class ContractNotInPermitException.
/// Implements the <see cref="System.Exception" />
/// </summary>
/// <seealso cref="System.Exception" />
public class ContractNotInPermitException : Exception
{
    /// <summary>
    /// Gets the contract.
    /// </summary>
    /// <value>The contract.</value>
    public string Contract { get; private set; }

    /// <summary>
    /// Gets the allowed tokens.
    /// </summary>
    /// <value>The allowed tokens.</value>
    public string[] AllowedTokens { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContractNotInPermitException"/> class.
    /// </summary>
    /// <param name="contract">The contract.</param>
    /// <param name="allowedTokens">The allowed tokens.</param>
    public ContractNotInPermitException(string contract, string[] allowedTokens)
    {
        Contract = contract;
        AllowedTokens = allowedTokens;
    }
}
