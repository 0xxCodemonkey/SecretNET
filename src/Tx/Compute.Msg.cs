using Secret.Compute.V1Beta1;

namespace SecretNET.Tx;

public abstract class MsgExecuteContractBase : MsgBase
{
    internal byte[] _msgEncrypted = null;
    internal bool _warnCodeHash = false;

    public string Sender { get; set; }

    /// <summary>
    /// The SHA256 hash value of the contract's WASM bytecode, represented as case-insensitive 64
    /// character hex string.
    /// This is used to make sure only the contract that's being invoked can decrypt the query data.
    /// 
    /// codeHash is an optional parameter but using it will result in way faster execution time.
    /// 
    /// Valid examples:
    /// - "af74387e276be8874f07bec3a87023ee49b0e7ebe08178c49d0a49c3c98ed60e"
    /// - "0xaf74387e276be8874f07bec3a87023ee49b0e7ebe08178c49d0a49c3c98ed60e"
    /// - "AF74387E276BE8874F07BEC3A87023EE49B0E7EBE08178C49D0A49C3C98ED60E"
    /// - "0xAF74387E276BE8874F07BEC3A87023EE49B0E7EBE08178C49D0A49C3C98ED60E"
    /// 
    /// </summary>
    public string CodeHash { get; set; }

    /// <summary>
    /// The input message
    /// </summary>
    public object Msg { get; set; }

    internal async Task<byte[]> GetEncryptedMsg(SecretEncryptionUtils utils)
    {
        if (_msgEncrypted == null)
        {
            // The encryption uses a random nonce
            // toProto() & toAmino() are called multiple times during signing
            // so to keep the msg consistant across calls we encrypt the msg only once
            string msgString = (Msg is string) ? (string)Msg : JsonConvert.SerializeObject(Msg);
            _msgEncrypted = await utils.Encrypt(CodeHash, msgString);
        }
        return _msgEncrypted;
    }

    public string GetMissingCodeHashWarning()
    {
        return $"{DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture)} WARNING: '{this.GetType().Name}' was used without the \"codeHash\" parameter. This is discouraged and will result in much slower execution times for your app.";
    }

    public void WarnIfCodeHashMissing()
    {
        if (String.IsNullOrEmpty(CodeHash))
        {
            _warnCodeHash = true;
            Debug.WriteLine(GetMissingCodeHashWarning());
        }
    }
}

public class MsgExecuteContract<T> : MsgExecuteContract where T : class
{
    public new T Msg { get; set; }

    public MsgExecuteContract(string contractAddress, T msg, string codeHash = null, string sender = null, Coin[] sentFunds = null)
        :base(contractAddress, msg, codeHash, sender, sentFunds)
    {
    }
}

public class MsgExecuteContract : MsgExecuteContractBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgExecuteContract;

    /// <summary>
    /// The contract's address
    /// </summary>
    public string ContractAddress { get; set; }

    /// <summary>
    /// Funds to send to the contract
    /// </summary>
    public Coin[] SentFunds { get; set; }

    //TODO: Validate parameter => SecretAddress, etc. => enhanced security
    public MsgExecuteContract(string contractAddress, object msg, string codeHash = null, string sender = null, Coin[] sentFunds = null)
    {
        Sender = sender;
        ContractAddress = contractAddress;
        Msg = msg;
        SentFunds = sentFunds;

        if (!String.IsNullOrEmpty(codeHash))
        {
            CodeHash = codeHash.Replace("0x", "").ToLower();
        }

        WarnIfCodeHashMissing();
    }

    public override async Task<IMessage> ToProto(SecretEncryptionUtils utils)
    {
        WarnIfCodeHashMissing();

        var msgEncrypted = await GetEncryptedMsg(utils);

        var msgExecuteContract = new Secret.Compute.V1Beta1.MsgExecuteContract()
        {
            Sender = SecretNetworkClient.AddressToBytes(Sender).GetByteStringFromBase64(),
            Contract = SecretNetworkClient.AddressToBytes(ContractAddress).GetByteStringFromBase64(),
            CallbackCodeHash = CodeHash,
            Msg = ByteString.CopyFrom(msgEncrypted)
        };
        if (SentFunds != null)
{
            msgExecuteContract.SentFunds.Add(SentFunds.Select(c => new Cosmos.Base.V1Beta1.Coin() { Amount = c.Amount, Denom = c.Denom }).ToArray());
        }

        return msgExecuteContract;
    }

    public override async Task<AminoMsg> ToAmino(SecretEncryptionUtils utils)
    {
        WarnIfCodeHashMissing();

        var msgEncrypted = await GetEncryptedMsg(utils);

        var aminoMsg = new AminoMsg("wasm/MsgExecuteContract");
        // order of properties must be sorted for amino signing!!
        aminoMsg.Value = new
        {
            contract = ContractAddress,
            msg = Convert.ToBase64String(msgEncrypted),
            sender = Sender,
            sent_funds = SentFunds
        };
        return aminoMsg;
    }    
}

