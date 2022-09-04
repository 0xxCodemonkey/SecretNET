namespace SecretNET.Tx;

/// <summary>
/// MsgCreateValidator defines an SDK message for creating a new validator.
/// </summary>
public class MsgCreateValidator : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgCreateValidator;

    /// <summary>
    /// Description defines a validator description.
    /// </summary>
    public Cosmos.Staking.V1Beta1.Description Description { get; set; }

    /// <summary>
    /// CommissionRates defines the initial commission rates to be used for creating a validator.
    /// </summary>
    public Cosmos.Staking.V1Beta1.CommissionRates Commission { get; set; }

    /// <summary>
    /// minSelfDelegation is the minimum uscrt amount that the self delegator must delegate to its validator.
    /// </summary>
    public string MinSelfDelegation { get; set; }

    /// <summary>
    /// selfDelegatorAddress is the self-delegator, which is the owner of the validator
    /// </summary>
    public string DelegatorAddress { get; set; }

    
    public string ValidatorAddress { get; set; }

    /// <summary>
    /// representation of the validator's ed25519 pubkey (32 bytes).
    /// </summary>
    public byte[] Pubkey { get; set; }

    /// <summary>
    /// the tokens to be transferred
    /// </summary>
    public Coin InitialDelegation { get; set; }

    public MsgCreateValidator(){}

    public MsgCreateValidator(
        Cosmos.Staking.V1Beta1.Description description, 
        Cosmos.Staking.V1Beta1.CommissionRates commission,
        string minSelfDelegation, string delegatorAddress, string validatorAddress, byte[] pubkey,
        Coin initialDelegation)
    {
        Description = description;
        Commission = commission;
        MinSelfDelegation = minSelfDelegation;
        DelegatorAddress = delegatorAddress;
        ValidatorAddress = validatorAddress;
        Pubkey = pubkey;
        InitialDelegation = initialDelegation;
    }

    public override async Task<IMessage> ToProto(SecretEncryptionUtils utils)
    {
        var msgCreateValidator = new Cosmos.Staking.V1Beta1.MsgCreateValidator()
        {
            Description = Description,
            Commission = Commission,
            MinSelfDelegation = MinSelfDelegation,
            DelegatorAddress = DelegatorAddress,
            ValidatorAddress = ValidatorAddress,
        };
        if (Pubkey != null && Pubkey.Length == 32)
        {
            msgCreateValidator.Pubkey = Any.Pack(new Cosmos.Crypto.Ed25519.PubKey()
            {
                Key = Pubkey.GetByteStringFromBase64(),
            },"");
        }
        if (InitialDelegation != null)
        {
            msgCreateValidator.Value = new Cosmos.Base.V1Beta1.Coin() { Amount = InitialDelegation.Amount, Denom = InitialDelegation.Denom };
        }

        return msgCreateValidator;
    }
    public override async Task<AminoMsg> ToAmino(SecretEncryptionUtils utils)
    {
        throw new NotImplementedException("MsgCreateValidator ToAmino is not implemented.");
    }
}

/// <summary>
/// MsgEditValidator defines an SDK message for editing an existing validator.
/// </summary>
public class MsgEditValidator : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgEditValidator;

    public string ValidatorAddress { get; set; }

    /// <summary>
    /// if description is provided it updates all values
    /// </summary>
    public Cosmos.Staking.V1Beta1.Description? Description { get; set; }

    /// <summary>
    /// CommissionRate defines the initial commission rates to be used for creating a validator.
    /// </summary>
    public ulong? CommissionRate { get; set; }

    /// <summary>
    /// minSelfDelegation is the minimum uscrt amount that the self delegator must delegate to its validator.
    /// </summary>
    public string MinSelfDelegation { get; set; }


    public MsgEditValidator() { }

    public MsgEditValidator(string validatorAddress, string minSelfDelegation,
        Cosmos.Staking.V1Beta1.Description? description = null,
        ulong? commissionRate = null)
    {
        ValidatorAddress = validatorAddress;
        MinSelfDelegation = minSelfDelegation;
        Description = description;
        CommissionRate = commissionRate;
    }

    public override async Task<IMessage> ToProto(SecretEncryptionUtils utils)
    {
        var msgEditValidator = new Cosmos.Staking.V1Beta1.MsgEditValidator()
        {
            Description = Description,
            CommissionRate = CommissionRate.GetValueOrDefault().ToString(),
            MinSelfDelegation = MinSelfDelegation,
            ValidatorAddress = ValidatorAddress,
        };

        return msgEditValidator;
    }
    public override async Task<AminoMsg> ToAmino(SecretEncryptionUtils utils)
    {
        throw new NotImplementedException("MsgEditValidator ToAmino is not implemented.");
    }
}

