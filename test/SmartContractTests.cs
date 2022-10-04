using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Newtonsoft.Json;
using SecretNET.Api;
using SecretNET.Tests.Common;
using SecretNET.Tx;
using System.Diagnostics;
using System.Xml.Linq;
using Xunit.Abstractions;

namespace SecretNET.Tests;

//TODO: Write unit / integration tests
// https://github.com/testcontainers/testcontainers-dotnet

[TestCaseOrderer("SecretNET.Tests.Common.PriorityOrderer", "SecretNET.Tests")]
public class SmartContractTests : IClassFixture<TestContext>
{
    TestContext _context;
    readonly ITestOutputHelper _output;

    public SmartContractTests(TestContext context, ITestOutputHelper output)
    {
        _output = output;
        _context = context;
    }

    [Fact, TestPriority(0)]
    public async void StoreCode_Response_ReturnsCodeId()
    {
        // Arrange
        //byte[] wasmByteCode = File.ReadAllBytes(@"..\..\..\..\Resources\mysimplecounter.wasm.gz");
        byte[] wasmByteCode = File.ReadAllBytes(@"..\..\..\..\Resources\snip20-ibc.wasm.gz");

        var msgStoreCodeCounter = new MsgStoreCode(wasmByteCode,
                                source: "", // Source is a valid absolute HTTPS URI to the contract's source code, optional
                                builder: ""  // Builder is a valid docker image name with tag, optional
        );

        // Act
        var storeCodeResponse = await _context.SecretClient.Tx.Compute.StoreCode(msgStoreCodeCounter);
        var codeIdFromLog = storeCodeResponse.TryFindEventValue("code_id");

        // Assert
        Assert.NotNull(storeCodeResponse);
        Assert.Equal((uint)TxResultCode.Success, storeCodeResponse?.Code);
        Assert.True((storeCodeResponse?.Response?.CodeId).GetValueOrDefault() > 0);
        Assert.True(!String.IsNullOrWhiteSpace(codeIdFromLog));

        var codeIdFromLogAsUint = uint.Parse(codeIdFromLog);
        Assert.Equal(codeIdFromLogAsUint, storeCodeResponse?.Response?.CodeId);

        _context.ContractSnip20IbcCodeId = storeCodeResponse.Response.CodeId;
        _output.WriteLine($"ContractSimpleCounterCodeId: {_context.ContractSnip20IbcCodeId}");
    }

    [Fact, TestPriority(1)]
    public async void GetCodeHashByCodeId_Response_ReturnsCodeHash()
    {
        Assert.True(_context.ContractSnip20IbcCodeId > 0);

        // Arrange

        // Act
        var getCodeHashByCodeIdResponse = await _context.SecretClient.Query.Compute.GetCodeHashByCodeId(_context.ContractSnip20IbcCodeId);

        // Assert
        Assert.NotNull(getCodeHashByCodeIdResponse);
        Assert.True(!String.IsNullOrWhiteSpace(getCodeHashByCodeIdResponse));

        _context.ContractSnip20IbcCodeHash = getCodeHashByCodeIdResponse;
        _output.WriteLine($"ContractSimpleCounterCodeHash: {_context.ContractSnip20IbcCodeHash}");
    }

    [Fact, TestPriority(2)]
    public async void Instantiate_Response_ReturnsContractAddress()
    {
        Assert.True(!String.IsNullOrWhiteSpace(_context.ContractSnip20IbcCodeHash));

        // Arrange
        var msgInitContract = new MsgInstantiateContract(
                            codeId: _context.ContractSnip20IbcCodeId,
                            label: $"SecretNET Test {_context.ContractSnip20IbcCodeId}",
                            initMsg: new
                            {
                                name = "Secret SCRT",
                                admin = _context.Wallet.Address,
                                symbol = "SSCRT",
                                decimals = 6,
                                initial_balances = new[] { new { address = _context.Wallet.Address, amount = "1" } },
                                prng_seed = "eW8=",
                                config = new
                                {
                                    public_total_supply = true,
                                    enable_deposit = true,
                                    enable_redeem = true,
                                    enable_mint = false,
                                    enable_burn = false,
                                },
                                supported_denoms = new[] { "uscrt" },
                            },
                            codeHash: _context.ContractSnip20IbcCodeHash); // optional but way faster

        // Act
        var initContractResponse = await _context.SecretClient.Tx.Compute.InstantiateContract(msgInitContract);


        // Assert
        Assert.NotNull(initContractResponse);
        Assert.Equal((uint)TxResultCode.Success, initContractResponse?.Code);
        Assert.True(!String.IsNullOrWhiteSpace(initContractResponse?.Response?.Address));

        var contractAddressFromLog = initContractResponse.TryFindEventValue("contract_address");
        Assert.True(!String.IsNullOrWhiteSpace(contractAddressFromLog));
        Assert.True(contractAddressFromLog.Equals(initContractResponse?.Response?.Address, StringComparison.OrdinalIgnoreCase));

        var actionFromLog = initContractResponse.TryFindEventValue("action");
        Assert.True(!String.IsNullOrWhiteSpace(actionFromLog));
        Assert.True(actionFromLog.Equals("/secret.compute.v1beta1.MsgInstantiateContract", StringComparison.OrdinalIgnoreCase));

        _context.ContractSnip20IbcContractAddress = initContractResponse.Response.Address;
        _output.WriteLine($"ContractSimpleCounterContractAddress: {_context.ContractSnip20IbcContractAddress}");
    }

    //[Fact, TestPriority(3)]
    //public async void ExecuteContract_Response_ReturnsResult()
    //{
    //    Assert.True(!String.IsNullOrWhiteSpace(_context.ContractSnip20IbcContractAddress));

