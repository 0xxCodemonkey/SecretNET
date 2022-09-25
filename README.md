
# Secret.NET Core Library
**Secret.NET** (port of the [secret.js](https://github.com/scrtlabs/secret.js) Client) is a .NET Client to interact with the [Secret Network blockchain](https://scrt.network/) (L1 / Cosmos based), the first privacy smart contract blockchain that processes and stores data on-chain in encrypted form (SGX). 

This allows [unique use cases](https://docs.scrt.network/secret-network-documentation/secret-network-overview/use-cases) like **Secret NFTs where you can store public and private data** e.g., encryption keys, passwords or other secrets. 

# Key Features
- Written in .NET 6 including MAUI Support.
- Can be used in MAUI Apps on Android, iOS, Windows and Mac.
- Provides simple abstractions over core data structures.
- Supports every possible message and transaction type.
- Exposes every possible query type.
- Handles input/output encryption/decryption for Secret Contracts.
- The SDK has a wallet built in and does not currently require / support external wallets.
- Custom APIs / clients for specific smart contracts can be easily created (see packages for tokens / SNIP20 or NFT / SNIP721).

:information_source: This library is still in beta (as [secret.js](https://github.com/scrtlabs/secret.js)), APIs may break. Beta testers are welcome!

## Additional packages
In addition to the Secret.NET Core Library, the following complementary packages are available:
- [**Full SNIP-20 (Token) client**](https://github.com/0xxCodemonkey/SecretNET.SNIP20), which exposes all methods of the [SNIP-20 reference implementation](https://github.com/scrtlabs/snip20-reference-impl).
- [**Full SNIP-721 / SNIP-722 (NFT) client**](https://github.com/0xxCodemonkey/SecretNET.SNIP721), which exposes all methods of the [SNIP-721 reference implementation](https://github.com/baedrik/snip721-reference-impl).
- **UI Package** (coming soon), which contains typical MAUI controls such as Confirm Transaction, Wallet Setup, Scan Keplr QR, etc. 

## Full API-documentation
You can find the **full API-documentation** here => [https://0xxcodemonkey.github.io/SecretNET](https://0xxcodemonkey.github.io/SecretNET/html/T-SecretNET.SecretNetworkClient.htm)

# Table of Contents
- [Key Features](#key-features)
  - [Additional packages](#additional-packages)
  - [Full API-documentation](#full-api-documentation)
- [General information](#general-information)
- [Installation](#installation)
  - [Additional packages](#additional-packages)
- [Usage Examples](#usage-examples)
	- [Sending Queries](#sending-queries)
	- [Broadcasting Transactions](#broadcasting-transactions)
- [API](#api)
  - [Creating / Initializing the Wallet](#creating--initializing-the-wallet)
  	- [Importing account from mnemonic phrase](#importing-account-from-mnemonic-phrase)
	- [Importing private key](#importing-private-key)
	- [Use previously saved wallet](#use-previously-saved-wallet)
	- Import via Keplr QR (coming soon...)
	- [Generating a new account](#generating-a-new-account)
	- [Attaching the wallet to the SecretNetworkClient (required for signing transactions)](#attaching-the-wallet-to-the-secretnetworkclient-required-for-signing-transactions)
  - [SecretNetworkClient](#secretnetworkclient)
	- [Queries](#querier-secretclientquery)
	- [Transactions](#transactions)
  - [Overview of all query and transaction methods](#overview-of-all-query-and-transaction-methods)
  	- [All query methods](#all-query-methods)
  	- [All transaction methods](#all-transaction-methods)

# General information
The rough structure of the Secret.NET client, from the user's perspective, is divided into the following areas:
- **Queries** (accessible via ```Query``` property), to get informations from the blockchain or an smart contract.
- **Transactions** (accessible via ```Tx``` property), to interact activly with the blockchain e.g. sending funds or calling smart contract methods.
- **Permits** (accessible via ```Permit``` property) for requesting protected informations in a query, without the need for a separate transaction.

**Queries** cost no fees, are executed "immediately" and do not require a wallet.

**Transactions** are broadcast to the blockchain in encrypted form and have to be processed. Therefore, they cost fees and the messages must be signed by the sender and a wallet is required for signing the transactions.

All transactions can also be simulated via ``Tx.Simulate`` to determine the estimated fees.

**Permits** can be signed with the ```Permit.Sign``` method.

**All types and methods are documented and eases programming:**

![](resources/VS_IntelliSense.png)

# Installation
The Secret.NET Core Libray can be easily installed via Nuget: 

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
# Usage Examples
Note: Public gRPC-web endpoints can be found in https://github.com/scrtlabs/api-registry for both mainnet and testnet.

For a lot more usage examples refer to the tests.

## Sending Queries
```csharp
using SecretNET;

// grpcWebUrl => TODO get from https://github.com/scrtlabs/api-registry
// chainId e.g. "secret-4" or "pulsar-2"
var secretClient = new SecretNetworkClient(grpcWebUrl, chainId, wallet: null); // no wallet needed for queries

var response = await secretClient.Query.Bank.Balance("secret1ap26qrlp8mcq2pg6r47w43l0y8zkqm8a450s03");
Console.WriteLine("Balance: " + (float.Parse(response.Amount) / 1000000f)); // denom: "uscrt"

// simple counter contract
var contractAddress = "secret12j8e6aeyy9ff7drfks3knxva0pxj847fle8zsf"; // pulsar-2
var contractCodeHash = "3d528d0d9889d5887abd3e497243ed6e5a4da008091e20ee420eca39874209ad";

var getCountQueryMsg = new { get_count = new { } };

var queryContractResult = await secretClient.Query.Compute.QueryContract<object>(
				contractAddress: contractAddress, 
				queryMsg: getCountQueryMsg, 
				codeHash: contractCodeHash); // optional but way faster

Console.WriteLine(queryContractResult.Response); // JSON string
```
## Broadcasting Transactions
```csharp
using SecretNET;

var myWallet = await SecretNET.Wallet.Create("detect unique diary skate horse hockey gain naive approve rabbit act lift");

// grpcWebUrl => TODO get from https://github.com/scrtlabs/api-registry
// chainId e.g. "secret-4" or "pulsar-2"
var secretClient = new SecretNetworkClient(grpcWebUrl, chainId, wallet: myWallet); // wallet needed for transactions

// simple counter contract
var contractAddress = "secret12j8e6aeyy9ff7drfks3knxva0pxj847fle8zsf"; // pulsar-2
var contractCodeHash = "3d528d0d9889d5887abd3e497243ed6e5a4da008091e20ee420eca39874209ad";

var msgExecuteContract = new SecretNET.Tx.MsgExecuteContract(
				contractAddress: contractAddress, 
				msg: new { increment = new { } }, 
				codeHash: contractCodeHash); // optional but way faster

var txOptionsExecute = new TxOptions()
{
    GasLimit = 300_000,
    GasPriceInFeeDenom = 0.26F
};

var executeContractResult = await secretClient.Tx.Compute.ExecuteContract(
				msg: msgExecuteContract, 
				txOptions: txOptionsExecute);
```

You can find **more examples** in the [ready to run example CLI project](https://github.com/0xxCodemonkey/SecretNET/blob/main/examples/SecretNET.Examples/Program.cs).

# API
## Creating / Initializing the wallet
When initializing a wallet, you must pass an ```IPrivateKeyStorage``` provider where the private key and mnemonic phrase will be stored (default = MauiSecureStorage). 
The following providers are available out of the box:

- **MauiSecureStorage**
Utilized the [Microsoft.Maui.Storage.SecureStorage](https://docs.microsoft.com/en-us/dotnet/maui/platform-integration/storage/secure-storage) to securely save data (can only be used in MAUI apps).
- **AesEncryptedFileStorage**
AesEncryptedFileStorage encryptes data to the local file system (not recommended in MAUI Apps since password is stored in memory).
- **InMemoryOnlyStorage**
Stores the data ONLY (unencrypted!) in memory.

The .NET MAUI SecureStorage uses the OS capabilities as follows to store the data securely:

| Plattform | Info |
| ------------- | -------------  |
| Android | Data is encrypted with the Android EncryptedSharedPreference Class, and the secure storage value is encrypted with AES-256 GCM. |
| iOS / macOS | The iOS Keychain is used to store values securely. | 
| Windows | DataProtectionProvider is used to encrypt values securely on Windows devices. | 

:warning: **Never store the private key or mnemonic phrase permanent in a variable (or somewhere else than in a secure storage) or output them in a log!**
**Only store them in a secure storage like the default implementation.**

### Importing account from mnemonic phrase
Simply use the mnemonic phrase in the ```Wallet.Create``` method (and use the wallet in the constructor of the SecretNetworkClient:) like this

```  csharp
var walletFromMnemonic = await SecretNET.Wallet.Create("detect unique diary skate horse hockey gain naive approve rabbit act lift");
```
### Importing private key
Simply use the private key in the ```Wallet.Create``` method (and use the wallet in the constructor of the SecretNetworkClient) like this:
```  csharp
var walletFromSeed = await SecretNET.Wallet.Create(** byte[] private key **);
```

### Use previously saved wallet

```csharp
var secureLocalStorageProvider = new MauiSecureStorage();
var securewalletOptions = new SecretNET.Common.CreateWalletOptions(secureLocalStorageProvider);

Wallet secureWallet = null;
if (await secureLocalStorageProvider.HasPrivateKey())
{
    var storedMnemonic = await secureLocalStorageProvider.GetFirstMnemonic();
    secureWallet = await SecretNET.Wallet.Create(storedMnemonic, options: securewalletOptions);
}
```

### Import via Keplr QR
- coming soon...
### Generating a new account
To generate a complete new random wallet just use the ```Wallet.Create``` method without a parameter (default wordlist is english), or specify another Wordlist (also supported: chinese_simplified, chinese_traditional, japanese, spanish, french, portuguese_brazil and czech).
```  csharp
var newRandomWallet = await SecretNET.Wallet.Create(Wordlist.English);
```
### Attaching the wallet to the SecretNetworkClient (required for signing transactions)
In the constructor:
```  csharp
var secretNetworkClient = new SecretNetworkClient("https://pulsar-2.api.trivium.network:9091", "pulsar-2", wallet);
```
Later via prop:
```  csharp
secretNetworkClient.Wallet = walletFromMnemonic;
```
## SecretNetworkClient
[**Full API »**](https://0xxcodemonkey.github.io/SecretNET/html/T-SecretNET.SecretNetworkClient.htm)

### Querier (`secretClient.Query`)
The querier can only send queries and get chain information. Access to all query types can be done via ```SecretNetworkClient.Query```.

You can find a full list of all query methods below under ['All query methods'](#all-query-methods) or in the [**Full API »**](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Query.Queries.htm)

#### `secretClient.Query.GetTx(string hash, bool tryToDecrypt = true)`
Returns a transaction with a txhash. `hash` is a 64 character upper-case hex string.

If set the parameter tryToDecrypt to true (default) the client tries to decrypt the tx data (works only if the tx was created in the same session / client instance or if the same CreateClientOptions.EncryptionSeed is used).

#### `secretClient.Query.TxsQuery(string query, bool tryToDecrypt = false)`
Returns all transactions that match a query.

If set the parameter `tryToDecrypt` to true (default = false) the client tries to decrypt the tx data (works only if the tx was created in the same session / client instance or if the same ```CreateClientOptions.EncryptionSeed``` is used).

To tell which events you want, you need to provide a query. query is a string, which has a form: `condition AND condition ...` (no OR at the moment). Condition has a form: `key operation operand`. key is a string with a restricted set of possible symbols (`\t`, `\n`, `\r`, `\`, `(`, `)`, `"`, `'`, `=`, `>`, `<` are not allowed). Operation can be `=`, `<`, `<=`, `>`, `>=`, `CONTAINS` AND `EXISTS`. Operand can be a string (escaped with single quotes), number, date or time.

Examples:

- `tx.hash = 'XYZ'` # single transaction
- `tx.height = 5` # all txs of the fifth block
- `create_validator.validator = 'ABC'` # tx where validator ABC was created

Tendermint provides a few predefined keys: `tx.hash` and `tx.height`. You can provide additional event keys that were emitted during the transaction. All events are indexed by a composite key of the form `{eventType}.{evenAttrKey}`. Multiple event types with duplicate keys are allowed and are meant to categorize unique and distinct events.

To create a query for txs where AddrA transferred funds: `transfer.sender = 'AddrA'`

See `txsQuery` under https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Query.Queries.htm

#### Get SCRT Balance

```csharp
var response = await secretClient.Query.Bank.Balance("secret1ap26qrlp8mcq2pg6r47w43l0y8zkqm8a450s03");
Console.WriteLine("Balance: " + (float.Parse(response.Amount) / 1000000f)); // denom: "uscrt"
```

#### Query smart contract (permissionless method)

```csharp
// grpcWebUrl => TODO get from https://github.com/scrtlabs/api-registry
// chainId e.g. "secret-4" or "pulsar-2"
var secretClient = new SecretNetworkClient(grpcWebUrl, chainId, wallet: null);

// simple counter contract
var contractAddress = "secret12j8e6aeyy9ff7drfks3knxva0pxj847fle8zsf"; // pulsar-2
var contractCodeHash = "3d528d0d9889d5887abd3e497243ed6e5a4da008091e20ee420eca39874209ad";

var getCountQueryMsg = new { get_count = new { } };

var queryContractResult = await secretClient.Query.Compute.QueryContract<object>(
				contractAddress: contractAddress, 
				queryMsg: getCountQueryMsg, 
				codeHash: contractCodeHash); // optional but way faster

Console.WriteLine(queryContractResult.Response); // JSON string
```

Or even easier for a **SNIP20 Contract** via [**SecretNET.SNIP20**](#additional-packages) Add-On:

```csharp
var snip20Client =  new SecretNET.SNIP20.Snip20Client(secretClient);

var sSCRT_Address = "secret1k0jntykt7e4g3y88ltc60czgjuqdy4c9e8fzek"; // secret-4
var sScrt_CodeHash = "af74387e276be8874f07bec3a87023ee49b0e7ebe08178c49d0a49c3c98ed60e";

var tokenInfoResult = (await snip20Client.Query.GetTokenInfo(
					contractAddress: sSCRT_Address, 
					codeHash:  sScrt_CodeHash // optional but way faster
          )).Response.Result;

Console.WriteLine($"TokenName: {tokenInfoResult.Name}, Symbol: {tokenInfoResult.Symbol}");
```

You can find **more examples** in the [ready to run example CLI project](https://github.com/0xxCodemonkey/SecretNET/blob/main/examples/SecretNET.Examples/Program.cs).

## Transactions
On a signer Secret.NET client, `SecretNetworkClient.Tx` is used to broadcast transactions. Every function under `SecretNetworkClient.Tx` can receive an optional `TxOptions`.

You can find a full list of all transaction methods below under ['All transaction methods'](#all-transaction-methods) or in the [**Full API »**]([https://0xxcodemonkey.github.io/SecretNET/html/T-SecretNET.SecretNetworkClient.htm](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Tx.TxClient.htm))

### Broadcasting transactions
Used to send a complex transactions, which contains a list of messages. The messages are executed in sequence, and the transaction succeeds if all messages succeed.
See code example in simulate.

### Simulate transactions
Used to simulate a complex transactions, which contains a list of messages, without broadcasting it to the chain. Can be used to get a gas estimation or to see the output without actually committing a transaction on-chain.

The input should be exactly how you'd use it in `SecretNetworkClient.Tx.Broadcast()`, except that you don't have to pass in `gasLimit`, `gasPriceInFeeDenom` & `feeDenom`.

Notes:

- :warning: On mainnet it's recommended to not simulate every transaction as this can burden your node provider. Instead, use this while testing to determine the gas limit for each of your app's transactions, then in production use hard-coded values.
- Gas estimation is known to be a bit off, so you might need to adjust it a bit before broadcasting.

```csharp
var sendToAlice = new Cosmos.Bank.V1Beta1.MsgSend()
{
    FromAddress = wallet.Address,
    ToAddress = subaccountWallet.Address
};
sendToAlice.Amount.Add(new Cosmos.Base.V1Beta1.Coin() { Amount = "1", Denom = "uscrt" });

var sendToEve = new Cosmos.Bank.V1Beta1.MsgSend()
{
    FromAddress = wallet.Address,
    ToAddress = subaccountWallet.Address // use the same address for simplicity
};
sendToEve.Amount.Add(new Cosmos.Base.V1Beta1.Coin() { Amount = "1", Denom = "uscrt" });

var messages = new[] { sendToAlice, sendToEve };

var simulate = await secretClient.Tx.Simulate(messages);

var tx = await secretClient.Tx.Broadcast(messages, new TxOptions
{
    // Adjust gasLimit up by 10% to account for gas estimation error
    GasLimit = (int)Math.Ceiling(simulate.GasInfo.GasUsed * 1.1),
});
```

### Sending SCRT

```csharp
var sendResponse = await secretClient.Tx.Bank.Send(
				toAddress: alice, 
				amount: 1000000, 
				denom: null,	// default "uscrt"
				new TxOptions{ GasLimit = 20000,}
				);
				
var success = sendResponse.Code == 0;
```

### Uploading a smart contract
With you can upload a compiled contract to Secret Network.

Input: MsgStoreCodeParams

```csharp
// https://github.com/0xxCodemonkey/SecretNET/blob/main/resources/mysimplecounter.wasm.gz
byte[] wasmByteCode = File.ReadAllBytes(@"Resources\mysimplecounter.wasm.gz");

// MsgStoreCode
var msgStoreCodeCounter = new MsgStoreCode(wasmByteCode,
                        source: "https://github.com/scrtlabs/secret-template", // Source is a valid absolute HTTPS URI to the contract's source code, optional
                        builder: "enigmampc/secret-contract-optimizer:latest"  // Builder is a valid docker image name with tag, optional
                        );

var storeCodeResponse = await secretClient.Tx.Compute.StoreCode(msgStoreCodeCounter, txOptions: txOptionsUpload);

Console.WriteLine("Init Contract with CodeId " + storeCodeResponse.Response.CodeId);

```
### Instantiate a contract from code id

```csharp
var codeId = storeCodeResponse.Response.CodeId; // see above
var codeHash = await secretClient.Query.Compute.GetCodeHashByCodeId(codeId); // get codeHash (optional)

var msgInitContract = new MsgInstantiateContract(
                            codeId: codeId, 
                            label: $"MySimpleCouter {codeId}", 
                            initMsg: new { count = 100 }, 
                            codeHash: codeHash	// optional but way faster
			    );

var initContractResponse = await secretClient.Tx.Compute.InstantiateContract(msgInitContract, txOptions: txOptionsUpload);
logSecretTx("InstantiateContract", initContractResponse);

const contractAddress = initContractResponse.Response.Address);

```

### Calling a smart contract
Execute a function on a contract

```csharp
var msgExecuteContract = new MsgExecuteContract(
				contractAddress: contractAddress, 
				msg: executeMsg, 
				codeHash: contractCodeHash,
				sender: null, 		// optional => set to the wallet address
				sentFunds: null 	// optional
				);
				
var tx = await secretClient.Tx.Compute.ExecuteContract(msgExecuteContract, new TxOptions
{
    // Adjust gasLimit to you got from a simulate tx
    GasLimit = 150000,
});

```

You can find more examples in the [ready to run example CLI project](https://github.com/0xxCodemonkey/SecretNET/blob/main/examples/SecretNET.Examples/Program.cs).

# Overview of all query and transaction methods

## All query methods
- secretClient.Query.Auth
- secretClient.Query.Authz
- secretClient.Query.Bank
- secretClient.Query.Compute
- secretClient.Query.Distribution
- secretClient.Query.Evidence
- secretClient.Query.Feegrant
- secretClient.Query.Gov
- secretClient.Query.IbcChannel
- secretClient.Query.IbcClient
- secretClient.Query.IbcConnection
- secretClient.Query.IbcTransfer
- secretClient.Query.Mint
- secretClient.Query.Params
- secretClient.Query.Registration
- secretClient.Query.Slashing
- secretClient.Query.Staking
- secretClient.Query.Tendermint
- secretClient.Query.Upgrade

See all details in the [**Full API »**](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Query.Queries.htm)

### [secretClient.Query](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Query.Queries.htm)
- `GetTx(string hash, bool tryToDecrypt = true)` => Returns a transaction with a txhash. `hash` is a 64 character upper-case hex string. (see [above](#secretclientquerygettxstring-hash-bool-trytodecrypt--true))
- `TxsQuery(string query, bool tryToDecrypt = false)` => Returns all transactions that match a query.  (see [above](#secretclientquerytxsquerystring-query-bool-trytodecrypt--false))
- `GetTxsEvent(GetTxsEventRequest request, bool tryToDecrypt = false)` => Returns all transactions that matches the specified events (`GetTxsEventRequest.Events`).

### [secretClient.Query.Auth](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Query.AuthQueryClient.htm)
- `Account(string address)` => Returns account details based on address.
- `Accounts()` => Returns all existing accounts on the blockchain.

### [secretClient.Query.Authz](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Query.AuthzQueryClient.htm)
- `GranteeGrants(QueryGranteeGrantsRequest request)` => GranteeGrants returns a list of `GrantAuthorization` by grantee. Since: cosmos-sdk 0.45.2.
- `GranterGrants(QueryGranterGrantsRequest request)` => GranterGrants returns list of `GrantAuthorization`, granted by granter. Since: cosmos-sdk 0.45.2.
- `Grants(QueryGrantsRequest request)` => Returns list of `Authorization`, granted to the grantee by the granter.

### [secretClient.Query.Bank](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Query.BankQueryClient.htm)
- `Balance(QueryBalanceRequest request)` / `Balance(string address, string denom)` => Balance queries the balance of **a single** coin for a single account.
- `Balance(QueryAllBalancesRequest request)` => AllBalances queries the balance of **all coins** for a single account.
- `DenomMetadata(QueryDenomMetadataRequest request)` => DenomsMetadata queries the client metadata of **a given coin** denomination.
- `DenomsMetadata(QueryDenomsMetadataRequest request)` => DenomsMetadata queries the client metadata **for all registered** coin denominations.
- `Params(QueryParamsRequest request)` => Params queries the parameters of x/bank module.
- `SpendableBalances(QuerySpendableBalancesRequest request)` => SpendableBalances queries the spenable balance of all coins for a single.
- `SupplyOf(QuerySupplyOfRequest request)` => SupplyOf queries the supply of a single coin.
- `TotalSupply(QueryTotalSupplyRequest request)` => TotalSupply queries the total supply of all coins.

### [secretClient.Query.Compute](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Query.ComputeQueryClient.htm)
- `Code(ulong codeId, Metadata metadata)` => Get WASM bytecode and metadata for a code id.
- `Codes(Metadata metadata)` => Query all codes on chain.
- `ContractInfo(string contractAddress, Metadata metadata)` => Get metadata of a Secret Contract.
- `ContractsByCode(ulong codeId, Metadata metadata)` => Get all contracts that were instantiated from a code id.
- `GetCodeHash(string contractAddress, Metadata metadata)` => Get the codeHash of a Secret Contract.
- `GetCodeHashByCodeId(ulong codeId, Metadata metadata)` => Get the codeHash from a code id.
- `QueryContract<R>(string contractAddress, Object queryMsg, string codeHash, Metadata metadata)` => Query a Secret Contract and cast the response as `R`. (see above)

### [secretClient.Query.Distribution](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Query.DistributionQueryClient.htm)
- `Balance(QueryParamsRequest request)` => CommunityPool queries the community pool coins.
- `CommunityPool(QueryCommunityPoolRequest request)` => Query all codes on chain.
- `DelegationRewards(QueryDelegationRewardsRequest request)` => DelegationRewards queries the total rewards accrued by a delegation.
- `DelegationTotalRewards(QueryDelegationTotalRewardsRequest request)` => DelegationTotalRewards queries the total rewards accrued by a each.
- `DelegatorValidators(QueryDelegatorValidatorsRequest request)` => DelegatorValidators queries the validators of a delegator.
- `DelegatorWithdrawAddress(QueryDelegatorWithdrawAddressRequest request)` => DelegatorWithdrawAddress queries withdraw address of a delegator.
- `FoundationTax(QueryFoundationTaxRequest request)` => FoundationTax queries.
- `ValidatorCommission(QueryValidatorCommissionRequest request)` => ValidatorCommission queries accumulated commission for a validator.
- `ValidatorOutstandingRewards(QueryValidatorCommissionRequest request)` => ValidatorOutstandingRewards queries rewards of a validator address.
- `ValidatorSlashes(QueryValidatorCommissionRequest request)` => ValidatorSlashes queries slash events of a validator.

### [secretClient.Query.Evidence](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Query.EvidenceQueryClient.htm)
- `AllEvidence(QueryAllEvidenceRequest request)` => AllEvidence queries all evidence.
- `FoundationTax(QueryEvidenceRequest request)` => Evidence queries evidence based on evidence hash.

### [secretClient.Query.Feegrant](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Query.FeegrantQueryClient.htm)
- `Allowance(QueryAllowanceRequest request)` => Allowance returns fee granted to the grantee by the granter.
- `Allowances(QueryAllowancesRequest request)` => Allowances returns all the grants for address.

### [secretClient.Query.Gov](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Query.GovQueryClient.htm)
- `Deposit(QueryDepositRequest request)` => Deposit queries single deposit information based proposalID, depositAddr.
- `Deposits(QueryDepositsRequest request)` => Deposits queries all deposits of a single proposal.
- `Params(QueryParamsRequest request)` => Params queries all parameters of the gov module.
- `Proposal(QueryProposalRequest request)` => Proposal queries proposal details based on ProposalID.
- `Proposals(QueryProposalsRequest request)` => Proposals queries all proposals based on given status.
- `TallyResult(QueryTallyResultRequest request)` => TallyResult queries the tally of a proposal vote.
- `Vote(QueryVoteRequest request)` => Vote queries voted information based on proposalID, voterAddr.
- `Votes(QueryVotesRequest request)` => Votes queries votes of a given proposal.

### [secretClient.Query.IbcChannel](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Query.IbcChannelQueryClient.htm)
- `Channel(QueryChannelRequest request)` => Channel queries an IBC Channel.
- `Channels(QueryChannelsRequest request)` => Channels queries all the IBC channels of a chain.
- `ChannelClientState(QueryChannelClientStateRequest request)` => ChannelClientState queries for the client state for the channel associated with the provided channel identifiers.
- `ChannelConsensusState(QueryChannelConsensusStateRequest request)` => ChannelConsensusState queries for the consensus state for the channel associated with the provided channel identifiers.
- `ConnectionChannels(QueryConnectionChannelsRequest request)` => ConnectionChannels queries all the channels associated with a connection end.
- `NextSequenceReceive(QueryNextSequenceReceiveRequest request)` => NextSequenceReceive returns the next receive sequence for a given channel.
- `PacketAcknowledgement(QueryPacketAcknowledgementRequest request)` => PacketAcknowledgement queries a stored packet acknowledgement hash.
- `PacketAcknowledgements(QueryPacketAcknowledgementsRequest request)` => PacketAcknowledgements returns all the packet acknowledgements associated with a channel.
- `PacketCommitment(QueryPacketCommitmentRequest request)` => PacketCommitment queries a stored packet commitment hash.
- `PacketCommitments(QueryPacketCommitmentsRequest request)` => PacketCommitments returns all the packet commitments hashes associated with a channel.
- `PacketReceipt(QueryPacketReceiptRequest request)` => PacketReceipt queries if a given packet sequence has been received on the queried chain.
- `UnreceivedAcks(QueryUnreceivedAcksRequest request)` => UnreceivedAcks returns all the unreceived IBC acknowledgements associated with a channel and sequences.
- `UnreceivedPackets(QueryUnreceivedPacketsRequest request)` => UnreceivedPackets returns all the unreceived IBC packets associated with a channel and sequences.

### [secretClient.Query.IbcClient](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Query.IbcClientQueryClient.htm)
- `ClientParams(QueryClientParamsRequest request)` => ClientParams queries all parameters of the ibc client.
- `ClientState(QueryClientStateRequest request)` => ClientState queries an IBC light client.
- `ClientStates(QueryClientStatesRequest request)` => ClientStates queries all the IBC light clients of a chain.
- `ClientStatus(QueryClientStatusRequest request)` => Status queries the status of an IBC client.
- `ConsensusState(QueryConsensusStateRequest request)` => ConsensusState queries a consensus state associated with a client state at a given height.
- `ConsensusStates(QueryTallyResultRequest request)` => ConsensusStates queries all the consensus state associated with a given client.
- `UpgradedClientState(QueryUpgradedClientStateRequest request)` => UpgradedClientState queries an Upgraded IBC light client.
- `UpgradedConsensusState(QueryUpgradedConsensusStateRequest request)` => UpgradedConsensusState queries an Upgraded IBC consensus state.

### [secretClient.Query.IbcConnection](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Query.IbcConnectionQueryClient.htm)
- `ClientConnections(QueryClientConnectionsRequest request)` => ClientConnections queries the connection paths associated with a client state.
- `Connection(QueryConnectionRequest request)` => Connection queries an IBC connection end.
- `Connections(QueryConnectionsRequest request)` => Connections queries all the IBC connections of a chain.
- `ConnectionClientState(QueryConnectionClientStateRequest request)` => ConnectionClientState queries the client state associated with the connection.
- `ConnectionConsensusState(QueryConnectionConsensusStateRequest request)` => ConnectionConsensusState queries the consensus state associated with the connection.

### [secretClient.Query.IbcTransfer](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Query.IbcTransferQueryClient.htm)
- `DenomHash(QueryDenomHashRequest request)` => DenomHash queries a denomination hash information.
- `DenomTrace(QueryDenomTraceRequest request)` => DenomTrace queries a denomination trace information.
- `DenomTraces(QueryDenomTracesRequest request)` => DenomTraces queries all denomination traces.
- `Params(QueryParamsRequest request)` => Params queries all parameters of the ibc-transfer module.

### [secretClient.Query.Mint](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Query.MintQueryClient.htm)
- `AnnualProvisions(QueryAnnualProvisionsRequest request)` => AnnualProvisions current minting annual provisions value.
- `Inflation(QueryInflationRequest request)` => Inflation returns the current minting inflation value.
- `Params(QueryParamsRequest request)` => Params returns the total set of minting parameters.

### [secretClient.Query.Params](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Query.ParamsQueryClient.htm)
- `Params(QueryParamsRequest request)` => Params queries a specific parameter of a module, given its subspace and key.

### [secretClient.Query.Registration](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Query.RegistrationQueryClient.htm)
- `EncryptedSeed(QueryEncryptedSeedRequest request)` => Encrypteds the seed.
- `RegistrationKey()` => Returns the key used for registration.
- `TxKey()` => Returns the key used for transactions.

### [secretClient.Query.Slashing](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Query.SlashingQueryClient.htm)
- `Params(QueryParamsRequest request)` => Params queries the parameters of slashing module.
- `SigningInfo(QuerySigningInfoRequest request)` => SigningInfo queries the signing info of given cons address.
- `SigningInfos(QuerySigningInfosRequest request)` => SigningInfos queries signing info of all validators.

### [secretClient.Query.Staking](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Query.StakingQueryClient.htm)
- `Delegation(QueryDelegationRequest request)` => Delegation queries delegate info for given validator delegator pair.
- `DelegatorDelegations(QueryDelegatorDelegationsRequest request)` => DelegatorDelegations queries all delegations of a given delegator address.
- `DelegatorValidator(QueryDelegatorValidatorRequest request)` => DelegatorValidator queries validator info for given delegator validator pair.
- `DelegatorsValidators(QueryDelegatorValidatorsRequest request)` => DelegatorValidators queries all validators info for given delegator address.
- `DelegatorUnbondingDelegations(QueryDelegatorUnbondingDelegationsRequest request)` => DelegatorUnbondingDelegations queries all unbonding delegations of a given delegator address.
- `HistoricalInfo(QueryHistoricalInfoRequest request)` => HistoricalInfo queries the historical info for given height.
- `Params(QueryParamsRequest request)` => Parameters queries the staking parameters.
- `Pool(QueryPoolRequest request)` => Pool queries the pool info.
- `Redelegations(QueryRedelegationsRequest request)` => Redelegations queries redelegations of given address.
- `UnbondingDelegation(QueryUnbondingDelegationRequest request)` => UnbondingDelegation queries unbonding info for given validator delegator pair.
- `Validator(QueryValidatorRequest request)` => Validator queries validator info for given validator address.
- `Validators(QueryValidatorsRequest request)` => Validators queries all validators that match the given status.
- `ValidatorDelegations(QueryValidatorDelegationsRequest request)` => ValidatorDelegations queries delegate info for given validator.
- `ValidatorUnbondingDelegations(QueryValidatorUnbondingDelegationsRequest request)` => ValidatorUnbondingDelegations queries unbonding delegations of a validator.

### [secretClient.Query.Tendermint](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Query.TendermintQueryClient.htm)
- `GetBlockByHeight(GetBlockByHeightRequest request)` => GetBlockByHeight queries block for given height.
- `GetLatestBlock(GetLatestBlockRequest request)` => GetLatestBlock returns the latest block.
- `GetLatestValidatorSet(GetLatestValidatorSetRequest request)` => GetLatestValidatorSet queries latest validator-set.
- `GetNodeInfo(GetNodeInfoRequest request)` => GetNodeInfo queries the current node info.
- `GetSyncing(GetSyncingRequest request)` => GetSyncing queries node syncing.
- `GetValidatorSetByHeight(GetValidatorSetByHeightRequest request)` => GetValidatorSetByHeight queries validator-set at a given height.

### [secretClient.Query.Upgrade](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Query.UpgradeQueryClient.htm)
- `AppliedPlan(QueryAppliedPlanRequest request)` => AppliedPlan queries a previously applied upgrade plan by its name.
- `CurrentPlan(QueryCurrentPlanRequest request)` => CurrentPlan queries the current upgrade plan.
- `ModuleVersions(QueryModuleVersionsRequest request)` => ModuleVersions queries the list of module versions from state. Since: cosmos-sdk 0.43.

## All transaction methods
- secretClient.Tx.Authz
- secretClient.Tx.Bank
- secretClient.Tx.Compute
- secretClient.Tx.Crisis
- secretClient.Tx.Distribution
- secretClient.Tx.Evidence
- secretClient.Tx.Feegrant
- secretClient.Tx.Gov
- secretClient.Tx.IbcChannel
- secretClient.Tx.IbcClient
- secretClient.Tx.IbcConnection
- secretClient.Tx.IbcTransfer
- secretClient.Tx.Slashing
- secretClient.Tx.Staking
- secretClient.Tx.Vesting

See all details in the [**Full API »**](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Tx.TxClient.htm)

### [secretClient.Tx](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Tx.TxClient.htm)
- `Simulate(IMessage message, TxOptions txOptions = null)` => Used to simulate a complex transactions, which contains a list of messages, without broadcasting it to the chain. (has several overloads).
- `Broadcast(IMessage message, TxOptions txOptions = null)` => Used to send a complex transactions, which contains a list of messages. The messages are executed in sequence, and the transaction succeeds if all messages succeed. (has several overloads).
- `Broadcast<T>(IMessage message, TxOptions txOptions = null)` => Like Broadcast but tries to convert the first message result to T. (has several overloads).

### [secretClient.Tx.Authz](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Tx.AuthzTx.htm)
- `Exec(MsgExec msg, TxOptions txOptions = null)` => Exec attempts to execute the provided messages using authorizations granted to the grantee. Each message should have only one signer corresponding to the granter of the authorization. 
- `Grant(MsgExec msg, TxOptions txOptions = null)` => Grant is a request type for Grant method. It declares authorization to the grantee on behalf of the granter with the provided expiration time. 
- `Revoke(MsgRevoke msg, TxOptions txOptions = null)` => Revoke revokes any authorization with the provided sdk.Msg type on the granter's account with that has been granted to the grantee. 

### [secretClient.Tx.Bank](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Tx.BankTx.htm)
- `Send(string toAddress, int amount, string denom, TxOptions txOptions = null)` => Sends SCRT to the specified to address.
- `Send(MsgSend msg, TxOptions txOptions = null)` => MsgSend represents a message to send coins from one account to another. 
- `MultiSend(MsgMultiSend msg, TxOptions txOptions = null)` => MsgMultiSend represents an arbitrary multi-in, multi-out send message.
- 
### [secretClient.Tx.Compute](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Tx.ComputeTx.htm)
- `ExecuteContract(MsgExecuteContract msg, TxOptions txOptions = null)` => Execute a function on a contract. (see also [above](#calling-a-smart-contract))
- `ExecuteContract<T>(MsgExecuteContract msg, TxOptions txOptions = null)` => Execute a function on a contract and tries to convert the response to T.
- `InstantiateContract(MsgInstantiateContract msg, TxOptions txOptions = null)` => Instantiate a contract from code id. (see also [above](#instantiate-a-contract-from-code-id))
- `StoreCode(MsgStoreCode msg, TxOptions txOptions = null)` => Upload a compiled contract to Secret Network. (see also [above](#uploading-a-smart-contract))

### [secretClient.Tx.Crisis](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Tx.CrisisTx.htm)
- `VerifyInvariant(MsgVerifyInvariant msg, TxOptions txOptions = null)` => MsgVerifyInvariant represents a message to verify a particular invariance. 

### [secretClient.Tx.Distribution](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Tx.DistributionTx.htm)
- `FundCommunityPool(MsgFundCommunityPool msg, TxOptions txOptions = null)` => MsgFundCommunityPool allows an account to directly fund the community pool. 
- `SetWithdrawAddress(MsgSetWithdrawAddress msg, TxOptions txOptions = null)` => MsgSetWithdrawAddress sets the withdraw address for a delegator (or validator self-delegation). 
- `WithdrawDelegatorReward(MsgWithdrawDelegatorReward msg, TxOptions txOptions = null)` => MsgWithdrawDelegatorReward represents delegation withdrawal to a delegator from a single validator. 
- `WithdrawValidatorCommission(MsgExec msg, TxOptions txOptions = null)` => MsgWithdrawValidatorCommission withdraws the full commission to the validator address. 

### [secretClient.Tx.Evidence](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Tx.EvidenceTx.htm)
- `SubmitEvidence(MsgSubmitEvidence msg, TxOptions txOptions = null)` => MsgSubmitEvidence represents a message that supports submitting arbitrary Evidence of misbehavior such as equivocation or counterfactual signing. 

### [secretClient.Tx.Feegrant](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Tx.FeegrantTx.htm)
- `GrantAllowance(MsgGrantAllowance msg, TxOptions txOptions = null)` => MsgGrantAllowance adds permission for Grantee to spend up to Allowance of fees from the account of Granter. 
- `RevokeAllowance(MsgRevokeAllowance msg, TxOptions txOptions = null)` => MsgRevokeAllowance removes any existing Allowance from Granter to Grantee. 

### [secretClient.Tx.Gov](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Tx.GovTx.htm)
- `Deposit(MsgDeposit msg, TxOptions txOptions = null)` => MsgDeposit defines a message to submit a deposit to an existing proposal. 
- `SubmitProposal(MsgSubmitProposal msg, TxOptions txOptions = null)` => MsgSubmitProposal defines an sdk.Msg type that supports submitting arbitrary proposal Content. 
- `Vote(MsgVote msg, TxOptions txOptions = null)` => MsgVote defines a message to cast a vote. 
- `VoteWeighted(MsgVoteWeighted msg, TxOptions txOptions = null)` => MsgVoteWeighted defines a message to cast a vote, with an option to split the vote. 

### [secretClient.Tx.IbcChannel](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Tx.IbcChannelTx.htm)
- `Acknowledgement(MsgAcknowledgement msg, TxOptions txOptions = null)` => 
- `ChannelCloseConfirm(MsgChannelCloseConfirm msg, TxOptions txOptions = null)` => 
- `ChannelCloseInit(MsgChannelCloseInit msg, TxOptions txOptions = null)` => 
- `ChannelOpenAck(MsgChannelOpenAck msg, TxOptions txOptions = null)` => 
- `ChannelOpenConfirm(MsgChannelOpenConfirm msg, TxOptions txOptions = null)` => 
- `ChannelOpenInit(MsgChannelOpenInit msg, TxOptions txOptions = null)` => 
- `ChannelOpenTry(MsgChannelOpenTry msg, TxOptions txOptions = null)` => 
- `RecvPacket(MsgRecvPacket msg, TxOptions txOptions = null)` => 
- `Timeout(MsgTimeout msg, TxOptions txOptions = null)` => 
- `TimeoutOnClose(MsgTimeoutOnClose msg, TxOptions txOptions = null)` => 

### [secretClient.Tx.IbcClient](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Tx.IbcClientTx.htm)
- `CreateClient(MsgCreateClient msg, TxOptions txOptions = null)` => 
- `SubmitMisbehaviour(MsgSubmitMisbehaviour msg, TxOptions txOptions = null)` => 
- `UpdateClient(MsgUpdateClient msg, TxOptions txOptions = null)` => 
- `UpgradeClient(MsgUpgradeClient msg, TxOptions txOptions = null)` => 

### [secretClient.Tx.IbcConnection](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Tx.IbcConnectionTx.htm)
- `ConnectionOpenAck(MsgConnectionOpenAck msg, TxOptions txOptions = null)` => 
- `ConnectionOpenConfirm(MsgConnectionOpenConfirm msg, TxOptions txOptions = null)` => 
- `ConnectionOpenInit(MsgConnectionOpenInit msg, TxOptions txOptions = null)` => 
- `ConnectionOpenTry(MsgConnectionOpenTry msg, TxOptions txOptions = null)` => 

### [secretClient.Tx.IbcTransfer](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Tx.IbcTransferTx.htm)
- `Transfer(MsgTransfer msg, TxOptions txOptions = null)` => MsgTransfer defines a msg to transfer fungible tokens (i.e Coins) between ICS20 enabled chains. See ICS Spec here: https://github.com/cosmos/ics/tree/master/spec/ics-020-fungible-token-transfer#data-structures 

### [secretClient.Tx.Slashing](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Tx.SlashingTx.htm)
- `Unjail(MsgUnjail msg, TxOptions txOptions = null)` => MsgUnjail defines a message to release a validator from jail. 

### [secretClient.Tx.Staking](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Tx.StakingTx.htm)
- `BeginRedelegate(MsgBeginRedelegate msg, TxOptions txOptions = null)` => MsgBeginRedelegate defines an SDK message for performing a redelegation of coins from a delegator and source validator to a destination validator. 
- `CreateValidator(MsgCreateValidator msg, TxOptions txOptions = null)` => MsgCreateValidator defines an SDK message for creating a new validator. 
- `Delegate(MsgDelegate msg, TxOptions txOptions = null)` => MsgDelegate defines an SDK message for performing a delegation of coins from a delegator to a validator. 
- `EditValidator(MsgEditValidator msg, TxOptions txOptions = null)` => MsgEditValidator defines an SDK message for editing an existing validator. 
- `Undelegate(MsgUndelegate msg, TxOptions txOptions = null)` => MsgUndelegate defines an SDK message for performing an undelegation from a delegate and a validator 

### [secretClient.Tx.Vesting](https://0xxcodemonkey.github.io/SecretNET/html/AllMembers.T-SecretNET.Tx.VestingTx.htm)
- `CreateVestingAccount(MsgCreateVestingAccount msg, TxOptions txOptions = null)` => MsgCreateVestingAccount defines a message that enables creating a vesting account. 


