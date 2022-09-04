namespace SecretNET.Crypto;

internal class ECDSASignature
{

    /* This function is taken from the libsecp256k1 distribution and implements
	*  DER parsing for ECDSA signatures, while supporting an arbitrary subset of
	*  format violations.
	*
	*  Supported violations include negative integers, excessive padding, garbage
	*  at the end, and overly long length descriptors. This is safe to use in
	*  Bitcoin because since the activation of BIP66, signatures are verified to be
	*  strict DER before being passed to this module, and we know it supports all
	*  violations present in the blockchain before that point.
	*/
    static bool ecdsa_signature_parse_der_lax(ReadOnlySpan<byte> input, out SecpECDSASignature sig)
    {
        int inputlen = input.Length;
        int rpos, rlen, spos, slen;
        int pos = 0;
        int lenbyte;
        Span<byte> tmpsig = stackalloc byte[64];
        tmpsig.Clear();
        sig = null;
        int overflow = 0;

        /* Sequence tag byte */
        if (pos == inputlen || input[pos] != 0x30)
        {
            return false;
        }
        pos++;

        /* Sequence length bytes */
        if (pos == inputlen)
        {
            return false;
        }
        lenbyte = input[pos++];
        if ((lenbyte & 0x80) != 0)
        {
            lenbyte -= 0x80;
            if (lenbyte > inputlen - pos)
            {
                return false;
            }
            pos += lenbyte;
        }

        /* Integer tag byte for R */
        if (pos == inputlen || input[pos] != 0x02)
        {
            return false;
        }
        pos++;

        /* Integer length for R */
        if (pos == inputlen)
        {
            return false;
        }
        lenbyte = input[pos++];
        if ((lenbyte & 0x80) != 0)
        {
            lenbyte -= 0x80;
            if (lenbyte > inputlen - pos)
            {
                return false;
            }
            while (lenbyte > 0 && input[pos] == 0)
            {
                pos++;
                lenbyte--;
            }
            if (lenbyte >= 4)
            {
                return false;
            }
            rlen = 0;
            while (lenbyte > 0)
            {
                rlen = (rlen << 8) + input[pos];
                pos++;
                lenbyte--;
            }
        }
        else
        {
            rlen = lenbyte;
        }
        if (rlen > inputlen - pos)
        {
            return false;
        }
        rpos = pos;
        pos += rlen;

        /* Integer tag byte for S */
        if (pos == inputlen || input[pos] != 0x02)
        {
            return false;
        }
        pos++;

        /* Integer length for S */
        if (pos == inputlen)
        {
            return false;
        }
        lenbyte = input[pos++];
        if ((lenbyte & 0x80) != 0)
        {
            lenbyte -= 0x80;
            if (lenbyte > inputlen - pos)
            {
                return false;
            }
            while (lenbyte > 0 && input[pos] == 0)
            {
                pos++;
                lenbyte--;
            }
            if (lenbyte >= 4)
            {
                return false;
            }
            slen = 0;
            while (lenbyte > 0)
            {
                slen = (slen << 8) + input[pos];
                pos++;
                lenbyte--;
            }
        }
        else
        {
            slen = lenbyte;
        }
        if (slen > inputlen - pos)
        {
            return false;
        }
        spos = pos;

        /* Ignore leading zeroes in R */
        while (rlen > 0 && input[rpos] == 0)
        {
            rlen--;
            rpos++;
        }
        /* Copy R value */
        if (rlen > 32)
        {
            overflow = 1;
        }
        else
        {
            input.Slice(rpos, rlen).CopyTo(tmpsig.Slice(32 - rlen));
            //memcpy(tmpsig + 32 - rlen, input + rpos, rlen);
        }

        /* Ignore leading zeroes in S */
        while (slen > 0 && input[spos] == 0)
        {
            slen--;
            spos++;
        }
        /* Copy S value */
        if (slen > 32)
        {
            overflow = 1;
        }
        else
        {
            input.Slice(spos, slen).CopyTo(tmpsig.Slice(64 - slen));
            //memcpy(tmpsig + 64 - slen, input + spos, slen);
        }

        if (overflow == 0)
        {
            overflow = SecpECDSASignature.TryCreateFromCompact(tmpsig, out sig) ? 0 : 1;
        }
        if (overflow != 0)
        {
            /* Overwrite the result again with a correctly-parsed but invalid
				   signature if parsing failed. */
            tmpsig.Clear();
            SecpECDSASignature.TryCreateFromCompact(tmpsig, out sig);
        }
        return true;
    }
    private readonly Scalar r, s;
    internal ECDSASignature(in Scalar r, in Scalar s)
    {
        this.r = r;
        this.s = s;
    }