/// <summary>
/// MsgDelegate defines an SDK message for performing a delegation of coins from a delegator to a validator.
/// </summary>
public class MsgDelegate : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgDelegate;

    public string DelegatorAddress { get; set; }

    public string ValidatorAddress { get; set; }

    public Coin Amount { get; set; }

    public MsgDelegate() { }

    public MsgDelegate(string delegatorAddress, string validatorAddress, Coin amount)
    {
        DelegatorAddress = delegatorAddress;
        ValidatorAddress = validatorAddress;
        Amount = amount;
    }

    public override async Task<IMessage> ToProto(SecretEncryptionUtils utils)
    {
        var msgDelegate = new Cosmos.Staking.V1Beta1.MsgDelegate()
        {
            DelegatorAddress = DelegatorAddress,
            ValidatorAddress = ValidatorAddress,
        };
        if (Amount != null)
        {
            msgDelegate.Amount = new Cosmos.Base.V1Beta1.Coin() { Amount = Amount.Amount, Denom = Amount.Denom };
        }

        return msgDelegate;
    }
    public override async Task<AminoMsg> ToAmino(SecretEncryptionUtils utils)
    {
        throw new NotImplementedException("MsgDelegate ToAmino is not implemented.");
    }
}

/// <summary>
/// MsgBeginRedelegate defines an SDK message for performing a redelegation of coins from a delegator and source validator to a destination validator.
/// </summary>
public class MsgBeginRedelegate : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgBeginRedelegate;

    public string DelegatorAddress { get; set; }

    public string ValidatorSrcAddress { get; set; }

    public string ValidatorDstAddress { get; set; }

    public Coin Amount { get; set; }

    public MsgBeginRedelegate() { }

    public MsgBeginRedelegate(string delegatorAddress, string validatorSrcAddress, string validatorDstAddress, Coin amount)
    {
        DelegatorAddress = delegatorAddress;
        ValidatorSrcAddress = validatorSrcAddress;
        ValidatorDstAddress = validatorDstAddress;
        Amount = amount;
    }

    public override async Task<IMessage> ToProto(SecretEncryptionUtils utils)
    {
        var msgBeginRedelegate = new Cosmos.Staking.V1Beta1.MsgBeginRedelegate()
        {
            DelegatorAddress = DelegatorAddress,
            ValidatorSrcAddress = ValidatorSrcAddress,
            ValidatorDstAddress = ValidatorDstAddress,
        };
        if (Amount != null)
        {
            msgBeginRedelegate.Amount = new Cosmos.Base.V1Beta1.Coin() { Amount = Amount.Amount, Denom = Amount.Denom };
        }

        return msgBeginRedelegate;
    }
    public override async Task<AminoMsg> ToAmino(SecretEncryptionUtils utils)
    {
        throw new NotImplementedException("MsgBeginRedelegate ToAmino is not implemented.");
    }
}

/// <summary>
/// MsgUndelegate defines an SDK message for performing an undelegation from a delegate and a validator
/// </summary>
public class MsgUndelegate : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgUndelegate;

    public string DelegatorAddress { get; set; }

    public string ValidatorAddress { get; set; }

    public Coin Amount { get; set; }

    public MsgUndelegate() { }

    public MsgUndelegate(string delegatorAddress, string validatorAddress, Coin amount)
    {
        DelegatorAddress = delegatorAddress;
        ValidatorAddress = validatorAddress;
        Amount = amount;
    }

    public override async Task<IMessage> ToProto(SecretEncryptionUtils utils)
    {
        var msgUndelegate = new Cosmos.Staking.V1Beta1.MsgUndelegate()
        {
            DelegatorAddress = DelegatorAddress,
            ValidatorAddress = ValidatorAddress,
        };
        if (Amount != null)
        {
            msgUndelegate.Amount = new Cosmos.Base.V1Beta1.Coin() { Amount = Amount.Amount, Denom = Amount.Denom };
        }

        return msgUndelegate;
    }
    public override async Task<AminoMsg> ToAmino(SecretEncryptionUtils utils)
    {
        throw new NotImplementedException("MsgUndelegate ToAmino is not implemented.");
    }
}



