using System.Text;
using Xunit.Abstractions;

namespace SecretNET.Tests;

public class UnitTests : IClassFixture<TestContext>
{
    TestContext _context;
    readonly ITestOutputHelper _output;

    string gprcUrl = "https://scrt.net";
    string chainId = "pulsar-2";

    public UnitTests(TestContext context, ITestOutputHelper output)
    {
        _output = output;
        _context = context;
    }

    [Fact]
    public void SecretClient_Constructor_SetClientPropsFromParams()
    {
        // Arrange
        var wallet = _context.Wallet;
        var encryptionSeed = _context.WalletsTxEncryptionSeed[wallet.Address];

        var createClientOptions = new CreateClientOptions(gprcUrl, chainId, wallet);

        // Act
        var secretClient = new SecretNetworkClient(createClientOptions);
        var encryptionUtils = secretClient.GetEncryptionUtils();

        // Assert
        Assert.Equal(secretClient.GrpcWebUrl, gprcUrl);
        Assert.Equal(secretClient.ChainId, chainId);
        Assert.Equal(secretClient.Wallet, wallet);
        Assert.Equal(secretClient.EncryptionSeed, encryptionSeed);        
    }

    [Fact]
    public void SecretClient_Constructor_UsedProvidedEncryptionUtils()
    {
        // Arrange
        var wallet = _context.Wallet;

        var createClientOptions = new CreateClientOptions(gprcUrl, chainId, wallet)
        {
            EncryptionSeed = SecretNET.Crypto.Hashes.DoubleSHA256(Encoding.UTF8.GetBytes("SecureSeed")).ToBytes(), 
        };

        // Act
        var secretClient = new SecretNetworkClient(createClientOptions);

        // Assert
        Assert.Equal(secretClient.EncryptionSeed, createClientOptions.EncryptionSeed);
    }

    [Fact]
    public void SecretClient_SetWallet_OnReadOnlyClient()
    {
        // Arrange
        var wallet = _context.Wallet;
        var encryptionSeed = _context.WalletsTxEncryptionSeed[wallet.Address];

        var createClientOptions = new CreateClientOptions(gprcUrl, chainId);
        var secretClient = new SecretNetworkClient(createClientOptions);

        // Act
        secretClient.Wallet = wallet;
        var encryptionUtils = secretClient.GetEncryptionUtils();

        // Assert
        Assert.Equal(secretClient.EncryptionSeed, encryptionSeed);
    }


    [Fact]
    public void SecretClient_SetWallet_ChangesEncryptionSeed()
    {
        // Arrange
        var wallet = _context.Wallet;
        var encryptionSeed = _context.WalletsTxEncryptionSeed[wallet.Address];

        var randomWalletIndex = Random.Shared.Next(1, _context.Wallets.Count()-1);
        var randomWallet = _context.Wallets[randomWalletIndex];
        var randomWalletEncryptionKey = _context.WalletsTxEncryptionSeed[randomWallet.Address];

        var createClientOptions = new CreateClientOptions(gprcUrl, chainId, wallet);
        var secretClient = new SecretNetworkClient(createClientOptions);
        var encryptionUtils = secretClient.GetEncryptionUtils();

        // Act
        Assert.Equal(secretClient.EncryptionSeed, encryptionSeed);

        secretClient.Wallet = randomWallet;
        encryptionUtils = secretClient.GetEncryptionUtils();

        // Assert
        Assert.Equal(secretClient.EncryptionSeed, randomWalletEncryptionKey);
    }
}
