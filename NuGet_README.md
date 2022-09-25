# Secret.NET Core Library
Secret.NET (port of the [secret.js](https://github.com/scrtlabs/secret.js) Client) is a .NET Client to interact with the [Secret Network blockchain](https://scrt.network/) (L1 / Cosmos based), the first privacy smart contract blockchain that processes and stores data on-chain in encrypted form (SGX). 
This allows [unique use cases](https://docs.scrt.network/secret-network-documentation/secret-network-overview/use-cases) like Secret NFTs where you can store public and private data e.g., Encryption Keys, passwords or other secrets. 

# Key Features
- Written in .NET 6 including MAUI Support.
- Can be used in MAUI Apps on Android, iOS, Windows and Mac.
- Provides simple abstractions over core data structures.
- Supports every possible message and transaction type.
- Exposes every possible query type.
- Handles input/output encryption/decryption for Secret Contracts.
- The SDK has a wallet built in and does not currently require / support external wallets.
- Custom APIs / clients for specific smart contracts can be easily created (see packages for tokens / SNIP20 or NFT / SNIP721).

All information and documentation is available in the [**GitHub repository**](https://github.com/0xxCodemonkey/SecretNET).

:information_source: This library is still in beta (as [secret.js](https://github.com/scrtlabs/secret.js)), APIs may break. Beta testers are welcome!

## Additional packages
In addition to the Secret.NET Core Library, the following complementary packages are available:
- [**Full Token (SNIP-20) client**](https://github.com/0xxCodemonkey/SecretNET.Token), which exposes all methods of the [SNIP-20 reference implementation](https://github.com/scrtlabs/snip20-reference-impl).
- [**Full NFT (SNIP-721 / SNIP-722) client**](https://github.com/0xxCodemonkey/SecretNET.NFT), which exposes all methods of the [SNIP-721 reference implementation](https://github.com/baedrik/snip721-reference-impl).

## Links
- [GitHub](https://github.com/0xxCodemonkey/SecretNET)
- [Secret Network - Secret Network is the first blockchain with customizable privacy.](https://scrt.network/)
- [Secret Network - Documentation](https://docs.scrt.network/secret-network-documentation/)