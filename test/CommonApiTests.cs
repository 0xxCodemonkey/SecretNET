using Cosmos.Bank.V1Beta1;
using Cosmos.Gov.V1Beta1;
using Docker.DotNet.Models;
using Newtonsoft.Json;
using SecretNET.Api;
using SecretNET.Tests.Common;
using SecretNET.Tx;
using Xunit.Abstractions;

namespace SecretNET.Tests;


[TestCaseOrderer("SecretNET.Tests.Common.PriorityOrderer", "SecretNET.Tests")]
public class CommonApiTests : IClassFixture<TestContext>
{
    TestContext _context;
    readonly ITestOutputHelper _output;

    public CommonApiTests(TestContext context, ITestOutputHelper output)
    {
        _output = output;
        _context = context;
    }

    #region Query.Auth / Account

    //TODO: query.auth.accounts() => fresh localsecret is needed for a test

    [Fact, TestPriority(1)]
    public async void QueryAuth_Account_ReturnsAccount()
    {
        // Arrange

        // Act
        var accountResponse = await _context.SecretClient.Query.Auth.Account(_context.Wallet.Address);

        // Assert
        Assert.NotNull(accountResponse);
        Assert.NotNull(accountResponse as Cosmos.Auth.V1Beta1.BaseAccount);
        Assert.True(((Cosmos.Auth.V1Beta1.BaseAccount)accountResponse).AccountNumber > 0);
        Assert.True(((Cosmos.Auth.V1Beta1.BaseAccount)accountResponse).Sequence >= 0);
    } 

    #endregion

    #region Bank

    [Fact, TestPriority(2)]
    public async void TxBank_Send_IncreaseAndDecreaseBalance()
    {
        // Arrange
        int sendAmount = 1;
        var account1_Before = await _context.SecretClient.Query.Bank.Balance(_context.Wallet.Address);
        var account2_Before = await _context.SecretClient.Query.Bank.Balance(_context.Wallets[2].Address);

        var msgSend = new MsgSend()
        {
            FromAddress = _context.Wallet.Address,
            ToAddress = _context.Wallets[2].Address,
        };
        msgSend.Amount.Add(new Cosmos.Base.V1Beta1.Coin()
        {
            Denom = "uscrt",
            Amount = sendAmount.ToString(),
        });

        // Act
        var simulateTx = await _context.SecretClient.Tx.Simulate(msgSend);

        var gasLimit = (ulong)((simulateTx?.GasInfo?.GasUsed).GetValueOrDefault() * 1.25f);
        var txOptions = new TxOptions()
        {
            GasLimit = gasLimit,
        };

        var sendResult = await _context.SecretClient.Tx.Bank.Send(msgSend, txOptions: txOptions);

        // Assert
        Assert.NotNull(account1_Before);
        Assert.NotNull(account2_Before);
        Assert.NotNull(sendResult);
        Assert.Equal((uint)TxResultCode.Success, sendResult?.Code);

        var account1_After = await _context.SecretClient.Query.Bank.Balance(_context.Wallet.Address);
        var account2_After = await _context.SecretClient.Query.Bank.Balance(_context.Wallets[2].Address);

        // increased balance
        Assert.True(int.Parse(account2_Before.Amount) + sendAmount == int.Parse(account2_After.Amount));

        // decreased balance (amount + gasFee)
        var x1 = (int)(ulong.Parse(account1_Before.Amount) - ulong.Parse(account1_After.Amount));
        var x2 = (int)(sendAmount + (gasLimit * 0.25f)); // estimated gas usage

        Assert.True(((uint)x2 - x1) < 1000); // estimated diff
    }

