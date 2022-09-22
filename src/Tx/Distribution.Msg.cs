namespace SecretNET.Tx;

/// <summary>
/// MsgSetWithdrawAddress sets the withdraw address for a delegator (or validator self-delegation).
/// </summary>
public class MsgSetWithdrawAddress : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgSetWithdrawAddress;

    public string DelegatorAddress { get; set; }

    public string WithdrawAddress { get; set; }


    public MsgSetWithdrawAddress() { }

    public MsgSetWithdrawAddress(string delegatorAddress, string withdrawAddress)
    {
        DelegatorAddress = delegatorAddress;
        WithdrawAddress = withdrawAddress;
    }


    public override async Task<IMessage> ToProto()
    {
        var msgSetWithdrawAddress = new Cosmos.Distribution.V1Beta1.MsgSetWithdrawAddress()
        {
            DelegatorAddress = DelegatorAddress,
            WithdrawAddress = WithdrawAddress,
        };

        return msgSetWithdrawAddress;
    }

    public override async Task<AminoMsg> ToAmino()
    {
        var aminoMsg = new AminoMsg("cosmos-sdk/MsgModifyWithdrawAddress");
        // order of properties must be sorted for amino signing!!
        aminoMsg.Value = new
        {
            delegator_address = DelegatorAddress,
            withdraw_address = WithdrawAddress,
        };

        return aminoMsg;
    }
}

/// <summary>
/// MsgWithdrawDelegatorReward represents delegation withdrawal to a delegator from a single validator.
/// </summary>
public class MsgWithdrawDelegatorReward : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgWithdrawDelegatorReward;

    public string DelegatorAddress { get; set; }

    public string ValidatorAddress { get; set; }


    public MsgWithdrawDelegatorReward() { }

    public MsgWithdrawDelegatorReward(string delegatorAddress, string validatorAddress)
    {
        DelegatorAddress = delegatorAddress;
        ValidatorAddress = validatorAddress;
    }


    public override async Task<IMessage> ToProto()
    {
        var msgWithdrawDelegatorReward = new Cosmos.Distribution.V1Beta1.MsgWithdrawDelegatorReward()
        {
            DelegatorAddress = DelegatorAddress,
            ValidatorAddress = ValidatorAddress,
        };

        return msgWithdrawDelegatorReward;
    }

    public override async Task<AminoMsg> ToAmino()
    {
        var aminoMsg = new AminoMsg("cosmos-sdk/MsgWithdrawDelegationReward");
        // order of properties must be sorted for amino signing!!
        aminoMsg.Value = new
        {
            delegator_address = DelegatorAddress,
            validator_address = ValidatorAddress,
        };

        return aminoMsg;
    }
}

/// <summary>
/// MsgWithdrawValidatorCommission withdraws the full commission to the validator address.
/// </summary>
public class MsgWithdrawValidatorCommission : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgWithdrawValidatorCommission;

    public string ValidatorAddress { get; set; }


    public MsgWithdrawValidatorCommission() { }

    public MsgWithdrawValidatorCommission(string validatorAddress)
    {
        ValidatorAddress = validatorAddress;
    }

    public override async Task<IMessage> ToProto()
    {
        var msgWithdrawValidatorCommission = new Cosmos.Distribution.V1Beta1.MsgWithdrawValidatorCommission()
        {
            ValidatorAddress = ValidatorAddress,
        };

        return msgWithdrawValidatorCommission;
    }

    public override async Task<AminoMsg> ToAmino()
    {
        var aminoMsg = new AminoMsg("cosmos-sdk/MsgWithdrawValidatorCommission");
        // order of properties must be sorted for amino signing!!
        aminoMsg.Value = new
        {
            validator_address = ValidatorAddress,
        };

        return aminoMsg;
    }
}

/// <summary>
/// MsgFundCommunityPool allows an account to directly fund the community pool.
/// </summary>
public class MsgFundCommunityPool : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgFundCommunityPool;

    public string Depositor { get; set; }

    public Coin[] Amount { get; set; }


    public MsgFundCommunityPool() { }

    public MsgFundCommunityPool(string depositor, Coin[] amount)
    {
        Depositor = depositor;
        Amount = amount;
    }

    public override async Task<IMessage> ToProto()
    {
        var msgFundCommunityPool = new Cosmos.Distribution.V1Beta1.MsgFundCommunityPool()
        {
            Depositor = Depositor,

        };
        if (Amount != null)
        {
            msgFundCommunityPool.Amount.Add(Amount.Select(c => new Cosmos.Base.V1Beta1.Coin() { Amount = c.Amount, Denom = c.Denom }).ToArray());
        }        

        return msgFundCommunityPool;
    }

    public override async Task<AminoMsg> ToAmino()
    {
        var aminoMsg = new AminoMsg("cosmos-sdk/MsgFundCommunityPool");
        // order of properties must be sorted for amino signing!!
        aminoMsg.Value = new
        {
            depositor = Depositor,
            amount = Amount
        };

        return aminoMsg;
    }
}