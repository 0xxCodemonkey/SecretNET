using SecretNET.Crypto.BIP32;

namespace SecretNET.Crypto;

/// <summary>
/// A IHDScriptPubKey represent an object which represent a tree of scriptPubKeys
/// </summary>
internal interface IHDScriptPubKey
{
    IHDScriptPubKey Derive(KeyPath keyPath);
    bool CanDeriveHardenedPath();
}
