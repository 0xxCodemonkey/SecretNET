using SecretNET.Common.Storage;
using System.Diagnostics;
using Xunit.Abstractions;

namespace SecretNET.Tests;

// https://xunit.net/docs/shared-context

public class TestContext : IDisposable
{
    public Wallet? Wallet { get { return Wallets?[0]; } }
    public SecretNetworkClient? SecretClient { get; private set; }

    public List<Wallet> Wallets { get; set; } = new List<Wallet>();


    // snip20-ibc
    public ulong ContractSnip20IbcCodeId = 0;
    public string ContractSnip20IbcCodeHash = null;
    public string ContractSnip20IbcContractAddress = null;

    // proposal
    public ulong TextProposalId = 0;
    public ulong CommunityPoolSpendProposalId = 0;
    public ulong ParameterChangeProposalId = 0;
    public ulong SoftwareUpgradeProposalId = 0;
    public ulong CancelSoftwareUpgradeProposalId = 0;


    /// <summary>
    /// Initializes a new instance of the <see cref="TestContext"/> class.
    /// </summary>
    public TestContext()
    {
        Init();
    }

    private async void Init()
    {
        var storageProvider = new InMemoryOnlyStorage();
        var createWalletOptions = new CreateWalletOptions(storageProvider);

        //TODO: use localsecret if available

        // SecretNET unfortunately cannot be connected to localsecret (Docker) yet, so we have to test against testnet and need to use a funded wallet.
        // use ** your own Mnemonic here *** in this test and fund the wallet via https://faucet.pulsar.scrttestnet.com/
        var myTestMnemonic = "film moral brass either category addict grid lobster suffer find fish attitude abandon lumber pottery task local spawn drastic enforce time sausage humor blouse";
        var wallet = await Wallet.Create(myTestMnemonic, options: createWalletOptions);      
        
        Debug.WriteLine($"Mainaccount: " + wallet.Address);
        Wallets.Add(wallet);

        // Generate a bunch of accounts because tx.staking tests require creating a bunch of validators
        Debug.WriteLine("Please fund subaccouts via https://faucet.pulsar.scrttestnet.com/");
        /*
        Mainaccount: secret1j8u7n4v93kjyqa7wzzrgjule8gh4adde36mnwd
        Subaccount 1: secret188lwg986lpk6ypm9ste4u934eavtje9yt5xlg0
        Subaccount 2: secret1ftlc6p7rqf3p2wfnwrptrfgg84txtclmjmpy5s
        Subaccount 3: secret18949w02l8tzwpvpefp4mktjq8tezakdr3er6wg
        Subaccount 4: secret1d8w4hac2h3den3xtvqxygvzseua3jx79tspgsc
        Subaccount 5: secret1ndyyg83cr3f89dqjkz674tz3xsjp7gx7kdqrdc
        Subaccount 6: secret16pdd4rs6zpgp46jsnhhhlt5dq398sp4ja98ju6
        Subaccount 7: secret1z6pvfw68nsyqyvgcaxlnqkpxh4l2kye2g47pxg
        Subaccount 8: secret1av08emx94a8x9w64uw50vgk235pqpt6sffhnyz
        Subaccount 9: secret16gn729wrj7ey4tl8fvzsf90ndyhuefkn0r36g9
        Subaccount 10: secret1medw6rsfa9ycv7kazq45ly9wfzd7st5zzsdac4
        Subaccount 11: secret18cnwjjlks7s62w93ep5w84kqgxwtylp4crh7x2
        Subaccount 12: secret1s4kcf0vzkr2uwg3g0v58kqqxrhngctye2f4hgl
        Subaccount 13: secret1x8gts3jueft9uq6z7xkjvf82a9xdrrp5mqx6u5
        Subaccount 14: secret169g2fcue7ghxa5r4qll4qy7k7ksaf0ykfh02tz
        Subaccount 15: secret14nl7klkutgv7ht6mxn6dyxkh20cd6yta55lxxv
        Subaccount 16: secret18aker03ar6ffy6knx4jn28n7l5uaykmnscx4e0
        Subaccount 17: secret1htmz8gcnfst9dgvkasyrtgtp99uf54tc7zqmv0
        Subaccount 18: secret18p83au7f80kgt308fvpw4crj975y03vme9q04a
        Subaccount 19: secret170utevy5uv8ang0v3d6ey5xu736gxgjy4v479p
        */
        for (int i = 1; i < 20; i++)
        {
            var subaccount = await wallet.GetSubaccount((byte)i);            
            Debug.WriteLine($"Subaccount {i}: " + subaccount.Address);
            Wallets.Add(subaccount);
        }

        var gprcUrl = "https://grpc.testnet.secretsaturn.net"; // get from https://github.com/scrtlabs/api-registry
        var chainId = "pulsar-2";

        var createClientOptions = new CreateClientOptions(gprcUrl, chainId, wallet)
        {
            AlwaysSimulateTransactions = true, // WARNING: On mainnet it's recommended to not simulate every transaction as this can burden your node provider. 
        };
        SecretClient = new SecretNetworkClient(createClientOptions);

        // Adjust TxOptionsDefaults if
    }

    public void Dispose()
    {
        // ... clean up
    }
}
