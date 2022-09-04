using Newtonsoft.Json.Linq;
using SecretNET.Crypto;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace SecretNET.Common;

public static class Utils
{
    /// <summary>
    /// Converts bytes to hexstring.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>System.String.</returns>
    public static string ToHexString(this byte[] date)
    {
        return Convert.ToHexString(date).ToLower();
    }

    /// <summary>
    /// Sorts an JObject (used for amino).
    /// </summary>
    /// <param name="jObj">The j object.</param>
    public static void SortJObject(JObject jObj)
    {
        if (jObj != null)
        {
            var props = jObj.Properties().ToList();
            foreach (var prop in props)
            {
                prop.Remove();
            }

            foreach (var prop in props.OrderBy(p => p.Name))
            {
                jObj.Add(prop);
                if (prop.Value is JObject)
                {
                    SortJObject((JObject)prop.Value);
                }
                else if (prop.Value is JArray)
                {
                    foreach (var item in (JArray)prop.Value)
                    {
                        if (item is JObject)
                        {
                            SortJObject((JObject)item);
                        }
                    }
                }
                else
                {

                }
            }
        }
    }

    /// <summary>
    /// Encrypts to file.
    /// </summary>
    /// <param name="fullFileName">Full name of the file.</param>
    /// <param name="data">The data.</param>
    /// <param name="encryptionKey">The encryption key.</param>
    public static void AesEncryptToFile(string fullFileName, string data, byte[] encryptionKey) 
    {
        using (FileStream fileStream = new(fullFileName, FileMode.OpenOrCreate))
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = encryptionKey;

                byte[] iv = aes.IV;
                fileStream.Write(iv, 0, iv.Length);

                using (CryptoStream cryptoStream = new(
                    fileStream,
                    aes.CreateEncryptor(),
                    CryptoStreamMode.Write))
                {
                    using (StreamWriter encryptWriter = new(cryptoStream))
                    {
                        encryptWriter.Write(data);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Aeses the decrypt file.
    /// </summary>
    /// <param name="fullFileName">Full name of the file.</param>
    /// <param name="encryptionKey">The encryption key.</param>
    /// <returns>System.String.</returns>
    public static async Task<string> AesDecryptFile(string fullFileName, byte[] encryptionKey)
    {
        using (FileStream fileStream = new(fullFileName, FileMode.Open))
        {
            using (Aes aes = Aes.Create())
            {
                byte[] iv = new byte[aes.IV.Length];
                int numBytesToRead = aes.IV.Length;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    int n = fileStream.Read(iv, numBytesRead, numBytesToRead);
                    if (n == 0) break;

                    numBytesRead += n;
                    numBytesToRead -= n;
                }

                using (CryptoStream cryptoStream = new(fileStream, aes.CreateDecryptor(encryptionKey, iv), CryptoStreamMode.Read))
                {
                    using (StreamReader decryptReader = new(cryptoStream))
                    {
                        string decryptedMessage = await decryptReader.ReadToEndAsync();
                        return decryptedMessage;
                    }
                }
            }
        }
    }


    // internal

    // signing
    internal static ECDSASignature Sign(this ECPrivKey key, uint256 h, bool enforceLowR)
    {
        return new ECDSASignature(key.Sign(h, enforceLowR, out _));
    }

    internal static SecpECDSASignature Sign(this ECPrivKey key, uint256 h, bool enforceLowR, out int recid)
    {
        Span<byte> hash = stackalloc byte[32];
        h.ToBytes(hash);
        byte[] extra_entropy = null;
        RFC6979NonceFunction nonceFunction = null;
        Span<byte> vchSig = stackalloc byte[SecpECDSASignature.MaxLength];
        SecpECDSASignature sig;
        uint counter = 0;
        bool ret = key.TrySignECDSA(hash, null, out recid, out sig);
        // Grind for low R
        while (ret && sig.r.IsHigh && enforceLowR)
        {
            if (extra_entropy == null || nonceFunction == null)
            {
                extra_entropy = new byte[32];
                nonceFunction = new RFC6979NonceFunction(extra_entropy);
            }
            Utils.ToBytes(++counter, true, extra_entropy.AsSpan());
            ret = key.TrySignECDSA(hash, nonceFunction, out recid, out sig);
        }
        return sig;
    }

    internal static void SafeSet(ManualResetEvent ar)
    {
        try
        {
            if (!ar.SafeWaitHandle.IsClosed && !ar.SafeWaitHandle.IsInvalid)
                ar.Set();
        }
        catch { }
    }
    internal static bool ArrayEqual(byte[] a, byte[] b)
    {
        if (a == null && b == null)
            return true;
        if (a == null)
            return false;
        if (b == null)
            return false;
        return ArrayEqual(a, 0, b, 0, Math.Max(a.Length, b.Length));
    }
    internal static bool ArrayEqual(byte[] a, int startA, byte[] b, int startB, int length)
    {
        if (a == null && b == null)
            return true;
        if (a == null)
            return false;
        if (b == null)
            return false;
        var alen = a.Length - startA;
        var blen = b.Length - startB;

        if (alen < length || blen < length)
            return false;

        for (int ai = startA, bi = startB; ai < startA + length; ai++, bi++)
        {
            if (a[ai] != b[bi])
                return false;
        }
        return true;
    }

    public static ArraySegment<T> Slice<T>(this ArraySegment<T> seg, int index)
    {
        return new ArraySegment<T>(seg.Array, seg.Offset + index, seg.Count - index);
    }

    static DateTimeOffset unixRef = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public static uint DateTimeToUnixTime(DateTimeOffset dt)
    {
        return (uint)DateTimeToUnixTimeLong(dt);
    }

    internal static ulong DateTimeToUnixTimeLong(DateTimeOffset dt)
    {
        dt = dt.ToUniversalTime();
        if (dt < unixRef)
            throw new ArgumentOutOfRangeException("The supplied datetime can't be expressed in unix timestamp");
        var result = (dt - unixRef).TotalSeconds;
        if (result > uint.MaxValue)
            throw new ArgumentOutOfRangeException("The supplied datetime can't be expressed in unix timestamp");
        return (ulong)result;
    }

    public static DateTimeOffset UnixTimeToDateTime(uint timestamp)
    {
        var span = TimeSpan.FromSeconds(timestamp);
        return unixRef + span;
    }
    public static DateTimeOffset UnixTimeToDateTime(ulong timestamp)
    {
        var span = TimeSpan.FromSeconds(timestamp);
        return unixRef + span;
    }
    public static DateTimeOffset UnixTimeToDateTime(long timestamp)
    {
        var span = TimeSpan.FromSeconds(timestamp);
        return unixRef + span;
    }

    public static void AddOrReplace<TKey, TValue>(this IDictionary<TKey, TValue> dico, TKey key, TValue value)
    {
        if (dico.ContainsKey(key))
            dico[key] = value;
        else
            dico.Add(key, value);
    }

    public static TValue TryGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        TValue value;
        dictionary.TryGetValue(key, out value);
        return value;
    }

    public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if (!dictionary.ContainsKey(key))
        {
            dictionary.Add(key, value);
            return true;
        }
        return false;
    }


