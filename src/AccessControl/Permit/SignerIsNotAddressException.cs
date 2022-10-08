namespace SecretNET.AccessControl;

/// <summary>
/// Class SignerIsNotAddressException.
/// Implements the <see cref="Exception" />
/// </summary>
/// <seealso cref="Exception" />
public class SignerIsNotAddressException : Exception
{

    /// <summary>
    /// Gets the pub key.
    /// </summary>
    /// <value>The pub key.</value>
    public Pubkey PubKey { get; private set; }

    /// <summary>
    /// Gets the address.
    /// </summary>
    /// <value>The address.</value>
    public string Address { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SignerIsNotAddressException"/> class.
    /// </summary>
    /// <param name="pubKey">The pub key.</param>
    /// <param name="address">The address.</param>
    public SignerIsNotAddressException(Pubkey pubKey, string address)
    {
        PubKey = pubKey;
        Address = address;
    }
}
