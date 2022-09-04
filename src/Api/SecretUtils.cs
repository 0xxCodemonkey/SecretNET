using System.IO.Compression;
using System.Text.RegularExpressions;

namespace SecretNET.Api;

public static class SecretUtils
{
    private static readonly Regex _isBase64RegEx = new Regex("^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)?$", RegexOptions.Compiled | RegexOptions.Singleline);

    public static ByteString GetByteStringFromBase64(this byte[] data)
    {
        return ByteString.FromBase64(Convert.ToBase64String(data));
    }

    public static bool IsBase64String(this string base64String)
    {
        if (!String.IsNullOrEmpty(base64String) && base64String.Length > 3)
        {
            return _isBase64RegEx.IsMatch(base64String);
        }
        return false;
    }

    public static async Task<bool> IsGzip(this byte[] data)
    {
        var result = false;
        // With any GZip format stream you are guarnateed
        // First two bytes: 1f (31), 8b (139)
        if (data.Length >= 2 && data[0] == 31 && data[1] == 139)
        {
            MemoryStream ms = null;
            GZipStream compressedzipStream = null;
            MemoryStream outBuffer = new MemoryStream();

            try
            {
                ms = new MemoryStream(data);
                compressedzipStream = new GZipStream(ms, CompressionMode.Decompress);

                byte[] block = new byte[1024 * 16];
                while (true)
                {
                    int bytesRead = await compressedzipStream.ReadAsync(block, 0, block.Length);
                    if (bytesRead <= 0)
                    {
                        break;
                    }
                    else
                    {
                        outBuffer.Write(block, 0, bytesRead);
                    }
                }
                result = true;
            }
            finally
            {
                if (null != compressedzipStream) compressedzipStream.Close();
                if (null != ms) ms.Close();                    
            }
        }
        return result;
    }
    
    public static async Task<byte[]> CompressGzip(this byte[] data, CompressionLevel compressionLevel = CompressionLevel.Optimal)
    {
        await using var input = new MemoryStream(data);
        await using var output = new MemoryStream();

        using var compressedStream = new GZipStream(output, compressionLevel);

        await input.CopyToAsync(output);

        var result = output.ToArray();
        return result;

    }

    public static bool IsProtoType(this string typeUrl, string matchType)
    {
        if (!String.IsNullOrEmpty(typeUrl) && !String.IsNullOrEmpty(matchType))
        {
            return typeUrl.TrimStart('/').Equals(matchType, StringComparison.InvariantCultureIgnoreCase);
        }
        return false;
    }

    public static bool IsProtoType(this IMessage msg, string matchType)
    {
        if (!String.IsNullOrEmpty(msg?.Descriptor?.FullName))
        {
            return IsProtoType(msg.Descriptor.FullName, matchType);
        }
        return false;
    }
}