    public static byte[] ToBytes(uint value, bool littleEndian)
    {

        if (littleEndian && BitConverter.IsLittleEndian)
        {
            var result = new byte[4];
            MemoryMarshal.Cast<byte, uint>(result)[0] = value;
            return result;
        }

        if (littleEndian)
        {
            return new byte[]
            {
                (byte)value,
                (byte)(value >> 8),
                (byte)(value >> 16),
                (byte)(value >> 24),
            };
        }
        else
        {
            return new byte[]
            {
                (byte)(value >> 24),
                (byte)(value >> 16),
                (byte)(value >> 8),
                (byte)value,
            };
        }
    }

    public static byte[] ToBytes(ulong value, bool littleEndian)
    {

        if (littleEndian && BitConverter.IsLittleEndian)
        {
            var result = new byte[8];
            MemoryMarshal.Cast<byte, ulong>(result)[0] = value;
            return result;
        }

        if (littleEndian)
        {
            return new byte[]
            {
                (byte)value,
                (byte)(value >> 8),
                (byte)(value >> 16),
                (byte)(value >> 24),
                (byte)(value >> 32),
                (byte)(value >> 40),
                (byte)(value >> 48),
                (byte)(value >> 56),
            };
        }
        else
        {
            return new byte[]
            {
                (byte)(value >> 56),
                (byte)(value >> 48),
                (byte)(value >> 40),
                (byte)(value >> 32),
                (byte)(value >> 24),
                (byte)(value >> 16),
                (byte)(value >> 8),
                (byte)value,
            };
        }
    }


    public static void ToBytes(uint value, bool littleEndian, Span<byte> output)
    {
        if (littleEndian && BitConverter.IsLittleEndian)
        {
            MemoryMarshal.Cast<byte, uint>(output)[0] = value;
            return;
        }

        if (littleEndian)
        {
            output[0] = (byte)value;
            output[1] = (byte)(value >> 8);
            output[2] = (byte)(value >> 16);
            output[3] = (byte)(value >> 24);
        }
        else
        {
            output[0] = (byte)(value >> 24);
            output[1] = (byte)(value >> 16);
            output[2] = (byte)(value >> 8);
            output[3] = (byte)value;
        }
    }

