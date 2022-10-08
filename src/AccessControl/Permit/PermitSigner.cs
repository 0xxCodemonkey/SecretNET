using Newtonsoft.Json.Linq;
using SecretNET.Crypto;

namespace SecretNET.AccessControl;

/// <summary>
/// Enum PermissionType
/// </summary>
public enum PermissionType
{
    Owner = 0,
    History = 1,
    Balance = 2,
    Allowance = 3
}

/// <summary>
/// Class PermitSigner.
/// </summary>
public class PermitSigner
{
    readonly IAminoSigner _signer = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="PermitSigner"/> class.
    /// </summary>
    /// <param name="signer">The signer.</param>
    public PermitSigner(IAminoSigner signer)
	{
        _signer = signer;
    }

    /// <summary>
    /// Signs the specified owner.
    /// </summary>
    /// <param name="owner">The adrress of the owner / approver.</param>
    /// <param name="chainId">The chain identifier.</param>
    /// <param name="permitName">A free-form string. The user can later revoke this permit using this name..</param>
    /// <param name="allowedContracts">A list of contract / token addresses to which this permit applies.</param>
    /// <param name="permissions">An array that may contain balance, history and allowance.</param>
    /// <returns>Permit.</returns>
    public async Task<Permit> Sign(string owner, string chainId, string permitName, string[] allowedContracts, PermissionType[] permissions)
    {
        var stdSignDoc = new StdSignDoc()
        {
            ChainId = chainId,
            AccountNumber = "0",    // Must be 0
            Sequence = "0",         // Must be 0
            Fee = new StdFeeAmino(new Tx.Coin()
            {
                Amount = "0"        // Must be 0
            },
            gas: "1"),               // Must be 1
            Msgs = new AminoMsg[]
            {
                new AminoMsg("query_permit") // Must be "query_permit"
                {
                    Value = new
                    {
                        permit_name = permitName,
                        allowed_tokens = allowedContracts,
                        permissions = permissions.Select(p => p.ToString().ToLower()).ToArray()
                    }
                }
            },
            Memo = ""   // Must be empty
        };

        var signResponse = await _signer.SignAmino(stdSignDoc, owner);

        var permit = new Permit()
        {
            Params = new Permit_Params()
            {
                ChainId = chainId,
                PermitName = permitName,
                AllowedTokens = allowedContracts,
                Permissions = permissions.Select(p => p.ToString().ToLower()).ToArray()
            },
            Signature = signResponse
        };

        return permit;
    }

    /// <summary>
    /// This method will verify a permit according to a contract address and a submitting address (and a set of permissions).
    /// On failure an appropriate error will be thrown according to the type of error.
    /// </summary>
    /// <param name="permit">The permit.</param>
    /// <param name="address">The address.</param>
    /// <param name="contract">The contract.</param>
    /// <param name="permissions">The permissions.</param>
    /// <param name="throwException">if set to <c>true</c> [throw exception].</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool Verify(Permit permit, string address, string contract, PermissionType[] permissions, bool throwException = true)
    {
        return ValidatePermit(permit, address, contract, permissions, throwException);
    }



    /// <summary>
    /// Validates the permit.
    /// </summary>
    /// <param name="permit">The permit.</param>
    /// <param name="address">The address.</param>
    /// <param name="contract">The contract.</param>
    /// <param name="permissions">The permissions.</param>
    /// <param name="throwException">if set to <c>true</c> [exceptions].</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    private bool ValidatePermit(Permit permit, string address, string contract, PermissionType[] permissions, bool throwException = true)
    {
        var isValid = false;
        try
        {
            // check if contract is valid
            var contractInPermit = permit.Params.AllowedTokens.Contains(contract);

            if (!contractInPermit)
            {
                if (!throwException)
                {
                    return false;
                }
                throw new ContractNotInPermitException(contract, permit.Params.AllowedTokens);
            }

            // check if permissions included
            var permissionInPermit = permissions.Select(x => x.ToString().ToLower()).Intersect(permit.Params.Permissions).Count() == permissions.Count();
            if (!permissionInPermit)
            {
                if (!throwException)
                {
                    return false;
                }
                throw new PermissionNotInPermitException(permissions, permit.Params.Permissions);
            }

            // decode address
            try
            {
                var bech32encoder = Encoders.Bech32("secret");
                var bech32Words = bech32encoder.DecodeDataRaw(address, out var encodingType);
                var bech32FromWords = bech32encoder.ConvertBits(bech32Words, 5, 8, false);
            }
            catch
            {
                throw new Exception($"Address address ={address} must be a valid bech32 address");
            }

            // check address
            var permitAcc = "";
            try
            {
                permitAcc = SecretNetworkClient.Base64PubkeyToAddress(permit.Signature.PubKey.AsBase64);
            }
            catch (Exception)
            {
                if (!throwException)
                {
                    return false;
                }
                throw new PermitException("Pubkey invalid");
            }

            if (permitAcc != address)
            {
                if (!throwException)
                {
                    return false;
                }
                throw new SignerIsNotAddressException(permit.Signature.PubKey, address);
            }

            isValid = ValidateSignature(permit);

        }
        catch (Exception)
        {
            if (throwException)
            {
                throw;
            }
        }

        return isValid;
    }

    /// <summary>
    /// Validates the signature.
    /// </summary>
    /// <param name="permit">The permit.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    private bool ValidateSignature(Permit permit)
    {
        var isValid = false;
        var signDoc = new StdSignDoc()
        {
            ChainId = permit.Params.ChainId,
            AccountNumber = "0",    // Must be 0
            Sequence = "0",         // Must be 0
            Fee = new StdFeeAmino(new Tx.Coin()
            {
                Amount = "0"        // Must be 0
            },
            gas: "1"),               // Must be 1
            Msgs = new AminoMsg[]
            {
                new AminoMsg("query_permit") // Must be "query_permit"
                {
                    Value = new
                    {
                        permit_name = permit.Params.PermitName,
                        allowed_tokens = permit.Params.AllowedTokens,
                        permissions = permit.Params.Permissions.Select(p => p.ToString().ToLower()).ToArray()
                    }
                }
            },
            Memo = ""   // Must be empty
        };

        var jObj = (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(signDoc));
        Utils.SortJObject(jObj);
        var sortedJson = jObj.ToString(Formatting.None);
        var serializedSignDoc = Encoding.UTF8.GetBytes(sortedJson);
        var sha256Hash = Hashes.SHA256(serializedSignDoc);
        var messageHash = new uint256(sha256Hash);

        if(ECDSASignature.TryParseFromCompact(Convert.FromBase64String(permit.Signature.Signature), out ECDSASignature signature))
        {
            var pubKey = new PubKey(Convert.FromBase64String(permit.Signature.PubKey.AsBase64));
            isValid = pubKey.Verify(messageHash, signature);
        }

        return isValid;
    }

}
