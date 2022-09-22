using Cosmos.Authz.V1Beta1;
using System.Net;

namespace SecretNET.Tx;

public class SendAuthorization : MsgBase
{
    public override string MsgType { get; } = MsgTypeNames.SendAuthorization;

    public Coin[] Coins { get; set; }

    public override async Task<IMessage> ToProto()
    {
        var sendAuthorization = new Cosmos.Bank.V1Beta1.SendAuthorization();
        sendAuthorization.SpendLimit.Add(Coins.Select(c => new Cosmos.Base.V1Beta1.Coin() { Amount = c.Amount, Denom = c.Denom }).ToArray());

        return sendAuthorization;
    }

    public override async Task<AminoMsg> ToAmino()
    {
        throw new Exception("SendAuthorization ToAmino is not implemented.");
    }
}

/// <summary>
/// AuthorizationType defines the type of staking module authorization type
/// </summary>
public enum StakeAuthorizationType : byte
{
    Unspecified = 0,
    /// <summary>
    /// defines an authorization type for Msg/Delegate
    /// </summary>
    Delegate = 1,
    /// <summary>
    /// defines an authorization type for Msg/Undelegate
    /// </summary>
    Undelegate = 2,
    /// <summary>
    /// defines an authorization type for Msg/BeginRedelegate
    /// </summary>
    Redelegate = 3,
}

public class StakeAuthorization : MsgBase
{
    public override string MsgType { get; } = MsgTypeNames.StakeAuthorization;

    /// <summary>
    /// max_tokens specifies the maximum amount of tokens can be delegate to a validator.
    /// If it is empty, there is no spend limit and any amount of coins can be delegated.
    /// </summary>
    public Coin MaxTokens { get; set; }

    /// <summary>
    /// allow_list specifies list of validator addresses to whom grantee can delegate tokens on behalf of granter's account.
    /// </summary>
    public string[] AllowList { get; set; }

    /// <summary>
    /// deny_list specifies list of validator addresses to whom grantee can not delegate tokens.
    /// </summary>
    public string[] DenyList { get; set; }

    /// <summary>
    /// deny_list specifies list of validator addresses to whom grantee can not delegate tokens.
    /// </summary>
    public StakeAuthorizationType AuthorizationType { get; set; }

    public override async Task<IMessage> ToProto()
    {
        var sendAuthorization = new Cosmos.Staking.V1Beta1.StakeAuthorization();
        if (MaxTokens != null)
        {
            sendAuthorization.MaxTokens = new Cosmos.Base.V1Beta1.Coin()
            {
                Denom = MaxTokens.Denom,
                Amount = MaxTokens.Amount,
            };
        }
        if (AllowList != null && AllowList.Length > 0)
        {
            foreach (var i in AllowList)
            {
                sendAuthorization.AllowList = new Cosmos.Staking.V1Beta1.StakeAuthorization.Types.Validators();
                sendAuthorization.AllowList.Address.Add(i.ToString());
            }
        }
        if (DenyList != null && DenyList.Length > 0)
        {
            foreach (var i in DenyList)
            {
                sendAuthorization.DenyList = new Cosmos.Staking.V1Beta1.StakeAuthorization.Types.Validators();
                sendAuthorization.DenyList.Address.Add(i.ToString());
            }
        }
        sendAuthorization.AuthorizationType = (Cosmos.Staking.V1Beta1.AuthorizationType)(byte)AuthorizationType;

        return sendAuthorization;
    }

    public override async Task<AminoMsg> ToAmino()
    {
        throw new Exception("SendAuthorization ToAmino is not implemented.");
    }
}