    //    // Arrange
    //    var executeMsg = new MsgExecuteContract(
    //        contractAddress: _context.ContractSnip20IbcContractAddress,
    //        msg: new
    //        {
    //            create_viewing_key = new
    //            {
    //                entropy = "bla bla"
    //            }
    //        },
    //        codeHash: _context.ContractSnip20IbcCodeHash);

    //    // Act
    //    var executeContractResult = await _context.SecretClient.Tx.Compute.ExecuteContract<object>(executeMsg);

    //    // Assert
    //    Assert.NotNull(executeContractResult);
    //    Assert.Equal((uint)TxResultCode.Success, executeContractResult?.Code);
    //    Assert.NotNull(executeContractResult?.Response);
    //    var resultAsString = Convert.ToString(executeContractResult?.Response);
    //    _output.WriteLine($"ResultAsString: {resultAsString}");
    //    Assert.True(!String.IsNullOrWhiteSpace(resultAsString));
    //    Assert.Contains("\"create_viewing_key\": {\r\n    \"key\": \"", resultAsString);

    //}

    //[Fact, TestPriority(4)]
    //public async void ExecuteContractWithWrongParameter_Response_ReturnsError()
    //{
    //    Assert.True(!String.IsNullOrWhiteSpace(_context.ContractSnip20IbcContractAddress));

    //    // Arrange
    //    var executeMsg = new MsgExecuteContract(
    //        contractAddress: _context.ContractSnip20IbcContractAddress,
    //        msg: new {
    //            transfer = new
    //            {
    //                recipient = _context.Wallet.Address,
    //                amount = "2",
    //            }
    //        },
    //        codeHash: _context.ContractSnip20IbcCodeHash);

    //    // simulate would fail
    //    var txOptions = new TxOptions()
    //    {
    //        SkipSimulate = true,
    //        GasLimit = 5_000_000
    //    };

    //    // Act
    //    var executeContractResult = await _context.SecretClient.Tx.Compute.ExecuteContract<object>(executeMsg, txOptions);

    //    // Assert
    //    Assert.NotNull(executeContractResult);
    //    Assert.NotEqual((uint)TxResultCode.Success, executeContractResult?.Code);

    //    Assert.Contains("failed to execute message; message index: 0", executeContractResult.RawLog);
    //    Assert.Contains("insufficient funds: balance=1, required=2", executeContractResult.RawLog);
    //}

    //[Fact, TestPriority(5)]
    //public async void QueryContract_Response_ReturnsExpectedResult()
    //{
    //    Assert.True(!String.IsNullOrWhiteSpace(_context.ContractSnip20IbcContractAddress));

    //    // Arrange


    //    // Act
    //    var queryContractResult = await _context.SecretClient.Query.Compute
    //        .QueryContract<object>(
    //            contractAddress: _context.ContractSnip20IbcContractAddress, 
    //            queryMsg: new { 
    //                token_info = new { }
    //            }, 
    //            codeHash: _context.ContractSnip20IbcCodeHash
    //            );

    //    // Assert
    //    var expectedResult = JsonConvert.SerializeObject(
    //        new
    //        {
    //            token_info = new
    //            {
    //                name = "Secret SCRT",
    //                symbol = "SSCRT",
    //                decimals = 6,
    //                total_supply = "1"
    //            }
    //        },
    //        Formatting.Indented);

    //    Assert.NotNull(executeContractResult);
    //    var responseAsString = Convert.ToString(queryContractResult.Response);
    //    Assert.True(!String.IsNullOrEmpty(responseAsString));
    //    Assert.Equal(expectedResult, responseAsString);
    //}

    //[Fact, TestPriority(6)]
    //public async void QueryContractWithWrongViewingKey_Response_ReturnsError()
    //{
    //    Assert.True(!String.IsNullOrWhiteSpace(_context.ContractSnip20IbcContractAddress));

    //    // Arrange

    //    // Act
    //    var queryContractResult = await _context.SecretClient.Query.Compute
    //        .QueryContract<object>(
    //            contractAddress: _context.ContractSnip20IbcContractAddress,
    //            queryMsg: new
    //            {
    //                balance = new
    //                {
    //                    address = _context.Wallet.Address,
    //                    key = "wrong",
    //                }
    //            },
    //            codeHash: _context.ContractSnip20IbcCodeHash
    //            );

    //    // Assert
    //    Assert.NotNull(queryContractResult);
    //    Assert.Contains("Wrong viewing key for this address or viewing key not set", queryContractResult.RawResponse);
    //}

    [Fact, TestPriority(7)]
    public async void QueryContractWithWrongMsgType_Response_ReturnsError()
    {
        Assert.True(!String.IsNullOrWhiteSpace(_context.ContractSnip20IbcContractAddress));

        // Arrange

        // Act
        var queryContractResult = await _context.SecretClient.Query.Compute
            .QueryContract<object>(
                contractAddress: _context.ContractSnip20IbcContractAddress,
                queryMsg: new
                {
                    non_existent_query = new{}
                },
                codeHash: _context.ContractSnip20IbcCodeHash
                );

        // Assert
        Assert.NotNull(queryContractResult);        
        Assert.NotNull(queryContractResult.Exception);
        Assert.True(queryContractResult.HasError);
        var smartContractException = queryContractResult.Exception as SmartContractException;
        Assert.NotNull(smartContractException);
        Assert.Contains("\"parse_err\":{\"msg\":\"unknown variant `non_existent_query`", smartContractException.RawResult);
    }

    // Arrange

    // Act

    // Assert
}