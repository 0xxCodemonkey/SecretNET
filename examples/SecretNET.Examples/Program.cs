global using Newtonsoft.Json;

// SecretNET
global using SecretNET;
global using SecretNET.Tx;
global using SecretNET.Common;
global using SecretNET.Common.Storage;
using System.Reflection.Metadata;


// See https://aka.ms/new-console-template for more information

#region *** Helper functions / Objects ***

Action<string> writeHeadline = (text) =>
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"\r\n**************** {text} ****************\r\n");
    Console.ForegroundColor = ConsoleColor.White;
};

Action<string, SecretTx> logSecretTx = (name, tx) =>
{
    Console.WriteLine($"{name} Txhash: {tx?.Txhash}");
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine($"(Gas used: {tx.GasUsed} - Gas wanted {tx.GasWanted})");
    Console.ForegroundColor = ConsoleColor.White;

    //Console.WriteLine($"\r\nCodeId: {JsonConvert.SerializeObject(tx.GetResponseJson(), Formatting.Indented)}");

    if (tx is SingleSecretTx<Secret.Compute.V1Beta1.MsgStoreCodeResponse>)
    {
        Console.WriteLine($"\r\nCodeId: {((SingleSecretTx<Secret.Compute.V1Beta1.MsgStoreCodeResponse>)tx).Response.CodeId}");
    }

    if (tx is SingleSecretTx<Secret.Compute.V1Beta1.MsgInstantiateContractResponse>)
    {
        Console.WriteLine($"\r\nContractAddress: {((SingleSecretTx<Secret.Compute.V1Beta1.MsgInstantiateContractResponse>)tx)?.Response?.Address}");
    }

    if (tx != null && (tx.Code > 0 || (tx.Exceptions?.Any()).GetValueOrDefault()))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        if (tx.Code > 0 && !string.IsNullOrWhiteSpace(tx.Codespace))
        {
            Console.WriteLine($"\r\n!!!!!!!!!!!! Something went wrong => Code: {tx.Code}; Codespace: {tx.Codespace} !!!!!!!!!!!!");
        }
        if ((tx.Exceptions?.Any()).GetValueOrDefault())
        {
            foreach (var ex in tx.Exceptions)
            {
                Console.WriteLine($"\r\n!!!!!!!!!!!! Exception: {ex.Message} !!!!!!!!!!!!");
            }
        }
        Console.WriteLine($"\r\n{tx.RawLog}");
        Console.ForegroundColor = ConsoleColor.White;
        Console.ReadLine();
    }
};

// TxOptions
var txOptions = new TxOptions()
{
    GasLimit = 60_000,
    GasPriceInFeeDenom = 0.26F
};

var txOptionsExecute = new TxOptions()
{
    GasLimit = 300_000,
    GasPriceInFeeDenom = 0.26F
};

var txOptionsUpload = new TxOptions()
{
    GasLimit = 12_000_000,
    GasPriceInFeeDenom = 0.26F
};

#endregion

writeHeadline("Setup SecretNetworkClient / Wallet");

//var storageProvider = new InMemoryOnlyStorage(); // Temporary and most likely only for DEV
var storageProvider = new AesEncryptedFileStorage("","SuperSecurePassword");
var createWalletOptions = new CreateWalletOptions(storageProvider);

Wallet wallet = null;
if (await storageProvider.HasPrivateKey())
{
    var storedMnemonic = await storageProvider.GetFirstMnemonic();
    wallet = await Wallet.Create(storedMnemonic, options: createWalletOptions);    
}
else
{
    wallet = await Wallet.Create(options: createWalletOptions);

    Console.WriteLine("Please first fund the wallet with some SCRT via https://faucet.pulsar.scrttestnet.com/ ");
    Console.ReadLine();
}

var gprcUrl = "https://grpc.testnet.secretsaturn.net"; // get from https://github.com/scrtlabs/api-registry
var chainId = "pulsar-2";

var createClientOptions = new CreateClientOptions(gprcUrl, chainId, wallet);
var secretClient = new SecretNetworkClient(createClientOptions);

Console.WriteLine("wallet.Address: " + wallet.Address);


#region *** Get Balance ***

// *** Get Balance (1000000 uscrt == 1 SCRT) ***

writeHeadline("Get Balance");

var response = await secretClient.Query.Bank.Balance(wallet.Address);
Console.WriteLine($"Balance: {(float.Parse(response.Amount) / 1000000f)} SCRT");

//Console.ReadLine();

#endregion

#region *** Get Subacccount and Send $SCRT ***

// Send SCRT
var subaccountWallet = await wallet.GetSubaccount(1);
Console.WriteLine($"\r\nSubaccount.Address: {subaccountWallet.Address}");

var sendResponse = await secretClient.Tx.Bank.Send(toAddress: subaccountWallet.Address, amount: 1000000, denom: null);
Console.WriteLine($"BroadcastResponse: {(sendResponse.Code == 0 ? "Success" : "Error (see log)")}");

var r1 = await secretClient.Query.Bank.Balance(subaccountWallet.Address);
Console.WriteLine($"Subaccount Balance: {(float.Parse(r1.Amount) / 1000000f)} SCRT\r\n");

