namespace SecretNET.Common;

internal static class ByteArrayExtensions
{
    internal static bool StartWith(this byte[] data, byte[] versionBytes)
    {
        if (data.Length < versionBytes.Length)
            return false;
        for (int i = 0; i < versionBytes.Length; i++)
        {
            if (data[i] != versionBytes[i])
                return false;
        }
        return true;
    }
    internal static byte[] SafeSubarray(this byte[] array, int offset, int count)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));
        if (offset < 0 || offset > array.Length)
            throw new ArgumentOutOfRangeException("offset");
        if (count < 0 || offset + count > array.Length)
            throw new ArgumentOutOfRangeException("count");
        if (offset == 0 && array.Length == count)
            return array;
        var data = new byte[count];
        Buffer.BlockCopy(array, offset, data, 0, count);
        return data;
    }

    internal static byte[] SafeSubarray(this byte[] array, int offset)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));
        if (offset < 0 || offset > array.Length)
            throw new ArgumentOutOfRangeException("offset");

        var count = array.Length - offset;
        var data = new byte[count];
        Buffer.BlockCopy(array, offset, data, 0, count);
        return data;
    }

    internal static byte[] Concat(this byte[] arr, params byte[][] arrs)
    {
        var len = arr.Length + arrs.Sum(a => a.Length);
        var ret = new byte[len];
        Buffer.BlockCopy(arr, 0, ret, 0, arr.Length);
        var pos = arr.Length;
        foreach (var a in arrs)
        {
            Buffer.BlockCopy(a, 0, ret, pos, a.Length);
            pos += a.Length;
        }
        return ret;
    }

    public static byte[] Subarray(this byte[] array, int startIndex, int? length = null)
    {
        return new ArraySegment<byte>(array, startIndex, length ?? array.Length - startIndex).ToArray();
    }

    public static void Set(this byte[] array, byte[] values, int offset = 0)
    {
        for (int i = 0; i < values.Length; i++)
        {
            array[i + offset] = values[i];
        }
    }

    public static void Xor(byte[] source, byte[] destination, int length)
    {
        Xor(source, 0, destination, 0, length);
    }

    public static void Xor(byte[] source, int sourceIndex, byte[] destination, int destinationIndex, int length)
    {
        for (int i = 0; i < length; ++i)
        {
            destination[destinationIndex + i] ^= source[sourceIndex + i];
        }
    }

    public static byte[] Wipe(byte[] array)
    {
        for (var i = 0; i < array.Length; i++)
        {
            array[i] = 0;
        }
        return array;
    }

    public static uint[] Wipe(uint[] array)
    {
        for (var i = 0; i < array.Length; i++)
        {
            array[i] = 0;
        }
        return array;
    }

    
}
