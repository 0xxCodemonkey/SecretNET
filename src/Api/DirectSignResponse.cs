namespace SecretNET.Api;

public class DirectSignResponse
{
    public SignDoc signed { get; set; }
    public StdSignature Signature { get; set; }
}
