﻿using SecretNET.Crypto;

namespace SecretNET;

/// <summary>
/// Enum WalletSignType
/// </summary>
public enum WalletSignType
{
    /// <summary>
    /// Default
    /// </summary>
    DirectSigner = 0,

    /// <summary>
    /// Since SecretNET currently not support Ledger, Amino signing is currently only used for permits.
    /// </summary>
    AminoSigner = 1
}

/// <summary>
/// Interface IWallet
/// Implements the <see cref="SecretNET.IAminoSigner" />
/// </summary>
/// <seealso cref="SecretNET.IAminoSigner" />
public interface IWallet : IAminoSigner
{
    /// <summary>
    /// SecretNET currently only support DirectSigner (Amino signing is currently only used for permits).
    /// </summary>
    /// <value>The type of the wallet sign.</value>
    WalletSignType WalletSignType { get; }

    /// <summary>
    /// Gets the tx encryption key from the storage.
    /// </summary>
    /// <value>The key storage.</value>
    Task<byte[]> GetTxEncryptionKey(string address);

    /// <summary>
    /// Gets the address of the wallet.
    /// </summary>
    /// <value>The address.</value>
    string Address { get; }

    /// <summary>
    /// Gets the public key.
    /// </summary>
    /// <value>The public key.</value>
    PubKey PublicKey { get; }

    /// <summary>
    /// Signs the transaction (direct).
    /// </summary>
    /// <param name="signDoc">The sign document.</param>
    /// <param name="address">The address.</param>
    /// <returns>Task&lt;StdSignature&gt;.</returns>
    Task<StdSignature> SignDirect(SignDoc signDoc, string address = null);
}

/// <summary>
/// Interface IAminoSigner
/// </summary>
public interface IAminoSigner
{
    /// <summary>
    /// Signs the transaction (amino).
    /// </summary>
    /// <param name="signDoc">The sign document.</param>
    /// <param name="address">The address.</param>
    /// <returns>Task&lt;StdSignatureAmino&gt;.</returns>
    Task<StdSignatureAmino> SignAmino(StdSignDoc signDoc, string address = null);
}