    public static void ToBytes(ulong value, bool littleEndian, Span<byte> output)
    {
        if (littleEndian && BitConverter.IsLittleEndian)
        {
            MemoryMarshal.Cast<byte, ulong>(output)[0] = value;
            return;
        }

        if (littleEndian)
        {
            output[0] = (byte)value;
            output[1] = (byte)(value >> 8);
            output[2] = (byte)(value >> 16);
            output[3] = (byte)(value >> 24);
            output[4] = (byte)(value >> 32);
            output[5] = (byte)(value >> 40);
            output[6] = (byte)(value >> 48);
            output[7] = (byte)(value >> 56);
        }
        else
        {
            output[0] = (byte)(value >> 56);
            output[1] = (byte)(value >> 48);
            output[2] = (byte)(value >> 40);
            output[3] = (byte)(value >> 32);
            output[4] = (byte)(value >> 24);
            output[5] = (byte)(value >> 16);
            output[6] = (byte)(value >> 8);
            output[7] = (byte)value;
        }
    }


    public static uint ToUInt32(byte[] value, int index, bool littleEndian)
    {

        if (littleEndian && BitConverter.IsLittleEndian)
        {
            return MemoryMarshal.Cast<byte, uint>(value.AsSpan().Slice(index))[0];
        }

        if (littleEndian)
        {
            return value[index]
                   + ((uint)value[index + 1] << 8)
                   + ((uint)value[index + 2] << 16)
                   + ((uint)value[index + 3] << 24);
        }
        else
        {
            return value[index + 3]
                   + ((uint)value[index + 2] << 8)
                   + ((uint)value[index + 1] << 16)
                   + ((uint)value[index + 0] << 24);
        }
    }

    public static uint ToUInt32(ReadOnlySpan<byte> value, bool littleEndian)
    {
        if (littleEndian && BitConverter.IsLittleEndian)
        {
            return MemoryMarshal.Cast<byte, uint>(value)[0];
        }
        if (littleEndian)
        {
            return value[0]
                   + ((uint)value[1] << 8)
                   + ((uint)value[2] << 16)
                   + ((uint)value[3] << 24);
        }
        else
        {
            return value[3]
                   + ((uint)value[2] << 8)
                   + ((uint)value[1] << 16)
                   + ((uint)value[0] << 24);
        }
    }

    public static int ToInt32(byte[] value, int index, bool littleEndian)
    {
        return unchecked((int)ToUInt32(value, index, littleEndian));
    }

    public static uint ToUInt32(byte[] value, bool littleEndian)
    {
        return ToUInt32(value, 0, littleEndian);
    }

    public static ulong ToUInt64(byte[] value, int offset, bool littleEndian)
    {

        if (littleEndian && BitConverter.IsLittleEndian)
        {
            return MemoryMarshal.Cast<byte, ulong>(value.AsSpan().Slice(offset))[0];
        }

        if (littleEndian)
        {
            return value[offset + 0]
                   + ((ulong)value[offset + 1] << 8)
                   + ((ulong)value[offset + 2] << 16)
                   + ((ulong)value[offset + 3] << 24)
                   + ((ulong)value[offset + 4] << 32)
                   + ((ulong)value[offset + 5] << 40)
                   + ((ulong)value[offset + 6] << 48)
                   + ((ulong)value[offset + 7] << 56);
        }
        else
        {
            return value[offset + 7]
                + ((ulong)value[offset + 6] << 8)
                + ((ulong)value[offset + 5] << 16)
                + ((ulong)value[offset + 4] << 24)
                + ((ulong)value[offset + 3] << 32)
                   + ((ulong)value[offset + 2] << 40)
                   + ((ulong)value[offset + 1] << 48)
                   + ((ulong)value[offset + 0] << 56);
        }
    }

    public static ulong ToUInt64(ReadOnlySpan<byte> value, bool littleEndian)
    {
        if (littleEndian && BitConverter.IsLittleEndian)
        {
            return MemoryMarshal.Cast<byte, ulong>(value)[0];
        }
        if (littleEndian)
        {
            return value[0]
                   + ((ulong)value[1] << 8)
                   + ((ulong)value[2] << 16)
                   + ((ulong)value[3] << 24)
                   + ((ulong)value[4] << 32)
                   + ((ulong)value[5] << 40)
                   + ((ulong)value[6] << 48)
                   + ((ulong)value[7] << 56);
        }
        else
        {
            return value[7]
                + ((ulong)value[6] << 8)
                + ((ulong)value[5] << 16)
                + ((ulong)value[4] << 24)
                + ((ulong)value[3] << 32)
                   + ((ulong)value[2] << 40)
                   + ((ulong)value[1] << 48)
                   + ((ulong)value[0] << 56);
        }
    }


    public static ulong ToUInt64(byte[] value, bool littleEndian)
    {
        return ToUInt64(value, 0, littleEndian);
    }

    public static int GetHashCode(byte[] array)
    {
        if (array == null)
        {
            return 0;
        }

        int i = array.Length;
        int hc = i + 1;

        while (--i >= 0)
        {
            hc *= 257;
            hc ^= array[i];
        }

        return hc;
    }
}
