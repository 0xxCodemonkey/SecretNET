using SecretNET.Crypto;

namespace SecretNET.Crypto.BIP32;

internal interface IHDKey
{
	IHDKey Derive(KeyPath keyPath);
	PubKey GetPublicKey();
	bool CanDeriveHardenedPath();
}
