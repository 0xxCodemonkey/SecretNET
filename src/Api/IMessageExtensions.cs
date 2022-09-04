namespace SecretNET.Api;

public static class IMessageExtensions
{    
    public static byte[] Encode(this IMessage message)
    {
        using var memoryStream = new MemoryStream();
        message.WriteTo(memoryStream);
        var messageBytes = memoryStream.ToArray();

        return messageBytes;
    }

    public static byte[] EncodeBase64(this IMessage message)
    {
        var msgBytes = message.Encode();
        string inBase64 = Convert.ToBase64String(msgBytes);
        byte[] backToBytes = Convert.FromBase64String(inBase64);

        return backToBytes;
    }

}
