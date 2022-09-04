// ***********************************************************************
// <copyright>
//     Copy from https://github.com/MetacoSA/NBitcoin
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Security.Cryptography;

namespace SecretNET.Crypto;

public interface IRandom
{
    void GetBytes(byte[] output);

    void GetBytes(Span<byte> output);
}

public class RandomNumberGeneratorRandom : IRandom
{
    readonly RandomNumberGenerator _Instance;
    public RandomNumberGeneratorRandom()
    {
        _Instance = RandomNumberGenerator.Create();
    }
    #region IRandom Members

    public void GetBytes(byte[] output)
    {
        _Instance.GetBytes(output);
    }

    public void GetBytes(Span<byte> output)
    {
        _Instance.GetBytes(output);
    }

    #endregion
}

public partial class RandomUtils
{
    public static bool UseAdditionalEntropy { get; set; } = true;

    public static IRandom Random
    {
        get;
        set;
    }

    static RandomUtils()
    {
        //Thread safe http://msdn.microsoft.com/en-us/library/system.security.cryptography.rngcryptoserviceprovider(v=vs.110).aspx
        Random = new RandomNumberGeneratorRandom();
        AddEntropy(Guid.NewGuid().ToByteArray());
    }

    public static byte[] GetBytes(int length)
    {
        byte[] data = new byte[length];
        if (Random == null)
            throw new InvalidOperationException("You must set the RNG (RandomUtils.Random) before generating random numbers");
        //Random.GetBytes(data);
        data = RandomNumberGenerator.GetBytes(length);
        PushEntropy(data);
        return data;
    }

    public static void GetBytes(Span<byte> span)
    {
        if (Random == null)
            throw new InvalidOperationException("You must set the RNG (RandomUtils.Random) before generating random numbers");
        RandomNumberGenerator.Create().GetBytes(span);
        //Random.GetBytes(span);
    }

    private static void PushEntropy(byte[] data)
    {
        if (!UseAdditionalEntropy || additionalEntropy == null || data.Length == 0)
            return;
        int pos = entropyIndex;
        var entropy = additionalEntropy;
        for (int i = 0; i < data.Length; i++)
        {
            data[i] ^= entropy[pos % 32];
            pos++;
        }
        entropy = Hashes.SHA256(data);
        for (int i = 0; i < data.Length; i++)
        {
            data[i] ^= entropy[pos % 32];
            pos++;
        }
        entropyIndex = pos % 32;
    }

    static volatile byte[] additionalEntropy = null;
    static volatile int entropyIndex = 0;

    public static void AddEntropy(string data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        AddEntropy(Encoding.UTF8.GetBytes(data));
    }

    public static void AddEntropy(byte[] data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        var entropy = Hashes.SHA256(data);
        if (additionalEntropy == null)
            additionalEntropy = entropy;
        else
        {
            for (int i = 0; i < 32; i++)
            {
                additionalEntropy[i] ^= entropy[i];
            }
            additionalEntropy = Hashes.SHA256(additionalEntropy);
        }
    }

    public static uint256 GetUInt256()
    {
        return new uint256(GetBytes(32));
    }

    public static uint GetUInt32()
    {
        return BitConverter.ToUInt32(GetBytes(sizeof(uint)), 0);
    }

    public static int GetInt32()
    {
        return BitConverter.ToInt32(GetBytes(sizeof(int)), 0);
    }
    public static ulong GetUInt64()
    {
        return BitConverter.ToUInt64(GetBytes(sizeof(ulong)), 0);
    }

    public static long GetInt64()
    {
        return BitConverter.ToInt64(GetBytes(sizeof(long)), 0);
    }

    public static void GetBytes(byte[] output)
    {
        if (Random == null)
            throw new InvalidOperationException("You must set the RNG (RandomUtils.Random) before generating random numbers");
        //Random.GetBytes(output);
        RandomNumberGenerator.Create().GetBytes(output);
        PushEntropy(output);
    }
}
