using System.Text;
using Xunit.Abstractions;

namespace SecretNET.Tests;

public class UnitTests : IClassFixture<TestContext>
{
    TestContext _context;
    readonly ITestOutputHelper _output;

    public UnitTests(TestContext context, ITestOutputHelper output)
    {
        _output = output;
        _context = context;
    }

    [Fact]
    public void SecretClient_Constructor_SetClientPropsFromParams()
    {
        // Arrange
        var gprcUrl = "https://scrt.net";
        var chainId = "secret-net";

        var wallet = _context.Wallet;
        var encryptionKey = _context.WalletsTxEncryptionKey[wallet.Address];

        var createClientOptions = new CreateClientOptions(gprcUrl, chainId, wallet);

        // Act
        var secretClient = new SecretNetworkClient(createClientOptions);

        // Assert
        Assert.Equal(secretClient.GrpcWebUrl, gprcUrl);
        Assert.Equal(secretClient.ChainId, chainId);
        Assert.Equal(secretClient.Wallet, wallet);
        Assert.Equal(secretClient.EncryptionSeed, encryptionKey);        
    }

    [Fact]
    public void SecretClient_Constructor_UsedProvidedEncryptionUtils()
    {
        // Arrange
        var gprcUrl = "https://scrt.net";
        var chainId = "secret-net";

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
    public void SecretClient_SetWallet_ChangesEncryptionSeed()
    {
        // Arrange
        var gprcUrl = "https://scrt.net";
        var chainId = "secret-net";

        var wallet = _context.Wallet;
        var encryptionKey = _context.WalletsTxEncryptionKey[wallet.Address];

        var randomWalletIndex = Random.Shared.Next(1, _context.Wallets.Count()-1);
        var randomWallet = _context.Wallets[randomWalletIndex];
        var randomWalletEncryptionKey = _context.WalletsTxEncryptionKey[randomWallet.Address];

        var createClientOptions = new CreateClientOptions(gprcUrl, chainId, wallet);
        var secretClient = new SecretNetworkClient(createClientOptions);

        // Act
        secretClient.Wallet = randomWallet;

        // Assert
        Assert.Equal(secretClient.EncryptionSeed, randomWalletEncryptionKey);
    }
}