public class MsgInstantiateContract : MsgExecuteContractBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgInstantiateContract;

    public ulong CodeId { get; set; }

    public string Label { get; set; }

    public Coin[] InitFunds { get; set; }

    public MsgInstantiateContract() { }

    public MsgInstantiateContract(ulong codeId, string label, object initMsg, string codeHash = null, string sender = null, Coin[] initFunds = null)
    {
        CodeId = codeId;
        Label = label;
        Msg = initMsg;
        InitFunds = initFunds;
        Sender = sender;

        if (!String.IsNullOrEmpty(codeHash))
        {
            CodeHash = codeHash.Replace("0x", "").ToLower();
        }

        WarnIfCodeHashMissing();
    }

    public override async Task<IMessage> ToProto(SecretEncryptionUtils utils)
    {
        WarnIfCodeHashMissing();

        var msgEncrypted = await GetEncryptedMsg(utils);

        var msgInstantiateContract = new Secret.Compute.V1Beta1.MsgInstantiateContract()
        {
            Sender = SecretNetworkClient.AddressToBytes(Sender).GetByteStringFromBase64(),
            CodeId = CodeId,
            Label = Label,
            CallbackCodeHash = CodeHash,
            InitMsg = ByteString.CopyFrom(msgEncrypted)
        };
        if (InitFunds != null)
        {
            msgInstantiateContract.InitFunds.Add(InitFunds.Select(c => new Cosmos.Base.V1Beta1.Coin() { Amount = c.Amount, Denom = c.Denom }).ToArray());
        }        

        return msgInstantiateContract;
    }

    public override async Task<AminoMsg> ToAmino(SecretEncryptionUtils utils)
    {
        WarnIfCodeHashMissing();

        var msgEncrypted = await GetEncryptedMsg(utils);

        var aminoMsg = new AminoMsg("wasm/MsgInstantiateContract");
        // order of properties must be sorted for amino signing!!
        aminoMsg.Value = new
        {
            code_id = CodeId,
            init_funds = InitFunds,
            init_msg = Convert.ToBase64String(msgEncrypted),
            label = Label,
            sender = Sender,
        };

        return aminoMsg;
    }
}

public class MsgStoreCode : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgStoreCode;

    public string Sender { get; set; }

    /// <summary>
    /// Gets or sets the wasm byte code.
    /// </summary>
    /// <value>The wasm byte code.</value>
    public byte[] WasmByteCode { get; set; }

    /// <summary>
    /// Source is a valid absolute HTTPS URI to the contract's source code, optional
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Builder is a valid docker image name with tag, optional
    /// </summary>
    public string Builder { get; set; }


    public MsgStoreCode() {}

    /// <summary>
    /// Initializes a new instance of the <see cref="MsgStoreCode"/> class.
    /// </summary>
    /// <param name="wasmByteCode">The wasm byte code.</param>
    /// <param name="source">Source is a valid absolute HTTPS URI to the contract's source code, optional.</param>
    /// <param name="builder">Builder is a valid docker image name with tag, optional.</param>
    /// <param name="sender">The sender.</param>
    public MsgStoreCode(byte[] wasmByteCode, string source = null, string builder = null, string sender = null)
    {
        Sender = sender;
        WasmByteCode = wasmByteCode;
        Source = source;
        Builder = builder;
    }

    public override async Task<IMessage> ToProto(SecretEncryptionUtils utils)
    {
        var msgStoreCode = new Secret.Compute.V1Beta1.MsgStoreCode()
        {
            Builder = Builder,
            Sender = SecretNetworkClient.AddressToBytes(Sender).GetByteStringFromBase64(),
            WasmByteCode = ByteString.CopyFrom(WasmByteCode),
            Source = Source,
        };

        return msgStoreCode;
    }

    public override async Task<AminoMsg> ToAmino(SecretEncryptionUtils utils)
    {
        var aminoMsg = new AminoMsg("wasm/MsgStoreCode");
        // order of properties must be sorted for amino signing!!
        aminoMsg.Value = new
        {
            builder = Builder,          
            sender = Sender,
            source = Source,
            wasm_byte_code = Convert.ToBase64String(WasmByteCode)
        };

        return aminoMsg;
    }
}


// Typed SecretTx responses

public class InstantiateContractSecretTx : SecretTx
{
    public string ContractAddress { get; set; }

    public InstantiateContractSecretTx(SecretTx secretTx) : base(secretTx)
    {
        ContractAddress = TryFindEventValue("contract_address");
    }
}

public class StoreCodeSecretTx : SecretTx
{
    public ulong CodeId { get; set; }

    public StoreCodeSecretTx(SecretTx secretTx) : base(secretTx)
    {
        TryGetCodeId();
    }

    private void TryGetCodeId()
    {
        var codeIdString = TryFindEventValue("code_id");
        if (!string.IsNullOrEmpty(codeIdString) && ulong.TryParse(codeIdString, out var code_id))
        {
            CodeId = code_id;
        }
    }
}