    [Fact, TestPriority(3)]
    public async void TxBank_MultiSend_IncreaseAndDecreaseBalance()
    {
        // Arrange
        int sendAmount = 1;
        var account1_Before = await _context.SecretClient.Query.Bank.Balance(_context.Wallet.Address);
        var account2_Before = await _context.SecretClient.Query.Bank.Balance(_context.Wallets[2].Address);
        var account3_Before = await _context.SecretClient.Query.Bank.Balance(_context.Wallets[3].Address);

        var msgMultiSend = new MsgMultiSend();

        var input = new Input() { Address = _context.Wallet.Address };
        input.Coins.Add(new Cosmos.Base.V1Beta1.Coin() { Denom = "uscrt", Amount = "2" });
        msgMultiSend.Inputs.Add(input);

        var output1 = new Output() { Address = _context.Wallets[2].Address };
        output1.Coins.Add(new Cosmos.Base.V1Beta1.Coin() { Denom = "uscrt", Amount = "1" });
        msgMultiSend.Outputs.Add(output1);

        var output2 = new Output() { Address = _context.Wallets[3].Address };
        output2.Coins.Add(new Cosmos.Base.V1Beta1.Coin() { Denom = "uscrt", Amount = "1" });
        msgMultiSend.Outputs.Add(output2);

        // Act
        var simulateTx = await _context.SecretClient.Tx.Simulate(msgMultiSend);

        var gasLimit = (ulong)((simulateTx?.GasInfo?.GasUsed).GetValueOrDefault() * 1.25f);
        var txOptions = new TxOptions()
        {
            GasLimit = gasLimit,
        };

        var sendResult = await _context.SecretClient.Tx.Bank.MultiSend(msgMultiSend, txOptions: txOptions);

        // Assert
        Assert.NotNull(account1_Before);
        Assert.NotNull(account2_Before);
        Assert.NotNull(account3_Before);
        Assert.NotNull(sendResult);
        Assert.Equal((uint)TxResultCode.Success, sendResult?.Code);

        var account1_After = await _context.SecretClient.Query.Bank.Balance(_context.Wallet.Address);
        var account2_After = await _context.SecretClient.Query.Bank.Balance(_context.Wallets[2].Address);
        var account3_After = await _context.SecretClient.Query.Bank.Balance(_context.Wallets[3].Address);

        Assert.True(int.Parse(account2_Before.Amount) + sendAmount == int.Parse(account2_After.Amount));
        Assert.True(int.Parse(account3_Before.Amount) + sendAmount == int.Parse(account3_After.Amount));

        var x1 = (int)(ulong.Parse(account1_Before.Amount) - ulong.Parse(account1_After.Amount));
        var x2 = (int)(sendAmount + (gasLimit * 0.25f)); // estimated gas usage

        Assert.True(((uint)x2 - x1) < 1000); // estimated diff
    } 
    #endregion

    #region Gov / Proposals

    private async Task<List<Cosmos.Gov.V1Beta1.Proposal>> GetAllProposals()
    {
        var result = await _context.SecretClient.Query.Gov.Proposals(new Cosmos.Gov.V1Beta1.QueryProposalsRequest()
        {
            ProposalStatus = Cosmos.Gov.V1Beta1.ProposalStatus.Unspecified,
            Voter = "",
            Depositor = ""
        });

        return result.Proposals.ToList();
    }

    [Fact, TestPriority(4)]
    public async void TxGov_SubmitTextProposal_IncreaseProposalCount()
    {
        // Arrange
        var proposalsBefore = await GetAllProposals();

        var msgSubmitProposal = new Tx.TextProposal(title: "Hi", description: "Hello", new Coin() { Amount = "1", Denom = "uscrt" });

        // Act
        var submitProposalResponse = await _context.SecretClient.Tx.Gov.SubmitProposal(msgSubmitProposal);

        // Assert
        Assert.NotNull(submitProposalResponse);
        Assert.Equal((uint)TxResultCode.Success, submitProposalResponse?.Code);
        Assert.NotNull(submitProposalResponse.Response);
        Assert.True(submitProposalResponse.Response.ProposalId >= 1);

        _context.TextProposalId = submitProposalResponse.Response.ProposalId;
        _output.WriteLine($"TextProposalId: {_context.TextProposalId}");

        var proposalsAfter = await GetAllProposals();
        Assert.True(proposalsBefore.Count() + 1 == proposalsAfter.Count());
    }

    /////secret.js tests => row 1409
    //[Fact, TestPriority(5)]
    //public async void TxGov_Vote_GetsCount()
    //{
    //    // Arrange
    //    var proposalsBefore = await GetAllProposals();

    //    var msgVote = new Cosmos.Gov.V1Beta1.MsgVote()
    //    {
    //        Voter = _context.Wallet.Address,
    //        ProposalId = _context.TextProposalId,
    //        Option = Cosmos.Gov.V1Beta1.VoteOption.Yes
    //    };

