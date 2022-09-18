using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretNET.Tx;

internal static class MsgDecoderRegistry
{
    internal static readonly ConcurrentDictionary<string, Func<Any, IMessage>> Registry = new ConcurrentDictionary<string, Func<Any, IMessage>>();

    static MsgDecoderRegistry()
	{
        // Cosmos.Authz.V1Beta1        
        Registry.TryAdd(MsgGrantAuthorization.MsgGrant.ToLower(), (any) => any.Unpack<Cosmos.Authz.V1Beta1.MsgGrant>());
        Registry.TryAdd(MsgGrantAuthorization.MsgExec.ToLower(), (any) => any.Unpack<Cosmos.Authz.V1Beta1.MsgExec>());
        Registry.TryAdd(MsgGrantAuthorization.MsgRevoke.ToLower(), (any) => any.Unpack<Cosmos.Authz.V1Beta1.MsgRevoke>());
        Registry.TryAdd(MsgTypeNames.GenericAuthorization.ToLower(), (any) => any.Unpack<Cosmos.Authz.V1Beta1.GenericAuthorization>());

        // Cosmos.Bank.V1Beta1
        Registry.TryAdd(MsgGrantAuthorization.MsgSend.ToLower(), (any) => any.Unpack<Cosmos.Bank.V1Beta1.MsgSend>());
        Registry.TryAdd(MsgTypeNames.SendAuthorization.ToLower(), (any) => any.Unpack<Cosmos.Bank.V1Beta1.SendAuthorization>());
        Registry.TryAdd(MsgGrantAuthorization.MsgMultiSend.ToLower(), (any) => any.Unpack<Cosmos.Bank.V1Beta1.MsgMultiSend>());

        // Secret.Compute.V1Beta1
        Registry.TryAdd(MsgGrantAuthorization.MsgExecuteContract.ToLower(), (any) => any.Unpack<Secret.Compute.V1Beta1.MsgExecuteContract>());
        Registry.TryAdd(MsgGrantAuthorization.MsgInstantiateContract.ToLower(), (any) => any.Unpack<Secret.Compute.V1Beta1.MsgInstantiateContract>());
        Registry.TryAdd(MsgGrantAuthorization.MsgStoreCode.ToLower(), (any) => any.Unpack<Secret.Compute.V1Beta1.MsgStoreCode>());

        // Cosmos.Crisis.V1Beta1
        Registry.TryAdd(MsgGrantAuthorization.MsgVerifyInvariant.ToLower(), (any) => any.Unpack<Cosmos.Crisis.V1Beta1.MsgVerifyInvariant>());

        // Cosmos.Distribution.V1Beta1
        Registry.TryAdd(MsgGrantAuthorization.MsgSetWithdrawAddress.ToLower(), (any) => any.Unpack<Cosmos.Distribution.V1Beta1.MsgSetWithdrawAddress>());
        Registry.TryAdd(MsgGrantAuthorization.MsgWithdrawDelegatorReward.ToLower(), (any) => any.Unpack<Cosmos.Distribution.V1Beta1.MsgWithdrawDelegatorReward>());
        Registry.TryAdd(MsgGrantAuthorization.MsgWithdrawValidatorCommission.ToLower(), (any) => any.Unpack<Cosmos.Distribution.V1Beta1.MsgWithdrawValidatorCommission>());
        Registry.TryAdd(MsgGrantAuthorization.MsgFundCommunityPool.ToLower(), (any) => any.Unpack<Cosmos.Distribution.V1Beta1.MsgFundCommunityPool>());

        // Cosmos.Evidence.V1Beta1
        Registry.TryAdd(MsgGrantAuthorization.MsgSubmitEvidence.ToLower(), (any) => any.Unpack<Cosmos.Evidence.V1Beta1.MsgSubmitEvidence>());

        // Cosmos.Feegrant.V1Beta1
        Registry.TryAdd(MsgGrantAuthorization.MsgGrantAllowance.ToLower(), (any) => any.Unpack<Cosmos.Feegrant.V1Beta1.MsgGrantAllowance>());
        Registry.TryAdd(MsgGrantAuthorization.MsgRevokeAllowance.ToLower(), (any) => any.Unpack<Cosmos.Feegrant.V1Beta1.MsgRevokeAllowance>());

        // Cosmos.Gov.V1Beta1
        Registry.TryAdd(MsgGrantAuthorization.MsgSubmitProposal.ToLower(), (any) => any.Unpack<Cosmos.Gov.V1Beta1.MsgSubmitProposal>());
        Registry.TryAdd(MsgGrantAuthorization.MsgVote.ToLower(), (any) => any.Unpack<Cosmos.Gov.V1Beta1.MsgVote>());
        Registry.TryAdd(MsgGrantAuthorization.MsgVoteWeighted.ToLower(), (any) => any.Unpack<Cosmos.Gov.V1Beta1.MsgVoteWeighted>());
        Registry.TryAdd(MsgGrantAuthorization.MsgDeposit.ToLower(), (any) => any.Unpack<Cosmos.Gov.V1Beta1.MsgDeposit>());

        // Ibc.Core.Channel.V1
        Registry.TryAdd(MsgGrantAuthorization.MsgRecvPacket.ToLower(), (any) => any.Unpack<Ibc.Core.Channel.V1.MsgRecvPacket>());
        Registry.TryAdd(MsgGrantAuthorization.MsgTimeout.ToLower(), (any) => any.Unpack<Ibc.Core.Channel.V1.MsgTimeout>());
        Registry.TryAdd(MsgGrantAuthorization.MsgTimeoutOnClose.ToLower(), (any) => any.Unpack<Ibc.Core.Channel.V1.MsgTimeoutOnClose>());
        Registry.TryAdd(MsgGrantAuthorization.MsgChannelOpenInit.ToLower(), (any) => any.Unpack<Ibc.Core.Channel.V1.MsgChannelOpenInit>());
        Registry.TryAdd(MsgGrantAuthorization.MsgAcknowledgement.ToLower(), (any) => any.Unpack<Ibc.Core.Channel.V1.MsgAcknowledgement>());
        Registry.TryAdd(MsgGrantAuthorization.MsgChannelOpenTry.ToLower(), (any) => any.Unpack<Ibc.Core.Channel.V1.MsgChannelOpenTry>());
        Registry.TryAdd(MsgGrantAuthorization.MsgChannelOpenAck.ToLower(), (any) => any.Unpack<Ibc.Core.Channel.V1.MsgChannelOpenAck>());
        Registry.TryAdd(MsgGrantAuthorization.MsgChannelOpenConfirm.ToLower(), (any) => any.Unpack<Ibc.Core.Channel.V1.MsgChannelOpenConfirm>());
        Registry.TryAdd(MsgGrantAuthorization.MsgChannelCloseInit.ToLower(), (any) => any.Unpack<Ibc.Core.Channel.V1.MsgChannelCloseInit>());
        Registry.TryAdd(MsgGrantAuthorization.MsgChannelCloseConfirm.ToLower(), (any) => any.Unpack<Ibc.Core.Channel.V1.MsgChannelCloseConfirm>());

        // Ibc.Core.Client.V1
        Registry.TryAdd(MsgGrantAuthorization.MsgUpgradeClient.ToLower(), (any) => any.Unpack<Ibc.Core.Client.V1.MsgUpgradeClient>());
        Registry.TryAdd(MsgGrantAuthorization.MsgSubmitMisbehaviour.ToLower(), (any) => any.Unpack<Ibc.Core.Client.V1.MsgSubmitMisbehaviour>());
        Registry.TryAdd(MsgGrantAuthorization.MsgCreateClient.ToLower(), (any) => any.Unpack<Ibc.Core.Client.V1.MsgCreateClient>());

        // Ibc.Applications.Transfer.V1
        Registry.TryAdd(MsgGrantAuthorization.MsgTransfer.ToLower(), (any) => any.Unpack<Ibc.Applications.Transfer.V1.MsgTransfer>());

        // Secret.Registration.V1Beta1
        Registry.TryAdd(MsgTypeNames.RaAuthenticate.ToLower(), (any) => any.Unpack<Secret.Registration.V1Beta1.RaAuthenticate>());

        // Cosmos.Slashing.V1Beta1
        Registry.TryAdd(MsgGrantAuthorization.MsgUnjail.ToLower(), (any) => any.Unpack<Cosmos.Slashing.V1Beta1.MsgUnjail>());

        // Cosmos.Staking.V1Beta1
        Registry.TryAdd(MsgGrantAuthorization.MsgCreateValidator.ToLower(), (any) => any.Unpack<Cosmos.Staking.V1Beta1.MsgCreateValidator>());
        Registry.TryAdd(MsgGrantAuthorization.MsgEditValidator.ToLower(), (any) => any.Unpack<Cosmos.Staking.V1Beta1.MsgEditValidator>());
        Registry.TryAdd(MsgGrantAuthorization.MsgDelegate.ToLower(), (any) => any.Unpack<Cosmos.Staking.V1Beta1.MsgDelegate>());
        Registry.TryAdd(MsgGrantAuthorization.MsgBeginRedelegate.ToLower(), (any) => any.Unpack<Cosmos.Staking.V1Beta1.MsgBeginRedelegate>());
        Registry.TryAdd(MsgGrantAuthorization.MsgUndelegate.ToLower(), (any) => any.Unpack<Cosmos.Staking.V1Beta1.MsgBeginRedelegate>());
        Registry.TryAdd(MsgTypeNames.StakeAuthorization.ToLower(), (any) => any.Unpack<Cosmos.Staking.V1Beta1.StakeAuthorization>());

        // Cosmos.Vesting.V1Beta1
        Registry.TryAdd(MsgTypeNames.MsgCreateVestingAccount.ToLower(), (any) => any.Unpack<Cosmos.Vesting.V1Beta1.MsgCreateVestingAccount>());


        

        
    }

    public static Func<Any, IMessage> Get(string typeUrl)
    {
        if (!String.IsNullOrEmpty(typeUrl))
        {
            var regKey = typeUrl.CleanUpTypeurl().ToLower();
            if (Registry.ContainsKey(regKey))
            {
                return Registry[regKey];
            }
        }
        return null;
    }
}
