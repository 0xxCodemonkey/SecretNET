using System.Collections.Concurrent;

namespace SecretNET.Api;

/// <summary>
/// Dictionary which assigns messages index to nonce.
/// </summary>
public class ComputeMsgToNonce : ConcurrentDictionary<int, byte[]>
{

}
