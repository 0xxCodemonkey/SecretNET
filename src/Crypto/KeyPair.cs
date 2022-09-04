#nullable enable

namespace SecretNET.Crypto;

public class KeyPair
{
	public KeyPair(Key key, IPubKey pubKey)
	{
		if (key is null)
			throw new ArgumentNullException(nameof(key));
		if (pubKey is null)
			throw new ArgumentNullException(nameof(pubKey));
		PubKey = pubKey;
		Key = key;
	}

	public IPubKey PubKey { get; }
	public Key Key { get; }



	public static KeyPair CreateECDSAPair(Key key)
	{
		if (key is null)
			throw new ArgumentNullException(nameof(key));
		return new KeyPair(key, key.PubKey);
	}
}

