namespace SecretNET.Crypto;

internal class CompactSignature
{
    internal CompactSignature(int recoveryId, byte[] sig64)
	{
		if (sig64 is null)
			throw new ArgumentNullException(nameof(sig64));
		if (sig64.Length is not 64)
			throw new ArgumentException("sig64 should be 64 bytes", nameof(sig64));
		RecoveryId = recoveryId;
		Signature = sig64;
	}

    /// <summary>
    /// 
    /// </summary>
    internal int RecoveryId { get; }

    /// <summary>
    /// The signature of 64 bytes
    /// </summary>
    internal byte[] Signature { get; }

}
