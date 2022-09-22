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
    internal static readonly ConcurrentDictionary<System.Type, Func<byte[], object>> TypeRegistry = new ConcurrentDictionary<System.Type, Func<byte[], object>>();

    static MsgDecoderRegistry()
	{

        #region Response message by type => T.Parser.ParseFrom

        // Cosmos.Authz.V1Beta1
        TypeRegistry.TryAdd(typeof(Cosmos.Authz.V1Beta1.MsgGrantResponse), Cosmos.Authz.V1Beta1.MsgGrantResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Cosmos.Authz.V1Beta1.MsgExecResponse), Cosmos.Authz.V1Beta1.MsgExecResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Cosmos.Authz.V1Beta1.MsgRevokeResponse), Cosmos.Authz.V1Beta1.MsgRevokeResponse.Parser.ParseFrom);

        // Cosmos.Bank.V1Beta1
        TypeRegistry.TryAdd(typeof(Cosmos.Bank.V1Beta1.MsgSendResponse), Cosmos.Bank.V1Beta1.MsgSendResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Cosmos.Bank.V1Beta1.MsgMultiSendResponse), Cosmos.Bank.V1Beta1.MsgMultiSendResponse.Parser.ParseFrom);

        // Secret.Compute.V1Beta1
        TypeRegistry.TryAdd(typeof(Secret.Compute.V1Beta1.MsgStoreCodeResponse), Secret.Compute.V1Beta1.MsgStoreCodeResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Secret.Compute.V1Beta1.MsgInstantiateContractResponse), Secret.Compute.V1Beta1.MsgInstantiateContractResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Secret.Compute.V1Beta1.MsgExecuteContractResponse), Secret.Compute.V1Beta1.MsgExecuteContractResponse.Parser.ParseFrom);

        // Cosmos.Crisis.V1Beta1
        TypeRegistry.TryAdd(typeof(Cosmos.Crisis.V1Beta1.MsgVerifyInvariantResponse), Cosmos.Crisis.V1Beta1.MsgVerifyInvariantResponse.Parser.ParseFrom);

        // Cosmos.Distribution.V1Beta1
        TypeRegistry.TryAdd(typeof(Cosmos.Distribution.V1Beta1.MsgSetWithdrawAddressResponse), Cosmos.Distribution.V1Beta1.MsgSetWithdrawAddressResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Cosmos.Distribution.V1Beta1.MsgWithdrawDelegatorRewardResponse), Cosmos.Distribution.V1Beta1.MsgWithdrawDelegatorRewardResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Cosmos.Distribution.V1Beta1.MsgWithdrawValidatorCommissionResponse), Cosmos.Distribution.V1Beta1.MsgWithdrawValidatorCommissionResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Cosmos.Distribution.V1Beta1.MsgFundCommunityPoolResponse), Cosmos.Distribution.V1Beta1.MsgFundCommunityPoolResponse.Parser.ParseFrom);

        // Cosmos.Evidence.V1Beta1
        TypeRegistry.TryAdd(typeof(Cosmos.Evidence.V1Beta1.MsgSubmitEvidenceResponse), Cosmos.Evidence.V1Beta1.MsgSubmitEvidenceResponse.Parser.ParseFrom);

        // Cosmos.Feegrant.V1Beta1
        TypeRegistry.TryAdd(typeof(Cosmos.Feegrant.V1Beta1.MsgGrantAllowanceResponse), Cosmos.Feegrant.V1Beta1.MsgGrantAllowanceResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Cosmos.Feegrant.V1Beta1.MsgRevokeAllowanceResponse), Cosmos.Feegrant.V1Beta1.MsgRevokeAllowanceResponse.Parser.ParseFrom);

        // Cosmos.Gov.V1Beta1
        TypeRegistry.TryAdd(typeof(Cosmos.Gov.V1Beta1.MsgSubmitProposalResponse), Cosmos.Gov.V1Beta1.MsgSubmitProposalResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Cosmos.Gov.V1Beta1.MsgVoteResponse), Cosmos.Gov.V1Beta1.MsgVoteResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Cosmos.Gov.V1Beta1.MsgVoteWeightedResponse), Cosmos.Gov.V1Beta1.MsgVoteWeightedResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Cosmos.Gov.V1Beta1.MsgDepositResponse), Cosmos.Gov.V1Beta1.MsgDepositResponse.Parser.ParseFrom);

        // Ibc.Core.Channel.V1
        TypeRegistry.TryAdd(typeof(Ibc.Core.Channel.V1.MsgRecvPacketResponse), Ibc.Core.Channel.V1.MsgRecvPacketResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Ibc.Core.Channel.V1.MsgTimeoutResponse), Ibc.Core.Channel.V1.MsgTimeoutResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Ibc.Core.Channel.V1.MsgTimeoutOnCloseResponse), Ibc.Core.Channel.V1.MsgTimeoutOnCloseResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Ibc.Core.Channel.V1.MsgChannelOpenInitResponse), Ibc.Core.Channel.V1.MsgChannelOpenInitResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Ibc.Core.Channel.V1.MsgAcknowledgementResponse), Ibc.Core.Channel.V1.MsgAcknowledgementResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Ibc.Core.Channel.V1.MsgChannelOpenTryResponse), Ibc.Core.Channel.V1.MsgChannelOpenTryResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Ibc.Core.Channel.V1.MsgChannelOpenAckResponse), Ibc.Core.Channel.V1.MsgChannelOpenAckResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Ibc.Core.Channel.V1.MsgChannelOpenConfirmResponse), Ibc.Core.Channel.V1.MsgChannelOpenConfirmResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Ibc.Core.Channel.V1.MsgChannelCloseInitResponse), Ibc.Core.Channel.V1.MsgChannelCloseInitResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Ibc.Core.Channel.V1.MsgChannelCloseConfirmResponse), Ibc.Core.Channel.V1.MsgChannelCloseConfirmResponse.Parser.ParseFrom);

        // Ibc.Core.Client.V1
        TypeRegistry.TryAdd(typeof(Ibc.Core.Client.V1.MsgUpdateClientResponse), Ibc.Core.Client.V1.MsgUpdateClientResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Ibc.Core.Client.V1.MsgUpgradeClientResponse), Ibc.Core.Client.V1.MsgUpgradeClientResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Ibc.Core.Client.V1.MsgSubmitMisbehaviourResponse), Ibc.Core.Client.V1.MsgSubmitMisbehaviourResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Ibc.Core.Client.V1.MsgCreateClientResponse), Ibc.Core.Client.V1.MsgCreateClientResponse.Parser.ParseFrom);        

        // Ibc.Applications.Transfer.V1
        TypeRegistry.TryAdd(typeof(Ibc.Applications.Transfer.V1.MsgTransferResponse), Ibc.Applications.Transfer.V1.MsgTransferResponse.Parser.ParseFrom);

        // Cosmos.Slashing.V1Beta1
        TypeRegistry.TryAdd(typeof(Cosmos.Slashing.V1Beta1.MsgUnjailResponse), Cosmos.Slashing.V1Beta1.MsgUnjailResponse.Parser.ParseFrom);

        // Cosmos.Staking.V1Beta1
        TypeRegistry.TryAdd(typeof(Cosmos.Staking.V1Beta1.MsgCreateValidatorResponse), Cosmos.Staking.V1Beta1.MsgCreateValidatorResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Cosmos.Staking.V1Beta1.MsgEditValidatorResponse), Cosmos.Staking.V1Beta1.MsgEditValidatorResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Cosmos.Staking.V1Beta1.MsgDelegateResponse), Cosmos.Staking.V1Beta1.MsgDelegateResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Cosmos.Staking.V1Beta1.MsgBeginRedelegateResponse), Cosmos.Staking.V1Beta1.MsgBeginRedelegateResponse.Parser.ParseFrom);
        TypeRegistry.TryAdd(typeof(Cosmos.Staking.V1Beta1.MsgUndelegateResponse), Cosmos.Staking.V1Beta1.MsgUndelegateResponse.Parser.ParseFrom);

        // Cosmos.Vesting.V1Beta1
        TypeRegistry.TryAdd(typeof(Cosmos.Vesting.V1Beta1.MsgCreateVestingAccountResponse), Cosmos.Vesting.V1Beta1.MsgCreateVestingAccountResponse.Parser.ParseFrom);

        #endregion

        #region Message by type name => any.Unpack<T>

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

        #endregion

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

    public static Func<byte[], object> Get(System.Type msgType)
    {
        if (msgType != null)
        {
            if (TypeRegistry.ContainsKey(msgType))
            {
                return TypeRegistry[msgType];
            }
        }
        return null;
    }
}
