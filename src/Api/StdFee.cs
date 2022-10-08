using SecretNET.Tx;

namespace SecretNET.Api;

public class StdFee
{
    public static Cosmos.Base.V1Beta1.Coin[] DefaultAmount { get; set; } = new Cosmos.Base.V1Beta1.Coin[] { new Cosmos.Base.V1Beta1.Coin() { Amount = "10000", Denom = "uscrt" } };
    public static string DefaultGas { get; set; } = "25000";

    [JsonProperty("amount")]
    public Cosmos.Base.V1Beta1.Coin[] Amount { get; set; }

    [JsonProperty("gas")]
    public string Gas { get; set; }

    [JsonProperty("granter")]
    public string FeeGranter { get; set; }

    public StdFee(Cosmos.Base.V1Beta1.Coin coin, string gas, string feeGranter = null) : this(new[] { coin }, gas, feeGranter)
    {
    }

    public StdFee(Cosmos.Base.V1Beta1.Coin[] coins, string gas, string feeGranter = null)
    {
        Amount = coins;
        Gas = gas;
        FeeGranter = feeGranter;
    }

    public static StdFee GetDefault(){
        return new StdFee(DefaultAmount, DefaultGas);
    }

    public static StdFee FromTxOptions(TxOptions txOptions)
    {
        var coin = new Cosmos.Base.V1Beta1.Coin() { 
            Amount = Math.Ceiling(txOptions.GasLimit * txOptions.GasPriceInFeeDenom).ToString(), 
            Denom = txOptions.FeeDenom 
        };

        return new StdFee(new Cosmos.Base.V1Beta1.Coin[] { coin }, txOptions.GasLimit.ToString(), txOptions.FeeGranter);
    }

    public StdFeeAmino ConvertToStdFeeAmino()
    {
        var coins = Amount.Select(c => new Tx.Coin() { Amount = c.Amount, Denom = c.Denom }).ToArray();
        var result = new StdFeeAmino(coins, Gas);
        return result;
    }
}

public class StdFeeAmino
{
    public static Tx.Coin[] DefaultAmount { get; set; } = new Tx.Coin[] { new Tx.Coin() { Amount = "10000", Denom = "uscrt" } };
    public static string DefaultGas { get; set; } = "25000";

    [JsonProperty("amount")]
    public Tx.Coin[] Amount { get; set; }

    [JsonProperty("gas")]
    public string Gas { get; set; }

    //[JsonProperty("granter")]
    [JsonIgnore]
    public string FeeGranter { get; set; }

    public StdFeeAmino(Tx.Coin coin, string gas, string feeGranter = null) : this(new[] { coin }, gas, feeGranter)
    {
    }

    public StdFeeAmino(Tx.Coin[] coins, string gas, string feeGranter = null)
    {
        Amount = coins;
        Gas = gas;
        FeeGranter = feeGranter;
    }

    public StdFee ConvertToStdFee()
    {
        var coins = Amount.Select(c => new Cosmos.Base.V1Beta1.Coin() { Amount = c.Amount, Denom = c.Denom }).ToArray();
        var result = new StdFee(coins, Gas, FeeGranter);
        return result;
    }

    public static StdFeeAmino GetDefault()
    {
        return new StdFeeAmino(DefaultAmount, DefaultGas);
    }

    public static StdFeeAmino FromTxOptions(TxOptions txOptions)
    {
        var coin = new Tx.Coin()
        {
            Amount = Math.Ceiling(txOptions.GasLimit * txOptions.GasPriceInFeeDenom).ToString(),
            Denom = txOptions.FeeDenom
        };

        return new StdFeeAmino(new Tx.Coin[] { coin }, txOptions.GasLimit.ToString(), txOptions.FeeGranter);
    }

}
