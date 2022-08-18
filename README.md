
SecretNET (port of the [secret.js](https://github.com/scrtlabs/secret.js) Client) is an .NET SDK for writing applications that interact with the [Secret Network blockchain](https://scrt.network/).

# Key Features
- Written in .NET 6 including MAUI Support.
- Can be used in MAUI Apps on Android, iOS, Windows and Mac.
- Provides simple abstractions over core data structures.
- Supports every possible message and transaction type.
- Exposes every possible query type.
- Handles input/output encryption/decryption for Secret Contracts.
- The SDK has a wallet built in and does not currently require / support external wallets.
- Custom APIs / clients for specific smart contracts can be easily created (see packages for tokens / SNIP20 or NFT / SNIP721).

:information_source: This library is still in beta (as secret.js), APIs may break. Beta testers are welcome!

## Additional packages
In addition to the SecretNET Core Library, the following complementary packages are available:
- [**Full SNIP-20 (Token) client**](https://github.com/0xxCodemonkey/SecretNET.SNIP20), which exposes all methods of the [SNIP-20 reference implementation](https://github.com/scrtlabs/snip20-reference-impl).
- [**Full SNIP-721 / SNIP-722 (NFT) client**](https://github.com/0xxCodemonkey/SecretNET.SNIP721), which exposes all methods of the [SNIP-721 reference implementation](https://github.com/baedrik/snip721-reference-impl).
- UI Package (coming soon), which contains typical MAUI controls such as Confirm Transaction, or Wallet Setup.

# Table of Contents
- Key Features
  - Additional packages
- Table of Contents
- Usage 
  - Installation
    - Additional packages
- Examples
   - Creating / Initializing the Wallet
      - Importing account from mnemonic phrase
      - Importing private key
      - Import via Keplr QR (coming soon)
      - Generating a random account
      - Attaching the wallet to the SecretNetworkClient (required for signing transactions)
  - Sending Queries
  - Broadcasting Transactions
  - Uploading and initialize Smart Contract
  - Calling a Smart Contract
  - Interacting with an Token Contract (SNIP20)
  - Interacting with an NFT Contract (SNIP721)
- SecretNetworkClient
  - Querier
  - Transactions

# Usage 
## Installation
``` nuget.exe ``` -CLI:
``` bash 
nuget install SecretNET
```
[NuGet-Paket-Manager-Konsole](https://docs.microsoft.com/de-de/nuget/consume-packages/install-use-packages-powershell):
```  bash
Install-Package SecretNET
```

## Additional packages
nuget.exe-CLI:
```  bash
nuget install SecretNET.SNIP20
nuget install SecretNET.SNIP721
nuget install SecretNET.UI (coming soon)
```

NuGet-Paket-Manager-Konsole:
```  bash
Install-Package SecretNET.SNIP20
Install-Package SecretNET.SNIP721
Install-Package SecretNET.UI (coming soon)
```
# Examples
## Creating / Initializing the Wallet
The wallet will use per default an secure storage which protects the sensitive data in a platform-specific manner:

| Plattform | Info |
| ------------- | -------------  |
| Android | Data is encrypted with the Android EncryptedSharedPreference Class, and the secure storage value is encrypted with AES-256 GCM. |
| iOS / macOS | uses the Keychain | 
| Windows | DataProtectionProvider is used to encrypt values securely on Windows devices. | 

:warning: **Never store the private key or mnemonic phrase permanent in a variable (or somewhere else than in SecureStore) or output them in a log!**
**Only store them in a secure storage like the default implementation.**

### Importing account from mnemonic phrase
Simply use the mnemonic phrase in the Wallet.Create method (and use the wallet in the constructor of the SecretNetworkClient:) like this

```  csharp
var wallet = await SecretNET.Wallet.Create("detect unique diary skate horse hockey gain naive approve rabbit act lift");
```
### Importing private key
Simply use the private key in the Wallet.Create method (and use the wallet in the constructor of the SecretNetworkClient:) like this
```  csharp
var walletFromSeed = await SecretNET.Wallet.Create(** byte[] private key **);
```
### Import via Keplr QR (coming soon)
### Generating a random account
To generate a complete new random wallet just use the Wallet.Create method without a parameter (default wordlist is english), or specify another Wordlist.
```  csharp
var newRandomWallet = await SecretNET.Wallet.Create();
```
### Attaching the wallet to the SecretNetworkClient (required for signing transactions)
In the constructor:
```  csharp
var secretNetworkClient = new SecretNetworkClient("https://pulsar-2.api.trivium.network:9091", "pulsar-2", wallet);
```
Later via prop:
```  csharp
secretNetworkClient.Wallet = wallet;
```
## Sending Queries
## Broadcasting Transactions
## Uploading and initialize Smart Contract
## Calling a Smart Contract
## Interacting with an Token Contract (SNIP20)
## Interacting with an NFT Contract (SNIP721)
# SecretNetworkClient
**Full API »**

## Querier
The querier can only send queries and get chain information. Access to all query types can be done via ```SecretNetworkClient.Query```.

## Transactions
Use ```SecretNetworkClient.Tx``` to broadcast transactions.
