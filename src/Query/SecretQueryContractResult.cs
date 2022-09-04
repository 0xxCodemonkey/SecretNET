namespace SecretNET.Query;

public class SecretQueryContractResult<T>
{
    public T Response { get; set; }

    public string RawResponse { get; set; }

    public Exception Exception { get; internal set; }

    public bool HasError
    {
        get
        {
            return Exception != null;
        }
    }
    public SecretQueryContractResult(T result)
    {
        Response = result;
    }

    public SecretQueryContractResult(Exception exception)
    {
        Exception = exception;
    }
}
