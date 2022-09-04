#nullable enable
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SecretNET.Crypto.Secp256k1;

internal partial class ECPubKey
{
    [Obsolete("Use SigVerifySchnorr instead")]
    public bool SigVerify(SecpSchnorrSignature signature, ReadOnlySpan<byte> msg32)
    {
        return SigVerifySchnorr(signature, msg32);
    }

    public bool SigVerifySchnorr(SecpSchnorrSignature signature, ReadOnlySpan<byte> msg32)
    {
        if (msg32.Length != 32)
            return false;
        if (signature is null)
            return false;
        ref readonly Scalar s = ref signature.s;
        Scalar e;
        GEJ rj;
        ref readonly FE rx = ref signature.rx;

        using var sha = new Secp256k1.SHA256();
        Span<byte> buf = stackalloc byte[33];
        signature.rx.WriteToSpan(buf);
        sha.Write(buf.Slice(0, 32));
        this.WriteToSpan(true, buf, out _);
        sha.Write(buf);
        msg32.CopyTo(buf);
        sha.Write(buf.Slice(0, 32));
        sha.GetHash(buf);
        e = new Scalar(buf, out _);

        if (!secp256k1_schnorrsig_real_verify(ctx, s, e, this.Q, out rj)
            || !rj.HasQuadYVariable /* fails if rj is infinity */
            || !rx.EqualsXVariable(rj))
        {
            return false;
        }

        return true;
    }

    private bool secp256k1_schnorrsig_real_verify(Context ctx, Scalar s, Scalar e, GE pkp, out GEJ rj)
    {
        Scalar nege;
        GEJ pkj;

        nege = e.Negate();

        pkj = pkp.ToGroupElementJacobian();

        rj = ctx.EcMultContext.Mult(pkj, nege, s);
        return true;
    }
}

