namespace SecretNET.Tx;

/// <summary>
/// MsgSend represents a message to send coins from one account to another.
/// </summary>
public class MsgSend : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgSend;

    public string FromAddress { get; set; }

    public string ToAddress { get; set; }

    public Coin[] Amount { get; set; }


    public MsgSend() { }

    public MsgSend(string fromAddress, string toAddress, Coin[] amount)
    {
        FromAddress = fromAddress;
        ToAddress = toAddress;
        Amount = amount;
    }

    public override async Task<IMessage> ToProto(SecretEncryptionUtils utils)
    {
        var msgSend = new Cosmos.Bank.V1Beta1.MsgSend()
        {
            FromAddress = FromAddress,
            ToAddress = ToAddress,
        };
        msgSend.Amount.Add(Amount.Select(c => new Cosmos.Base.V1Beta1.Coin() { Amount = c.Amount, Denom = c.Denom }).ToArray());

        return msgSend;
    }

    public override async Task<AminoMsg> ToAmino(SecretEncryptionUtils utils)
    {
        var aminoMsg = new AminoMsg("cosmos-sdk/MsgSend");
        // order of properties must be sorted for amino signing!!
        aminoMsg.Value = new
        {
            amount = Amount,
            from_address = FromAddress,
            to_address = ToAddress,
        };

        return aminoMsg;
    }
}

/// <summary>
/// Input / Output models transaction input for MsgMultiSend.
/// </summary>
public class Input
{
    [JsonProperty("address")]
    public string Address { get; set; }

    [JsonProperty("coins")]
    public Coin[] Coins { get; set; }

    public Input(string address, Coin[] coins)
    {
        Address = address;
        Coins = coins;
    }
}

/// <summary>
/// Output models transaction outputs for MsgMultiSend.
/// </summary>
public class Output
{
    [JsonProperty("address")]
    public string Address { get; set; }

    [JsonProperty("coins")]
    public Coin[] Coins { get; set; }

    public Output(string address, Coin[] coins)
    {
        Address = address;
        Coins = coins;
    }
}


/// <summary>
/// MsgMultiSend represents an arbitrary multi-in, multi-out send message. */
/// </summary>
public class MsgMultiSend : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgMultiSend;

    public Input[] Inputs { get; set; }

    public Output[] Outputs { get; set; }


    public MsgMultiSend() { }

    public MsgMultiSend(Input[] inputs, Output[] outputs)
    {
        Inputs = inputs;
        Outputs = outputs;
    }

    public override async Task<IMessage> ToProto(SecretEncryptionUtils utils)
    {
        if (Inputs != null || Outputs != null)
        {
            var msgMultiSend = new Cosmos.Bank.V1Beta1.MsgMultiSend();

            if (Inputs != null)
            {
                var inputList = new List<Cosmos.Bank.V1Beta1.Input>();
                foreach (var i in Inputs)
                {
                    var input = new Cosmos.Bank.V1Beta1.Input()
                    {
                        Address = i.Address
                    };
                    input.Coins.Add(i.Coins.Select(c => new Cosmos.Base.V1Beta1.Coin() { Amount = c.Amount, Denom = c.Denom }).ToArray());
                }
                msgMultiSend.Inputs.Add(inputList.ToArray());
            }

            if (Outputs != null)
            {
                var outputList = new List<Cosmos.Bank.V1Beta1.Output>();
                foreach (var o in Outputs)
                {
                    var input = new Cosmos.Bank.V1Beta1.Input()
                    {
                        Address = o.Address
                    };
                    input.Coins.Add(o.Coins.Select(c => new Cosmos.Base.V1Beta1.Coin() { Amount = c.Amount, Denom = c.Denom }).ToArray());
                }
                msgMultiSend.Outputs.Add(outputList.ToArray());
            }

            return msgMultiSend;
        }
        return null;
    }

    public override async Task<AminoMsg> ToAmino(SecretEncryptionUtils utils)
    {
        var aminoMsg = new AminoMsg("cosmos-sdk/MsgMultiSend");
        // order of properties must be sorted for amino signing!!
        aminoMsg.Value = new
        {
            inputs = Inputs,
            outputs = Outputs
        };

        return aminoMsg;
    }
}