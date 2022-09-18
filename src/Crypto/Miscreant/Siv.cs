using SecretNET.Crypto.Miscreant.mac;
using SecretNET.Crypto.Miscreant.providers;

namespace SecretNET.Crypto.Miscreant;

internal class Siv : ISIVLike
{
    private IMACLike _mac;
    private ICTRLike _ctr;
    private Block _tmp1;
    private Block _tmp2;

    /// <summary>
    /// Maximum number of associated data items
    /// </summary>
    internal const int MAX_ASSOCIATED_DATA = 126;

    /// <summary>
    /// The AES-SIV mode of authenticated encryption
    /// </summary>
    /// <param name="keyData">The key data.</param>
    /// <param name="provider">The provider.</param>
    /// <returns>Siv.</returns>
    /// <exception cref="Exception">AES - SIV: key must be 32 or 64 - bytes(got { keyData.Length }</exception>
    internal static Siv ImportKey(byte[] keyData, ICryptoProvider provider = null)
    {
        // We only support AES-128 and AES-256. AES-SIV needs a key 2X as long the intended security level
        if (keyData.Length != 32 && keyData.Length != 64)
        {
            throw new Exception($"AES - SIV: key must be 32 or 64 - bytes(got {keyData.Length}");
        }

        provider = provider ?? new SoftCryptoProvider();

        var macKey = keyData.Take(keyData.Length / 2).ToArray();
        var encKey = keyData.Skip(keyData.Length / 2).ToArray();

        IMACLike mac = Cmac.ImportKey(provider, macKey); ;
        ICTRLike ctr = provider.ImportCTRKey(encKey);

        return new Siv(mac, ctr);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Siv"/> class.
    /// </summary>
    /// <param name="mac">The mac.</param>
    /// <param name="ctr">The CTR.</param>
    private Siv(IMACLike mac, ICTRLike ctr)
    {
        _mac = mac;
        _ctr = ctr;
        _tmp1 = new Block();
        _tmp2 = new Block();
    }

    /// <summary>
    /// Encrypt and authenticate data using AES-SIV
    /// </summary>
    /// <param name="plaintext">The plaintext.</param>
    /// <param name="associatedData">The associated data.</param>
    /// <param name="">The .</param>
    /// <returns>System.Byte[].</returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="NotImplementedException"></exception>
    public byte[] Seal(byte[] plaintext, byte[][] associatedData = null)
    {
        associatedData = new byte[][] { new byte[0] };
        if (associatedData.Length > MAX_ASSOCIATED_DATA)
        {
            throw new Exception("AES-SIV: too many associated data items");
        }

        // Allocate space for sealed ciphertext.
        var resultLength = Block.SIZE + plaintext.Length;
        var result = new byte[resultLength];

        // Authenticate.
        var iv = S2V(plaintext, associatedData);
        result.Set(iv);

        // Encrypt.
        zeroIVBits(iv);
        var r = _ctr.EncryptCtr(iv, plaintext);
        result.Set(r, iv.Length);
        return result;
    }

    /// <summary>
    /// Decrypt and authenticate data using AES-SIV
    /// </summary>
    /// <param name="sealedText">The sealed text.</param>
    /// <param name="associatedData">The associated data.</param>
    /// <returns>System.Byte[].</returns>
    /// <exception cref="Exception">AES-SIV: too many associated data items</exception>
    /// <exception cref="Exception">AES-SIV: ciphertext is truncated</exception>
    /// <exception cref="Exception">AES-SIV: ciphertext verification failure!</exception>
    /// <exception cref="NotImplementedException"></exception>
    public byte[] Open(byte[] sealedText, byte[][]? associatedData = null)
    {
        associatedData = new byte[][] { new byte[0] };
        if (associatedData.Length > MAX_ASSOCIATED_DATA)
        {
            throw new Exception("AES-SIV: too many associated data items");
        }

        if (sealedText.Length < Block.SIZE)
        {
            throw new Exception("AES-SIV: ciphertext is truncated");
        }

        // Decrypt.
        var tag = sealedText.Subarray(0, Block.SIZE);
        var iv = _tmp1.Data;

        iv.Set(tag);
        zeroIVBits(iv);

        // NOTE: "encryptCtr" is intentional. CTR encryption/decryption are the same
        var result = _ctr.EncryptCtr(iv, sealedText.Subarray(Block.SIZE));

        // Authenticate.
        var expectedTag = S2V(result, associatedData);

        if (!expectedTag.SequenceEqual(tag))
        {
            ByteArrayExtensions.Wipe(result);
            throw new Exception("AES-SIV: ciphertext verification failure!");
        }

        return result;
    }

    /// <summary>
    /// Make a best effort to wipe memory used by this instance
    /// </summary>
    /// <returns>ISIVLike.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public ISIVLike Clear()
    {
        _tmp1.Clear();
        _tmp2.Clear();
        _ctr.Clear();
        _mac.Clear();

        return this;
    }

    // private methods

    /// <summary>
    /// S2V operation, defined in the section 2.4 of
    /// <see href="https://tools.ietf.org/html/rfc5297#section-2.4">RFC 5297</see>.
    /// </summary>
    private byte[] S2V(byte[] plaintext, byte[][] associated_data)
    {

        if (associated_data == null)
        {
            throw new ArgumentNullException(nameof(associated_data));
        }

        if (plaintext == null)
        {
            throw new ArgumentNullException(nameof(plaintext));
        }

        _mac.Reset();
        _tmp1.Clear();

        // Note: the standalone S2V returns CMAC(1) if the number of passed
        // vectors is zero, however in SIV construction this case is never
        // triggered, since we always pass plaintext as the last vector (even
        // if it's zero-length), so we omit this case.
        _mac.Update(_tmp1.Data);
        _tmp2.Clear();
        var r = _mac.Finish();

        _tmp2.Data.Set(r);
        _mac.Reset();

        foreach (var ad in associated_data)
        {
            _mac.Update(ad);
            _tmp1.Clear();
            _tmp1.Data.Set(_mac.Finish());
            _mac.Reset();
            _tmp2.Dbl();
            ByteArrayExtensions.Xor(_tmp1.Data, _tmp2.Data, Block.SIZE);
        }

        _tmp1.Clear();

        if (plaintext.Length >= Block.SIZE)
        {
            var n = plaintext.Length - Block.SIZE;
            _tmp1.Data.Set(plaintext.Subarray(n));
            _mac.Update(plaintext.Subarray(0, n));
        }
        else
        {
            _tmp1.Data.Set(plaintext);
            _tmp1.Data[plaintext.Length] = 0x80;
            _tmp2.Dbl();
        }
        ByteArrayExtensions.Xor(_tmp2.Data, _tmp1.Data, _tmp1.Data.Length);
        _mac.Update(_tmp1.Data);
        return _mac.Finish();
    }

    /// <summary>
    /// Zero out the top bits in the last 32-bit words of the IV
    /// </summary>
    /// <param name="iv">The iv.</param>
    private void zeroIVBits(byte[] iv)
    {
        // "We zero-out the top bit in each of the last two 32-bit words
        // of the IV before assigning it to Ctr"
        //  — http://web.cs.ucdavis.edu/~rogaway/papers/siv.pdf
        iv[iv.Length - 8] &= 0x7f;
        iv[iv.Length - 4] &= 0x7f;
    }

}