/// <summary>
/// MsgGrant is a request type for Grant method. It declares authorization to the grantee
/// on behalf of the granter with the provided expiration time.
/// </summary>
public class MsgGrant : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgGrant;

    public string Granter { get; set; }
    public string Grantee { get; set; }

    public MsgBase Authorization { get; set; }

    /// <summary>
    /// Represents seconds of UTC time since Unix epoch 1970-01-01T00:00:00Z.
    /// => use DateTime.UnixEpoch
    /// </summary>
    public ulong Expiration { get; set; }

    public override async Task<IMessage> ToProto()
    {
        Cosmos.Authz.V1Beta1.MsgGrant msgGrant = null;

        if (Authorization != null)
        {
            // SendAuthorization or StakeAuthorization
            if (Authorization.MsgType.IsProtoType(MsgTypeNames.SendAuthorization) ||
                Authorization.MsgType.IsProtoType(MsgTypeNames.StakeAuthorization))
            {
                msgGrant = new Cosmos.Authz.V1Beta1.MsgGrant
                {
                     Grant = new Cosmos.Authz.V1Beta1.Grant()
                     {
                         Authorization = Any.Pack((await Authorization.ToProto()),""),
                         Expiration = new Timestamp()
                         {
                             Seconds = (long)Expiration
                         }
                     }
                };
            }
            // GenericAuthorization
            if (MsgGrantAuthorization.GrantMessageTypes.Contains(Authorization.MsgType))
            {
                var genericAuthorization = new Cosmos.Authz.V1Beta1.GenericAuthorization()
                {
                    Msg = Authorization.MsgType
                };
                msgGrant = new Cosmos.Authz.V1Beta1.MsgGrant
                {
                    Grant = new Cosmos.Authz.V1Beta1.Grant()
                    {
                        Authorization = Any.Pack(genericAuthorization,""),
                        Expiration = new Timestamp()
                        {
                            Seconds = (long)Expiration
                        }
                    }
                };                
            }

            if (msgGrant != null)
            {
                msgGrant.Granter = Granter;
                msgGrant.Grantee = Grantee;

                return msgGrant;
            }
        }
        return null;
    }

    public override async Task<AminoMsg> ToAmino()
    {
        throw new Exception("MsgGrant ToAmino is not implemented.");
    }
}

/// <summary>
/// MsgExec attempts to execute the provided messages using authorizations granted to the grantee. 
/// Each message should have only one signer corresponding to the granter of the authorization.
/// </summary>
public class MsgExec : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgExec;

    public string Grantee { get; set; }

    /// <summary>
    /// Authorization Msg requests to execute. Each msg must implement Authorization interface
    /// The x/authz will try to find a grant matching (msg.signers[0], grantee, MsgTypeURL(msg))
    /// triple and validate it.
    /// </summary>
    public MsgBase[] AuthorizationMsgs { get; set; }

    public override async Task<IMessage> ToProto()
    {
        if (AuthorizationMsgs != null && AuthorizationMsgs.Length > 0)
        {
            var msgExec = new Cosmos.Authz.V1Beta1.MsgExec()
            {
                Grantee = Grantee
            };

            var protoMsgs = new List<Any>();
            foreach (var m in AuthorizationMsgs)
            {
                var protoMsg = await m.ToProto();
                protoMsgs.Add(Any.Pack(protoMsg, ""));
            }

            msgExec.Msgs.Add(protoMsgs.ToArray());

            return msgExec;
        }
        return null;
    }

    public override Task<AminoMsg> ToAmino()
    {
        throw new Exception("MsgExec ToAmino is not implemented.");
    }
}

/// <summary>
/// MsgRevoke revokes any authorization with the provided sdk.Msg type on the
/// granter's account with that has been granted to the grantee.
/// </summary>
public class MsgRevoke : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgRevoke;

    public string Granter { get; set; }
    public string Grantee { get; set; }

    /// <summary>
    /// revokes any authorization with the provided sdk.Msg type on the
    /// granter's account with that has been granted to the grantee.
    /// </summary>
    public MsgBase RevokeAuthorization { get; set; }

    /// <summary>
    /// Authorization Msg requests to execute. Each msg must implement Authorization interface
    /// The x/authz will try to find a grant matching (msg.signers[0], grantee, MsgTypeURL(msg))
    /// triple and validate it.
    /// </summary>
    public MsgBase[] AuthorizationMsgs { get; set; }

    public override async Task<IMessage> ToProto()
    {
        if (RevokeAuthorization != null)
        {
            if (MsgGrantAuthorization.GrantMessageTypes.Contains(RevokeAuthorization.MsgType))
            {
                
                var msgRevoke = new Cosmos.Authz.V1Beta1.MsgRevoke
                {
                  Grantee = Grantee,
                  Granter = Granter,
                  MsgTypeUrl = RevokeAuthorization.MsgType
                };

                return msgRevoke;
            }
        }
        return null;
    }

    public override Task<AminoMsg> ToAmino()
    {
        throw new NotImplementedException("MsgRevoke ToAmino is not implemented.");
    }
}