using System.Reflection;

namespace SecretNET.Tx;

public static class MsgGrantAuthorization
{
    // Secret
    public const string MsgStoreCode =                          "secret.compute.v1beta1.MsgStoreCode";
    public const string MsgInstantiateContract =                "secret.compute.v1beta1.MsgInstantiateContract";
    public const string MsgExecuteContract =                    "secret.compute.v1beta1.MsgExecuteContract";

    // Cosmos
    public const string MsgExec =                               "cosmos.authz.v1beta1.MsgExec";
    public const string MsgGrant =                              "cosmos.authz.v1beta1.MsgGrant";
    public const string MsgRevoke =                             "cosmos.authz.v1beta1.MsgRevoke";

    public const string MsgMultiSend =                          "cosmos.bank.v1beta1.MsgMultiSend";
    public const string MsgSend =                               "cosmos.bank.v1beta1.MsgSend";

    public const string MsgDeposit =                            "cosmos.gov.v1beta1.MsgDeposit";
    public const string MsgSubmitProposal =                     "cosmos.gov.v1beta1.MsgSubmitProposal";
    public const string MsgVote =                               "cosmos.gov.v1beta1.MsgVote";
    public const string MsgVoteWeighted =                       "cosmos.gov.v1beta1.MsgVoteWeighted";

    public const string MsgBeginRedelegate =                    "cosmos.staking.v1beta1.MsgBeginRedelegate";
    public const string MsgCreateValidator =                    "cosmos.staking.v1beta1.MsgCreateValidator";
    public const string MsgDelegate =                           "cosmos.staking.v1beta1.MsgDelegate";
    public const string MsgEditValidator =                      "cosmos.staking.v1beta1.MsgEditValidator";
    public const string MsgUndelegate =                         "cosmos.staking.v1beta1.MsgUndelegate";

    public const string MsgUnjail =                             "cosmos.slashing.v1beta1.MsgUnjail";

    public const string MsgFundCommunityPool =                  "cosmos.distribution.v1beta1.MsgFundCommunityPool";
    public const string MsgSetWithdrawAddress =                 "cosmos.distribution.v1beta1.MsgSetWithdrawAddress";
    public const string MsgWithdrawDelegatorReward =            "cosmos.distribution.v1beta1.MsgWithdrawDelegatorReward";
    public const string MsgWithdrawValidatorCommission =        "cosmos.distribution.v1beta1.MsgWithdrawValidatorCommission";

    public const string MsgGrantAllowance =                     "cosmos.feegrant.v1beta1.MsgGrantAllowance";
    public const string MsgRevokeAllowance =                    "cosmos.feegrant.v1beta1.MsgRevokeAllowance";
    public const string MsgSubmitEvidence =                     "cosmos.evidence.v1beta1.MsgSubmitEvidence";

    public const string MsgVerifyInvariant =                    "cosmos.crisis.v1beta1.MsgVerifyInvariant";

    // IBC
    public const string MsgTransfer =                           "ibc.applications.transfer.v1.MsgTransfer";

    public const string MsgAcknowledgement =                    "ibc.core.channel.v1.MsgAcknowledgement";
    public const string MsgChannelCloseConfirm =                "ibc.core.channel.v1.MsgChannelCloseConfirm";
    public const string MsgChannelCloseInit =                   "ibc.core.channel.v1.MsgChannelCloseInit";
    public const string MsgChannelOpenAck =                     "ibc.core.channel.v1.MsgChannelOpenAck";
    public const string MsgChannelOpenConfirm =                 "ibc.core.channel.v1.MsgChannelOpenConfirm";
    public const string MsgChannelOpenInit =                    "ibc.core.channel.v1.MsgChannelOpenInit";
    public const string MsgChannelOpenTry =                     "ibc.core.channel.v1.MsgChannelOpenTry";
    public const string MsgRecvPacket =                         "ibc.core.channel.v1.MsgRecvPacket";
    public const string MsgTimeout =                            "ibc.core.channel.v1.MsgTimeout";
    public const string MsgTimeoutOnClose =                     "ibc.core.channel.v1.MsgTimeoutOnClose";

    public const string MsgConnectionOpenAck =                  "ibc.core.connection.v1.MsgConnectionOpenAck";
    public const string MsgConnectionOpenConfirm =              "ibc.core.connection.v1.MsgConnectionOpenConfirm";
    public const string MsgConnectionOpenInit =                 "ibc.core.connection.v1.MsgConnectionOpenInit";
    public const string MsgConnectionOpenTry =                  "ibc.core.connection.v1.MsgConnectionOpenTry";

    public const string MsgCreateClient =                       "ibc.core.client.v1.MsgCreateClient";
    public const string MsgSubmitMisbehaviour =                 "ibc.core.client.v1.MsgSubmitMisbehaviour";
    public const string MsgUpdateClient =                       "ibc.core.client.v1.MsgUpdateClient";
    public const string MsgUpgradeClient =                      "ibc.core.client.v1.MsgUpgradeClient";

    public static List<string> GrantMessageTypes;

    static MsgGrantAuthorization()
    {
        GrantMessageTypes = typeof(MsgGrantAuthorization)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
            .Select(x => Convert.ToString(x.GetRawConstantValue()))
            .ToList();
    }
}

public static class MsgTypeNames
{
    // Cosmos Auth
    public const string SendAuthorization =                     "cosmos.bank.v1beta1.SendAuthorization";
    public const string StakeAuthorization =                    "cosmos.staking.v1beta1.StakeAuthorization";
    public const string GenericAuthorization =                  "cosmos.authz.v1beta1.GenericAuthorization";
    public const string RaAuthenticate =                        "secret.registration.v1beta1.RaAuthenticate";
    public const string MsgCreateVestingAccount =               "cosmos.vesting.v1beta1.MsgCreateVestingAccount";
}