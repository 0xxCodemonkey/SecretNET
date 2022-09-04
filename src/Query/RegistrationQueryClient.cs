using Secret.Registration.V1Beta1;

namespace SecretNET.Query;

/// <summary>
/// Class RegistrationQueryClient.
/// Implements the <see cref="SecretNET.Common.GprcBase" />
/// Implements the <see cref="SecretNET.Query.IRegistrationQueryClient" />
/// </summary>
/// <seealso cref="SecretNET.Common.GprcBase" />
/// <seealso cref="SecretNET.Query.IRegistrationQueryClient" />
public class RegistrationQueryClient : GprcBase, IRegistrationQueryClient
{
    private Secret.Registration.V1Beta1.Query.QueryClient? _queryClient;

    internal RegistrationQueryClient(ISecretNetworkClient secretNetworkClient, GrpcChannel grpcChannel, CallInvoker? grpcMessageInterceptor) : base(secretNetworkClient, grpcChannel, grpcMessageInterceptor)
    {
    }

    private Secret.Registration.V1Beta1.Query.QueryClient client
    {
        get
        {
            if (_queryClient == null)
            {
                _queryClient = grpcMessageInterceptor != null ? 
                    new Secret.Registration.V1Beta1.Query.QueryClient(grpcMessageInterceptor) : 
                    new Secret.Registration.V1Beta1.Query.QueryClient(grpcChannel);
            }
            return _queryClient;
        }
    }

    /// <summary>
    /// Returns the key used for transactions.
    /// </summary>
    /// <returns>Key.</returns>
    public async Task<Key> TxKey()
    {
        var result = await client.TxKeyAsync(new Empty());
        return result;
    }

    /// <summary>
    /// Returns the key used for registration.
    /// </summary>
    /// <returns>Secret.Registration.V1Beta1.Key.</returns>
    public async Task<Key> RegistrationKey()
    {
        var result = await client.RegistrationKeyAsync(new Empty());
        return result;
    }

    /// <summary>
    /// Encrypteds the seed.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryEncryptedSeedResponse.</returns>
    public async Task<QueryEncryptedSeedResponse> EncryptedSeed(QueryEncryptedSeedRequest request)
    {
        var result = await client.EncryptedSeedAsync(request);
        return result;
    }
}
