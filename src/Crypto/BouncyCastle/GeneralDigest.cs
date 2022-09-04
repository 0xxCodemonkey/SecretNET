namespace SecretNET.Crypto.BouncyCastle;

/**
    * base implementation of MD4 family style digest as outlined in
    * "Handbook of Applied Cryptography", pages 344 - 347.
    */
internal abstract class GeneralDigest
{
    private const int BYTE_LENGTH = 64;

    private byte[] xBuf;
    private int xBufOff;

    private long byteCount;

    internal GeneralDigest()
    {
        xBuf = new byte[4];
    }

    internal GeneralDigest(GeneralDigest t)
    {
        xBuf = new byte[t.xBuf.Length];
        CopyIn(t);
    }

    protected void CopyIn(GeneralDigest t)
    {
        Array.Copy(t.xBuf, 0, xBuf, 0, t.xBuf.Length);

        xBufOff = t.xBufOff;
        byteCount = t.byteCount;
    }

    internal void Update(byte input)
    {
        xBuf[xBufOff++] = input;

        if (xBufOff == xBuf.Length)
        {
            ProcessWord(xBuf, 0);
            xBufOff = 0;
        }

        byteCount++;
    }

    internal void BlockUpdate(
        byte[] input,
        int inOff,
        int length)
    {
        length = Math.Max(0, length);

        //
        // fill the current word
        //
        int i = 0;
        if (xBufOff != 0)
        {
            while (i < length)
            {
                xBuf[xBufOff++] = input[inOff + i++];
                if (xBufOff == 4)
                {
                    ProcessWord(xBuf, 0);
                    xBufOff = 0;
                    break;
                }
            }
        }

        //
        // process whole words.
        //
        int limit = (length - i & ~3) + i;
        for (; i < limit; i += 4)
        {
            ProcessWord(input, inOff + i);
        }

        //
        // load in the remainder.
        //
        while (i < length)
        {
            xBuf[xBufOff++] = input[inOff + i++];
        }

        byteCount += length;
    }

    internal void Finish()
    {
        long bitLength = byteCount << 3;

        //
        // add the pad bytes.
        //
        Update(128);

        while (xBufOff != 0)
            Update(0);
        ProcessLength(bitLength);
        ProcessBlock();
    }

    internal virtual void Reset()
    {
        byteCount = 0;
        xBufOff = 0;
        Array.Clear(xBuf, 0, xBuf.Length);
    }

    internal int GetByteLength()
    {
        return BYTE_LENGTH;
    }

    internal abstract void ProcessWord(byte[] input, int inOff);
    internal abstract void ProcessLength(long bitLength);
    internal abstract void ProcessBlock();
    internal abstract string AlgorithmName
    {
        get;
    }
    internal abstract int GetDigestSize();
    internal abstract int DoFinal(byte[] output, int outOff);

}
