namespace SecretNET.Query
{
    public interface IRegistrationQueryClient
    {
        Task<Secret.Registration.V1Beta1.Key> TxKey();
    }
}