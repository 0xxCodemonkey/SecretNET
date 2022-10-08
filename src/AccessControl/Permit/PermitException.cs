namespace SecretNET.AccessControl;

/// <summary>
/// Class PermitException.
/// Implements the <see cref="Exception" />
/// </summary>
/// <seealso cref="Exception" />
public class PermitException : Exception
{
	/// <summary>
	/// Initializes a new instance of the <see cref="PermitException"/> class.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public PermitException(string message):base(message)
	{

	}
}
