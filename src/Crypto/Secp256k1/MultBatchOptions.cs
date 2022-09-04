#nullable enable

namespace SecretNET.Crypto.Secp256k1;

public enum ECMultiImplementation
{
    /// <summary>
    /// Pick the optimum algorithm depending on the size of the batch
    /// </summary>
    Auto,
    Pippenger,
    Strauss,
    Simple
}

	public class MultBatchOptions
{
    public MultBatchOptions()
    {

    }
    public MultBatchOptions(ECMultiImplementation implementation)
    {
        Implementation = implementation;
    }
    /// <summary>
    /// The number of scalars until the Auto implementation pick pippenger algorithm over strauss (Default: 88)
    /// </summary>
    public int PippengerThreshold { get; set; } = 88;
    /// <summary>
    /// The implementation to pick
    /// </summary>
    public ECMultiImplementation Implementation { get; set; } = ECMultiImplementation.Auto;
}
#nullable restore
