using Cosmos.Bank.V1Beta1;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Newtonsoft.Json;
using SecretNET.AccessControl;
using SecretNET.Api;
using SecretNET.Tests.Common;
using SecretNET.Tx;
using Xunit.Abstractions;

namespace SecretNET.Tests;

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

    #region StoreCode / instantiate contract

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

    [Fact, TestPriority(3)]
    public async void Instantiate_WithMissingMsgParameter_ReturnsVmError()
    {
        Assert.True(!String.IsNullOrWhiteSpace(_context.ContractSnip20IbcCodeHash));

        // Arrange
        var msgInitContract = new MsgInstantiateContract(
                            codeId: _context.ContractSnip20IbcCodeId,
                            label: $"SecretNET Test {_context.ContractSnip20IbcCodeId} Err",
                            initMsg: new
                            {
                                //name = "Secret SCRT", => mandatory
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

        // simulate would fail
        var txOptions = new TxOptions()
        {
            SkipSimulate = true,
            GasLimit = 5_000_000
        };

        // Act
        var initContractResponse = await _context.SecretClient.Tx.Compute.InstantiateContract(msgInitContract, txOptions);


        // Assert
        Assert.NotNull(initContractResponse);
        Assert.NotEqual((uint)TxResultCode.Success, initContractResponse?.Code);
        Assert.Contains("missing field `name`", initContractResponse.RawLog);

    }

    #endregion

    #region Execute contract

    [Fact, TestPriority(4)]
    public async void ExecuteContract_Response_ReturnsResult()
    {
        Assert.True(!String.IsNullOrWhiteSpace(_context.ContractSnip20IbcContractAddress));

        // Arrange
        var executeMsg = new MsgExecuteContract(
            contractAddress: _context.ContractSnip20IbcContractAddress,
            msg: new
            {
                set_viewing_key = new
                {
                    key = _context.ViewingKey
                }
            },
            codeHash: _context.ContractSnip20IbcCodeHash);

        // Act
        var executeContractResult = await _context.SecretClient.Tx.Compute.ExecuteContract<string>(executeMsg);

        // Assert
        Assert.NotNull(executeContractResult);
        Assert.Equal((uint)TxResultCode.Success, executeContractResult?.Code);
        Assert.NotNull(executeContractResult?.Response);
        var resultAsString = Convert.ToString(executeContractResult?.Response);
        _output.WriteLine($"ResultAsString: {resultAsString}");
        Assert.True(!String.IsNullOrWhiteSpace(resultAsString));
        Assert.Contains("{\"set_viewing_key\":{\"status\":\"success\"}}                                                                                                                                                                                                                      ", resultAsString);

    }

    [Fact, TestPriority(5)]
    public async void ExecuteContract_WithWrongParameter_ReturnsError()
    {
        Assert.True(!String.IsNullOrWhiteSpace(_context.ContractSnip20IbcContractAddress));

        // Arrange
        var executeMsg = new MsgExecuteContract(
            contractAddress: _context.ContractSnip20IbcContractAddress,
            msg: new
            {
                transfer = new
                {
                    recipient = _context.Wallet.Address,
                    amount = "2",
                }
            },
            codeHash: _context.ContractSnip20IbcCodeHash);

        // simulate would fail
        var txOptions = new TxOptions()
        {
            SkipSimulate = true,
            GasLimit = 5_000_000
        };

        // Act
        var executeContractResult = await _context.SecretClient.Tx.Compute.ExecuteContract<string>(executeMsg, txOptions);

        // Assert
        Assert.NotNull(executeContractResult);
        Assert.NotEqual((uint)TxResultCode.Success, executeContractResult?.Code);

        Assert.Contains("failed to execute message; message index: 0", executeContractResult.RawLog);
        Assert.Contains("insufficient funds: balance=1, required=2", executeContractResult.RawLog);
    } 

    #endregion

    #region Query contract

    [Fact, TestPriority(6)]
    public async void Query_GetTxWithError_ReturnsError()
    {
        Assert.True(!String.IsNullOrWhiteSpace(_context.ContractSnip20IbcContractAddress));

        // Arrange
        var executeMsg = new MsgExecuteContract(
            contractAddress: _context.ContractSnip20IbcContractAddress,
            msg: new
            {
                transfer = new
                {
                    recipient = _context.Wallet.Address,
                    amount = "2",
                }
            },
            codeHash: _context.ContractSnip20IbcCodeHash);

        // simulate would fail
        var txOptions = new TxOptions()
        {
            SkipSimulate = true,
            GasLimit = 5_000_000
        };

        // Act
        var executeContractResult = await _context.SecretClient.Tx.Compute.ExecuteContract<string>(executeMsg, txOptions);

        var getTxResult = await _context.SecretClient.Query.GetTx(executeContractResult.Txhash);

        // Assert
        Assert.NotNull(getTxResult);
        Assert.NotEqual((uint)TxResultCode.Success, getTxResult?.Code);
        Assert.NotNull(getTxResult.RawLog);
        Assert.Contains("failed to execute message; message index: 0", getTxResult.RawLog);
        Assert.Contains("insufficient funds: balance=1, required=2", getTxResult.RawLog);
    }

    [Fact, TestPriority(7)]
    public async void QueryContract_Response_ReturnsExpectedResult()
    {
        Assert.True(!String.IsNullOrWhiteSpace(_context.ContractSnip20IbcContractAddress));

        // Arrange

        // Act
        var queryContractResult = await _context.SecretClient.Query.Compute
            .QueryContract<object>(
                contractAddress: _context.ContractSnip20IbcContractAddress,
                queryMsg: new
                {
                    token_info = new { }
                },
                codeHash: _context.ContractSnip20IbcCodeHash
                );

        // Assert
        var expectedResult = JsonConvert.SerializeObject(
            new
            {
                token_info = new
                {
                    name = "Secret SCRT",
                    symbol = "SSCRT",
                    decimals = 6,
                    total_supply = "1"
                }
            },
            Formatting.Indented);

        Assert.NotNull(queryContractResult);
        var responseAsString = Convert.ToString(queryContractResult.Response);
        Assert.True(!String.IsNullOrEmpty(responseAsString));
        Assert.Equal(expectedResult, responseAsString);
    }

    [Fact, TestPriority(8)]
    public async void QueryContract_WithoutCodeHash_ReturnsExpectedResult()
    {
        Assert.True(!String.IsNullOrWhiteSpace(_context.ContractSnip20IbcContractAddress));

        // Arrange


        // Act
        var queryContractResult = await _context.SecretClient.Query.Compute
            .QueryContract<object>(
                contractAddress: _context.ContractSnip20IbcContractAddress,
                queryMsg: new
                {
                    token_info = new { }
                },
                codeHash: null
                );

        // Assert
        var expectedResult = JsonConvert.SerializeObject(
            new
            {
                token_info = new
                {
                    name = "Secret SCRT",
                    symbol = "SSCRT",
                    decimals = 6,
                    total_supply = "1"
                }
            },
            Formatting.Indented);

        Assert.NotNull(queryContractResult);
        var responseAsString = Convert.ToString(queryContractResult.Response);
        Assert.True(!String.IsNullOrEmpty(responseAsString));
        Assert.Equal(expectedResult, responseAsString);
    }

    [Fact, TestPriority(9)]
    public async void QueryContract_WithWrongMsgType_ReturnsError()
    {
        Assert.True(!String.IsNullOrWhiteSpace(_context.ContractSnip20IbcContractAddress));

        // Arrange

        // Act
        var queryContractResult = await _context.SecretClient.Query.Compute
            .QueryContract<object>(
                contractAddress: _context.ContractSnip20IbcContractAddress,
                queryMsg: new
                {
                    non_existent_query = new { }
                },
                codeHash: _context.ContractSnip20IbcCodeHash
                );

        // Assert
        Assert.NotNull(queryContractResult);
        Assert.NotNull(queryContractResult.Exception);
        Assert.True(queryContractResult.HasError);
        var smartContractException = queryContractResult.Exception as SmartContractException;
        Assert.NotNull(smartContractException);
        Assert.Contains("\"parse_err\":{\"msg\":\"unknown variant `non_existent_query`", queryContractResult.RawResponse);
    }

    #endregion

    #region Permit / ViewingKey

    [Fact, TestPriority(10)]
    public async void QueryContract_WithViewingKey_ReturnsBalance()
    {
        Assert.True(!String.IsNullOrWhiteSpace(_context.ContractSnip20IbcContractAddress));

        // Arrange

        // Act
        var queryContractResult = await _context.SecretClient.Query.Compute
            .QueryContract<object>(
                contractAddress: _context.ContractSnip20IbcContractAddress,
                queryMsg: new
                {
                    balance = new
                    {
                        address = _context.Wallet.Address,
                        key = _context.ViewingKey,
                    }
                },
                codeHash: _context.ContractSnip20IbcCodeHash
                );

        // Assert
        Assert.NotNull(queryContractResult);
        Assert.Contains("{\"balance\":{\"amount\":\"1\"}}", queryContractResult.RawResponse);
    }

    [Fact, TestPriority(11)]
    public async void QueryContract_WithWrongViewingKey_ReturnsError()
    {
        Assert.True(!String.IsNullOrWhiteSpace(_context.ContractSnip20IbcContractAddress));

        // Arrange

        // Act
        var queryContractResult = await _context.SecretClient.Query.Compute
            .QueryContract<object>(
                contractAddress: _context.ContractSnip20IbcContractAddress,
                queryMsg: new
                {
                    balance = new
                    {
                        address = _context.Wallet.Address,
                        key = "wrong",
                    }
                },
                codeHash: _context.ContractSnip20IbcCodeHash
                );

        // Assert
        Assert.NotNull(queryContractResult);
        Assert.Contains("Wrong viewing key for this address or viewing key not set", queryContractResult.RawResponse);
    }    

    [Fact, TestPriority(12)]
    public async void QueryContract_WithPermit_ReturnsBalance()
    {
        Assert.True(!String.IsNullOrWhiteSpace(_context.ContractSnip20IbcContractAddress));

        // Arrange
        var permit = await _context.SecretClient.Permit.Sign(
            owner: _context.Wallet.Address,
            chainId: _context.SecretClient.ChainId,
            permitName: "test",
            allowedContracts: new string[] { _context.ContractSnip20IbcContractAddress },
            permissions: new PermissionType[] {
                PermissionType.Balance
            });

        // Act
        var queryContractResult = await _context.SecretClient.Query.Compute
            .QueryContract<object>(
                contractAddress: _context.ContractSnip20IbcContractAddress,
                queryMsg: new
                {
                    with_permit = new
                    {
                        permit = permit,
                        query = new
                        {
                            balance = new
                            {
                                address = _context.Wallet.Address,
                                key = _context.ViewingKey,
                            }
                        }
                    }
                },
                codeHash: _context.ContractSnip20IbcCodeHash
                );

        // Assert
        Assert.NotNull(queryContractResult);
        Assert.Contains("{\"balance\":{\"amount\":\"1\"}}", queryContractResult.RawResponse);
    }

    [Fact, TestPriority(13)]
    public async void QueryContract_WithWrongPermit_ReturnsError()
    {
        Assert.True(!String.IsNullOrWhiteSpace(_context.ContractSnip20IbcContractAddress));

        // Arrange
        var permit = await _context.SecretClient.Permit.Sign(
            owner: _context.Wallet.Address,
            chainId: _context.SecretClient.ChainId,
            permitName: "test",
            allowedContracts: new string[] { "secret1234567" },
            permissions: new PermissionType[] {
                PermissionType.Balance
            });

        // Act
        var queryContractResult = await _context.SecretClient.Query.Compute
            .QueryContract<object>(
                contractAddress: _context.ContractSnip20IbcContractAddress,
                queryMsg: new
                {
                    with_permit = new
                    {
                        permit = permit,
                        query = new
                        {
                            balance = new
                            {
                                address = _context.Wallet.Address,
                                key = _context.ViewingKey,
                            }
                        }
                    }
                },
                codeHash: _context.ContractSnip20IbcCodeHash
                );

        // Assert
        Assert.NotNull(queryContractResult);
        Assert.Contains("{\"generic_err\":{\"msg\":\"Permit doesn't apply to token", queryContractResult.RawResponse);
    }

    [Fact, TestPriority(1)]
    public async void Permit_VerifyPermit_ReturnsTrue()
    {
        // Arrange
        var permit = await _context.SecretClient.Permit.Sign(
            owner: _context.Wallet.Address,
            chainId: _context.SecretClient.ChainId,
            permitName: "test",
            allowedContracts: new string[] { _context.ContractSnip20IbcContractAddress },
            permissions: new PermissionType[] {
                PermissionType.Balance
            });

        // Act
        var verifyResult = _context.SecretClient.Permit.Verify(
                          permit,
                          _context.Wallet.Address,
                          _context.ContractSnip20IbcContractAddress,
                          new PermissionType[] { PermissionType.Balance }
                        );

        // Assert
        Assert.True(verifyResult);
    }

    [Fact, TestPriority(1)]
    public async void Permit_VerifyPermitWrongContractWithException_ThrowsContractNotInPermitException()
    {
        // Arrange
        var permit = await _context.SecretClient.Permit.Sign(
            owner: _context.Wallet.Address,
            chainId: _context.SecretClient.ChainId,
            permitName: "test",
            allowedContracts: new string[] { _context.ContractSnip20IbcContractAddress },
            permissions: new PermissionType[] {
                PermissionType.Balance
            });

        // Act / Assert
        Assert.Throws<ContractNotInPermitException>(
            () => _context.SecretClient.Permit.Verify(
                          permit,
                          _context.Wallet.Address,
                          "abcd",
                          new PermissionType[] { PermissionType.Balance }
                        ));

    }

    [Fact, TestPriority(1)]
    public async void Permit_VerifyPermitWrongPermissionsWithException_ThrowsPermissionNotInPermitException()
    {
        // Arrange
        var permit = await _context.SecretClient.Permit.Sign(
            owner: _context.Wallet.Address,
            chainId: _context.SecretClient.ChainId,
            permitName: "test",
            allowedContracts: new string[] { _context.ContractSnip20IbcContractAddress },
            permissions: new PermissionType[] {
                PermissionType.Balance
            });

        // Act / Assert
        Assert.Throws<PermissionNotInPermitException>(
            () => _context.SecretClient.Permit.Verify(
                          permit,
                          _context.Wallet.Address,
                          _context.ContractSnip20IbcContractAddress,
                          new PermissionType[] { PermissionType.Owner }
                        ));

    } 

    #endregion


    // Arrange

    // Act

    // Assert
}