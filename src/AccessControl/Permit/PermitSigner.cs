using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