//Console.ReadLine();

#endregion

#region *** Simulate and broadcast a complex transaction  ***

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

Console.WriteLine($"Simulate => GasUsed {simulate.GasInfo.GasUsed} uscrt");

var tx = await secretClient.Tx.Broadcast(messages, new TxOptions
{
    // Adjust gasLimit up by 10% to account for gas estimation error
    GasLimit = (int)Math.Ceiling(simulate.GasInfo.GasUsed * 1.1),
});

logSecretTx("Broadcast result", tx);

//Console.ReadLine();

#endregion

#region *** Auth  ***

// *** Get Account ***

writeHeadline("Get Account");

var accountResponse = await secretClient.Query.Auth.Account(wallet.Address);
Console.WriteLine("AccountResponse:\r\n" + JsonConvert.SerializeObject(accountResponse, Formatting.Indented) + "\r\n");

//Console.ReadLine();

#endregion

#region *** Codes  ***

// *** Get Codes with source ***

writeHeadline("Get my Codes with source");

var codesResponse = await secretClient.Query.Compute.Codes();
var withSource = codesResponse.Where(c => !string.IsNullOrWhiteSpace(c.Source) && c.CreatorAddress == wallet.Address).ToList();
Console.WriteLine($"My Codes with source (Count: {withSource.Count} ):\r\n" + JsonConvert.SerializeObject(withSource, Formatting.Indented) + "\r\n");

//Console.ReadLine();

#endregion

#region *** Smart Contract (Upload, Init, Query, Execute) ***

// *** Smart Contract (Upload, Init, Query, Execute) ***

writeHeadline("Upload Contract (mysimplecounter.wasm.gz)");

// *** Upload Contract ***
// https://secretjs.scrt.network/#secretjstxcomputestorecode

byte[] wasmByteCode = File.ReadAllBytes(@"..\..\..\..\..\Resources\mysimplecounter.wasm.gz");

// MsgStoreCode
var msgStoreCodeCounter = new MsgStoreCode(wasmByteCode,
                        source: "https://github.com/scrtlabs/secret-template", // Source is a valid absolute HTTPS URI to the contract's source code, optional
                        builder: "enigmampc/secret-contract-optimizer:latest"  // Builder is a valid docker image name with tag, optional
                        );

var storeCodeResponse = await secretClient.Tx.Compute.StoreCode(msgStoreCodeCounter, txOptions: txOptionsUpload);
logSecretTx("StoreCodeResponse", storeCodeResponse);

// *** Init Contract ***
writeHeadline("Init Contract with CodeId " + storeCodeResponse.Response.CodeId);

string? contractAddress = null;
string contractCodeHash = null;
if (storeCodeResponse.Response.CodeId > 0)
{
    var codeId = storeCodeResponse.Response.CodeId;
    contractCodeHash = await secretClient.Query.Compute.GetCodeHashByCodeId(codeId);    

    var msgInitContract = new MsgInstantiateContract(
                            codeId: codeId, 
                            label: $"MySimpleCouter {codeId}", 
                            initMsg: new { count = 100 }, 
                            codeHash: contractCodeHash); // optional but way faster

    var initContractResponse = await secretClient.Tx.Compute.InstantiateContract(msgInitContract, txOptions: txOptionsUpload);
    logSecretTx("InstantiateContract", initContractResponse);

    contractAddress = initContractResponse?.Response?.Address;

    Console.WriteLine("Contract CodeHash: " + contractCodeHash);
    Console.WriteLine("Contract Address: " + contractAddress);

}

//Console.ReadLine();

#region *** Query Contract ***

//string contractAddress = "Set manual if needed";
//string contractCodeHash = "Set manual if needed";

// *** Query Contract ***

writeHeadline("Query Contract with address " + contractAddress);

var queryMsg = new { get_count = new { } };

Console.WriteLine("Query : " + JsonConvert.SerializeObject(queryMsg, Formatting.Indented) + "\r\n");

var queryContractResult = await secretClient.Query.Compute.QueryContract<object>(contractAddress, queryMsg, contractCodeHash);
Console.WriteLine("QueryContractResult:\r\n " + queryContractResult.Response);

//Console.ReadLine();

#endregion

#region *** Execute Contract ***

// *** Execute Contract ***

writeHeadline("Execute Contract with address " + contractAddress);

var executeMsg = new { increment = new { } };

Console.WriteLine("Execute : " + JsonConvert.SerializeObject(executeMsg, Formatting.Indented) + "\r\n");

var msgExecuteContract = new MsgExecuteContract(contractAddress: contractAddress, msg: executeMsg, codeHash: contractCodeHash, sender: null, sentFunds: null);

var executeContractResponse = await secretClient.Tx.Compute.ExecuteContract(msgExecuteContract, txOptionsExecute);
logSecretTx("ExecuteContract", executeContractResponse);

Thread.Sleep(1000); // give some time to let it settle

var queryContractResult2 = await secretClient.Query.Compute.QueryContract<object>(contractAddress, queryMsg, contractCodeHash);
Console.WriteLine("\r\nQueryContractResult (again) :\r\n " + queryContractResult2.Response);

Console.ReadLine();

#endregion

#endregion