using Newtonsoft.Json.Linq;

namespace SecretNET.Api;

/// <summary>
/// Class SmartContractException.
/// Implements the <see cref="System.Exception" />
/// </summary>
/// <seealso cref="System.Exception" />
public class SmartContractException : Exception
{
    /// <summary>
    /// Gets or sets the raw result.
    /// </summary>
    /// <value>The raw result.</value>
    public string RawResult { get; set; }

    /// <summary>
    /// Gets or sets the error result.
    /// </summary>
    /// <value>The error result.</value>
    public JObject ErrorResult { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SmartContractException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="result">The result.</param>
    /// <param name="rawResult">The raw result.</param>
    /// <param name="innerException">The inner exception.</param>
    public SmartContractException(string message, JObject result, string rawResult, Exception innerException)
        :base(message, innerException)
    {
        ErrorResult = result;
        RawResult = rawResult;
    }
}