    //    // simulate would fail
    //    var txOptions = new TxOptions()
    //    {
    //        SkipSimulate = true,
    //        GasLimit = 5_000_000
    //    };

    //    // Act
    //    var voteResponse = await _context.SecretClient.Tx.Gov.Vote(msgVote, txOptions);

    //    // Assert
    //    Assert.NotNull(voteResponse);
    //    Assert.Equal((uint)TxResultCode.Success, voteResponse?.Code); // "failed to execute message; message index: 0: 26: inactive proposal"

    //    var proposalId = voteResponse.TryFindEventValue("proposal_id");
    //    var option = voteResponse.TryFindEventValue("option");
    //}

    //TODO: TxGov_VoteWeighted_GetsWeightedCount() => secret.js tests => row 1460

    [Fact, TestPriority(6)]
    public async void TxGov_Deposit_IncreaseAmount()
    {
        // Arrange
        var proposalsBefore = await GetAllProposals();

        var msgDeposit = new Cosmos.Gov.V1Beta1.MsgDeposit()
        {
            Depositor = _context.Wallet.Address,
            ProposalId = _context.TextProposalId,
        };
        msgDeposit.Amount.Add(new Cosmos.Base.V1Beta1.Coin() { Amount = "1", Denom = "uscrt" });

        // Act
        var depositResponse = await _context.SecretClient.Tx.Gov.Deposit(msgDeposit);

        // Assert
        Assert.NotNull(depositResponse);
        Assert.Equal((uint)TxResultCode.Success, depositResponse?.Code);

        var queryDepositResult = await _context.SecretClient.Query.Gov.Deposit(new Cosmos.Gov.V1Beta1.QueryDepositRequest()
        {
            Depositor = _context.Wallet.Address,
            ProposalId = _context.TextProposalId,
        });

        Assert.NotNull(queryDepositResult);
        Assert.True(Convert.ToString(queryDepositResult?.Deposit?.Amount[0]?.Amount) == "2");
    }

    [Fact, TestPriority(7)]
    public async void TxGov_SubmitCommunityPoolSpendProposal_IncreaseProposalCount()
    {
        // Arrange
        var proposalsBefore = await GetAllProposals();

        var msgSubmitProposal = new Tx.CommunityPoolSpendProposal(
            title: "Hi",
            description: "Hello",
            recipientAddress: _context.Wallets[2].Address,
            amount: new[] { new Coin() { Amount = "1", Denom = "uscrt" } },
            initialDeposit: new[] { new Coin() { Amount = "1", Denom = "uscrt" } });

        // Act
        var submitProposalResponse = await _context.SecretClient.Tx.Gov.SubmitProposal(msgSubmitProposal);

        // Assert
        Assert.NotNull(submitProposalResponse);
        Assert.Equal((uint)TxResultCode.Success, submitProposalResponse?.Code);
        Assert.NotNull(submitProposalResponse.Response);
        Assert.True(submitProposalResponse.Response.ProposalId >= 1);

        var proposalTypeFromLog = submitProposalResponse.TryFindEventValue("proposal_type");
        Assert.True("CommunityPoolSpend".Equals(proposalTypeFromLog, StringComparison.OrdinalIgnoreCase));

        _context.CommunityPoolSpendProposalId = submitProposalResponse.Response.ProposalId;
        _output.WriteLine($"ProposalId: {_context.CommunityPoolSpendProposalId}");

        var proposalsAfter = await GetAllProposals();
        Assert.True(proposalsBefore.Count() + 1 == proposalsAfter.Count());
    }

