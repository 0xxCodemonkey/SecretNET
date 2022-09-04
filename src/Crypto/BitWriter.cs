using System.Collections;

namespace SecretNET.Crypto;

class BitReader
{
    BitArray array;

    internal BitReader(BitArray array)
    {
        this.array = new BitArray(array.Length);
        for (int i = 0; i < array.Length; i++)
            this.array.Set(i, array.Get(i));
    }

    internal bool Read()
    {
        var v = array.Get(Position);
        Position++;
        return v;
    }

    internal int Position
    {
        get;
        set;
    }

    internal int Count
    {
        get
        {
            return array.Length;
        }
    }
}
class BitWriter
{
    List<bool> values = new List<bool>();
    internal int Count
    {
        get
        {
            return values.Count;
        }
    }
    internal void Write(bool value)
    {
        values.Insert(Position, value);
        _Position++;
    }

    internal void Write(byte[] bytes)
    {
        Write(bytes, bytes.Length * 8);
    }

    internal void Write(byte[] bytes, int bitCount)
    {
        bytes = SwapEndianBytes(bytes);
        BitArray array = new BitArray(bytes);
        values.InsertRange(Position, array.OfType<bool>().Take(bitCount));
        _Position += bitCount;
    }

    internal byte[] ToBytes()
    {
        var array = ToBitArray();
        var bytes = ToByteArray(array);
        bytes = SwapEndianBytes(bytes);
        return bytes;
    }

    //BitArray.CopyTo do not exist in portable lib
    static byte[] ToByteArray(BitArray bits)
    {
        int arrayLength = bits.Length / 8;
        if (bits.Length % 8 != 0)
            arrayLength++;
        byte[] array = new byte[arrayLength];

        for (int i = 0; i < bits.Length; i++)
        {
            int b = i / 8;
            int offset = i % 8;
            array[b] |= bits.Get(i) ? (byte)(1 << offset) : (byte)0;
        }
        return array;
    }


    internal BitArray ToBitArray()
    {
        return new BitArray(values.ToArray());
    }

    internal int[] ToIntegers()
    {
        var array = new BitArray(values.ToArray());
        return Wordlist.ToIntegers(array);
    }


    static byte[] SwapEndianBytes(byte[] bytes)
    {
        byte[] output = new byte[bytes.Length];
        for (int i = 0; i < output.Length; i++)
        {
            byte newByte = 0;
            for (int ib = 0; ib < 8; ib++)
            {
                newByte += (byte)((bytes[i] >> ib & 1) << 7 - ib);
            }
            output[i] = newByte;
        }
        return output;
    }



    internal void Write(uint value, int bitCount)
    {
        for (int i = 0; i < bitCount; i++)
        {
            Write((value & 1) == 1);
            value = value >> 1;
        }
    }

    int _Position;
    internal int Position
    {
        get
        {
            return _Position;
        }
        set
        {
            _Position = value;
        }
    }

    internal void Write(BitReader reader, int bitCount)
    {
        for (int i = 0; i < bitCount; i++)
        {
            Write(reader.Read());
        }
    }

    internal void Write(BitArray bitArray)
    {
        Write(bitArray, bitArray.Length);
    }
    internal void Write(BitArray bitArray, int bitCount)
    {
        for (int i = 0; i < bitCount; i++)
        {
            Write(bitArray.Get(i));
        }
    }

    internal void Write(BitReader reader)
    {
        Write(reader, reader.Count - reader.Position);
    }

    internal BitReader ToReader()
    {
        return new BitReader(ToBitArray());
    }

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder(values.Count);
        for (int i = 0; i < Count; i++)
        {
            if (i != 0 && i % 8 == 0)
                builder.Append(' ');
            builder.Append(values[i] ? "1" : "0");
        }
        return builder.ToString();
    }
}

