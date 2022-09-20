using Cosmos.Auth.V1Beta1;

namespace SecretNET.Query;

/// <summary>
/// AuthQuerier is the query interface for the x/auth module
/// </summary>
public class AuthQueryClient : GprcBase
{
    private Cosmos.Auth.V1Beta1.Query.QueryClient? _queryClient;

    internal AuthQueryClient(ISecretNetworkClient secretNetworkClient, GrpcChannel grpcChannel, CallInvoker? grpcMessageInterceptor) : base(secretNetworkClient, grpcChannel, grpcMessageInterceptor)
    {
    }

    private Cosmos.Auth.V1Beta1.Query.QueryClient client
    {
        get
        {
            if (_queryClient == null)
            {
                _queryClient = grpcMessageInterceptor != null ? 
                    new Cosmos.Auth.V1Beta1.Query.QueryClient(grpcMessageInterceptor) : 
                    new Cosmos.Auth.V1Beta1.Query.QueryClient(grpcChannel);
            }
            return _queryClient;
        }
    }

    /// <summary>
    /// Returns account details based on address.
    /// </summary>
    /// <param name="address"></param>
    /// <returns></returns>
    public async Task<IMessage> Account(string address)
    {
        var result = await client.AccountAsync(new Cosmos.Auth.V1Beta1.QueryAccountRequest { Address = address });

        if (result?.Account != null)
        {
            return AccountFromAny(result.Account);
        }
        return null;
    }

    /// <summary>
    /// Accountses this instance.
    /// </summary>
    /// <returns>List&lt;IMessage&gt;.</returns>
    public async Task<List<IMessage>> Accounts()
    {
        return await Accounts(new QueryAccountsRequest());
    }

    /// <summary>
    /// Returns all the existing accounts
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<List<IMessage>> Accounts(QueryAccountsRequest request)
    {
        var result = await client.AccountsAsync(request);

        if ((result?.Accounts?.Any()).GetValueOrDefault())
        {
            var accounts = new List<IMessage>();
            foreach (var a in result.Accounts)
            {
                var account = AccountFromAny(a);
                if (account != null)
                {
                    accounts.Add(account);
                }
            }
            return accounts;
        }
        return null;
    }

    /// <summary>
    /// Takes an `Any` encoded account from the chain and converts it into common `Account` types.
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private IMessage AccountFromAny(Any account)
    {
        if (account != null)
        {
            if (account.TypeUrl.IsProtoType("cosmos.auth.v1beta1.BaseAccount"))
            {
                return account.Unpack<BaseAccount>();
            }
            else if (account.TypeUrl.IsProtoType("cosmos.auth.v1beta1.ModuleAccount"))
            {
                return account.Unpack<ModuleAccount>();
            }
            else if (account.TypeUrl.IsProtoType("cosmos.vesting.v1beta1.BaseVestingAccount"))
            {
                return account.Unpack<Cosmos.Vesting.V1Beta1.BaseVestingAccount>();
            }
            else
            {
                throw new Exception($"Unsupported account type: '{account.TypeUrl}'");
            }
        }
        return null;
    }
}
