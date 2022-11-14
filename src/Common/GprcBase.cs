namespace SecretNET.Common;

public abstract class GprcBase
{
    protected GrpcChannel grpcChannel { get; private set; }

    protected CallInvoker? grpcMessageInterceptor { get; private set; }

    internal readonly ISecretNetworkClient secretClient;

    internal IWallet? Wallet { get { return secretClient.Wallet; } }

    internal string? WalletAddress { get { return secretClient.WalletAddress; } }

    internal async Task<SecretEncryptionUtils> GetEncryptionUtils()
    {
        return await secretClient.GetEncryptionUtils();
    }

    internal GprcBase(ISecretNetworkClient secretNetworkClient, GrpcChannel grpcChannel, CallInvoker? grpcMessageInterceptor)
    {
        this.secretClient = secretNetworkClient;
        this.grpcChannel = grpcChannel;
        this.grpcMessageInterceptor = grpcMessageInterceptor;
    }

    internal byte[] AddressToBytes(string contractAddress)
    {
        return SecretNetworkClient.AddressToBytes(contractAddress);
    }

}