    [Fact, TestPriority(8)]
    public async void TxGov_SubmitParameterChangeProposal_IncreaseProposalCount()
    {
        // Arrange
        var proposalsBefore = await GetAllProposals();

        var msgSubmitProposal = new Tx.ParameterChangeProposal(
            title: "Hi",
            description: "YOLO",
            changes: new[] { new Cosmos.Params.V1Beta1.ParamChange()
                {
                    Subspace = "auth",
                    Key = "MaxMemoCharacters",
                    Value = "\"512\""
                }
            },
            initialDeposit: new[] { new Coin() { Amount = "1", Denom = "uscrt" } });

        // Act
        var submitProposalResponse = await _context.SecretClient.Tx.Gov.SubmitProposal(msgSubmitProposal);

        // Assert
        Assert.NotNull(submitProposalResponse);
        Assert.Equal((uint)TxResultCode.Success, submitProposalResponse?.Code);
        Assert.NotNull(submitProposalResponse.Response);
        Assert.True(submitProposalResponse.Response.ProposalId >= 1);

        var proposalTypeFromLog = submitProposalResponse.TryFindEventValue("proposal_type");
        Assert.True("ParameterChange".Equals(proposalTypeFromLog, StringComparison.OrdinalIgnoreCase));

        _context.ParameterChangeProposalId = submitProposalResponse.Response.ProposalId;
        _output.WriteLine($"ProposalId: {_context.ParameterChangeProposalId}");

        var proposalsAfter = await GetAllProposals();
        Assert.True(proposalsBefore.Count() + 1 == proposalsAfter.Count());
    }

    [Fact, TestPriority(9)]
    public async void TxGov_SubmitSoftwareUpgradeProposal_IncreaseProposalCount()
    {
        // Arrange
        var proposalsBefore = await GetAllProposals();

        var msgSubmitProposal = new Tx.SoftwareUpgradeProposal(
            title: "Hi",
            description: "YOLO",
            plan: new Cosmos.Upgrade.V1Beta1.Plan()
            {
                Name = "Shockwave!",
                Height = 1000000000,
                Info = "000000000019D6689C085AE165831E934FF763AE46A2A6C172B3F1B60A8CE26F"
            },
            initialDeposit: new[] { new Coin() { Amount = "1", Denom = "uscrt" } });

        // Act
        var submitProposalResponse = await _context.SecretClient.Tx.Gov.SubmitProposal(msgSubmitProposal);

        // Assert
        Assert.NotNull(submitProposalResponse);
        Assert.Equal((uint)TxResultCode.Success, submitProposalResponse?.Code);
        Assert.NotNull(submitProposalResponse.Response);
        Assert.True(submitProposalResponse.Response.ProposalId >= 1);

        var proposalTypeFromLog = submitProposalResponse.TryFindEventValue("proposal_type");
        Assert.True("SoftwareUpgrade".Equals(proposalTypeFromLog, StringComparison.OrdinalIgnoreCase));

        _context.SoftwareUpgradeProposalId = submitProposalResponse.Response.ProposalId;
        _output.WriteLine($"ProposalId: {_context.SoftwareUpgradeProposalId}");

        var proposalsAfter = await GetAllProposals();
        Assert.True(proposalsBefore.Count() + 1 == proposalsAfter.Count());
    }

    [Fact, TestPriority(10)]
    public async void TxGov_SubmitCancelSoftwareUpgradeProposal_IncreaseProposalCount()
    {
        // Arrange
        var proposalsBefore = await GetAllProposals();

        var msgSubmitProposal = new Tx.CancelSoftwareUpgradeProposal(
            title: "Hi",
            description: "YOLO",
            initialDeposit: new[] { new Coin() { Amount = "1", Denom = "uscrt" } });

        // Act
        var submitProposalResponse = await _context.SecretClient.Tx.Gov.SubmitProposal(msgSubmitProposal);

        // Assert
        Assert.NotNull(submitProposalResponse);
        Assert.Equal((uint)TxResultCode.Success, submitProposalResponse?.Code);
        Assert.NotNull(submitProposalResponse.Response);
        Assert.True(submitProposalResponse.Response.ProposalId >= 1);

        var proposalTypeFromLog = submitProposalResponse.TryFindEventValue("proposal_type");
        Assert.True("CancelSoftwareUpgrade".Equals(proposalTypeFromLog, StringComparison.OrdinalIgnoreCase));

        _context.CancelSoftwareUpgradeProposalId = submitProposalResponse.Response.ProposalId;
        _output.WriteLine($"ProposalId: {_context.CancelSoftwareUpgradeProposalId}");

        var proposalsAfter = await GetAllProposals();
        Assert.True(proposalsBefore.Count() + 1 == proposalsAfter.Count());
    }

    #endregion



    



    // Arrange

    // Act

    // Assert
}