    internal ECDSASignature(SecpECDSASignature sig)
    {
        r = sig.r;
        s = sig.s;
    }
    internal static bool TryParseFromCompact(byte[] compactFormat, out ECDSASignature signature)
    {
        if (compactFormat == null)
            throw new ArgumentNullException(nameof(compactFormat));
        signature = null;
        if (compactFormat.Length != 64)
            return false;

        if (SecpECDSASignature.TryCreateFromCompact(compactFormat, out var s) && s is SecpECDSASignature)
        {
            signature = new ECDSASignature(s);
            return true;
        }
        return false;
    }
    internal static bool TryParseFromCompact(ReadOnlySpan<byte> compactFormat, out ECDSASignature signature)
    {
        signature = null;
        if (compactFormat.Length != 64)
            return false;
        if (SecpECDSASignature.TryCreateFromCompact(compactFormat, out var s) && s is SecpECDSASignature)
        {
            signature = new ECDSASignature(s);
            return true;
        }
        return false;
    }
    internal ECDSASignature(byte[] derSig) : this(derSig.AsSpan())
    {
    }

    public byte[] ToCompact()
    {
        var result = new byte[64];
        ToSecpECDSASignature().WriteCompactToSpan(result.AsSpan());
        return result;
    }

    internal ECDSASignature(ReadOnlySpan<byte> derSig)
    {
        if (ecdsa_signature_parse_der_lax(derSig, out var sig) && sig is SecpECDSASignature)
        {
            (r, s) = sig;
            return;
        }
        throw new FormatException(InvalidDERSignature);
    }
    /**
	* What we get back from the signer are the two components of a signature, r and s. To get a flat byte stream
	* of the type used by Bitcoin we have to encode them using DER encoding, which is just a way to pack the two
	* components into a structure.
	*/
    internal byte[] ToDER()
    {
        Span<byte> tmp = stackalloc byte[75];
        ToSecpECDSASignature().WriteDerToSpan(tmp, out int l);
        tmp = tmp.Slice(0, l);
        return tmp.ToArray();
    }


    internal void WriteDerToSpan(Span<byte> sigs, out int length)
    {
        ToSecpECDSASignature().WriteDerToSpan(sigs, out length);
    }
    internal SecpECDSASignature ToSecpECDSASignature()
    {
        return new SecpECDSASignature(r, s.IsHigh ? s.Negate() : s, false);
    }

    const string InvalidDERSignature = "Invalid DER signature";
    internal static ECDSASignature FromDER(byte[] sig)
    {
        return new ECDSASignature(sig);
    }


    /// <summary>
    /// Enforce LowS on the signature
    /// </summary>
    internal ECDSASignature MakeCanonical()
    {
        if (!IsLowS)
        {
            return new ECDSASignature(r, s.Negate());
        }
        else
            return this;
    }
    internal bool IsLowS
    {
        get
        {
            return !s.IsHigh;
        }
    }

    internal bool IsLowR
    {
        get
        {
            return !r.IsHigh;
        }
    }
    internal static bool IsValidDER(ReadOnlySpan<byte> bytes)
    {
        if (bytes == null)
            throw new ArgumentNullException(nameof(bytes));
        return ecdsa_signature_parse_der_lax(bytes, out _);
    }
    internal static bool IsValidDER(byte[] bytes)
    {
        if (bytes == null)
            throw new ArgumentNullException(nameof(bytes));
        return IsValidDER(bytes.AsSpan());
    }

}
