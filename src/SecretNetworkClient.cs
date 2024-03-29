﻿using Cosmos.Tx.Signing.V1Beta1;
using SecretNET.Query;
using SecretNET.Tx;
using SecretNET.Crypto;
using SecretNET.Api;

namespace SecretNET;

/// <summary>
/// Class SecretNetworkClient.
/// Implements the <see cref="SecretNET.ISecretNetworkClient" />
/// </summary>
/// <seealso cref="SecretNET.ISecretNetworkClient" />
public class SecretNetworkClient : ISecretNetworkClient
{
    private CreateClientOptions _createClientOptions;

    private GrpcChannel _grpcChannel;
    private CallInvoker? _rpcMessageInterceptor;
    private IWallet? _wallet;
    private SecretEncryptionUtils _encryptionUtils;

    private Queries? _queries;
    private TxClient? _txClient;
    private AccessControl.PermitSigner _permit;

    #region Properties

    /// <summary>
    /// A gRPC-web url, by default on port 9091
    /// </summary>
    /// <value>The GRPC web URL.</value>
    public string GrpcWebUrl { get { return _createClientOptions.GrpcWebUrl; } }

    /// <summary>
    /// The chain-id is used in encryption code and when signing txs.
    /// </summary>
    /// <value>The chain identifier.</value>
    public string ChainId { get { return _createClientOptions.ChainId; } }

    /// <summary>
    /// A wallet for signing transactions and permits. When `wallet` is supplied, `walletAddress` and `chainId` must be supplied too.
    /// </summary>
    /// <value>The wallet.</value>
    public IWallet Wallet { 
        get { return _wallet; } 
        set {
            _wallet = value;
        } 
    }

    /// <summary>
    /// WalletAddress is the specific account address in the wallet that is permitted to sign transactions and permits.
    /// If no wallet is attached the `CreateClientOptions.WalletAddress` is returned.
    /// </summary>
    /// <value>The wallet address.</value>
    public string WalletAddress { get { return !string.IsNullOrWhiteSpace(_wallet?.Address) ? _wallet?.Address : _createClientOptions.WalletAddress; } }

    /// <summary>
    /// Passing `encryptionSeed` will allow tx decryption at a later time. Ignored if `encryptionUtils` is supplied.
    /// </summary>
    /// <value>The encryption seed.</value>
    public byte[] EncryptionSeed { get; private set; }

    ///// <summary>
    ///// Secret Network encription utils
    ///// </summary>
    ///// <value>The encryption utils.</value>
    //public SecretEncryptionUtils EncryptionUtils { get; private set; }

    /// <summary>
    /// Transaction approval callback for a user approval of an transaction.
    /// </summary>
    /// <value>The transaction approval callback.</value>
    public Func<TransactionApprovalData, Task<UserApprovalDecision>> TransactionApprovalCallback { get; set; }

    /// <summary>
    /// WARNING: On mainnet it's recommended to not simulate every transaction as this can burden your node provider.
    /// Instead, use this while testing to determine the gas limit for each of your app's transactions (use TxOptions.GasLimit), then in production use hard-coded.
    /// </summary>
    /// <value><c>true</c> if [always simulate transactions]; otherwise, <c>false</c>.</value>
    public bool AlwaysSimulateTransactions { get; set; }

    /// <summary>
    /// Gas estimation is known to be a bit off, so you might need to adjust it a bit before broadcasting (default is 1.1 / 10%).
    /// </summary>
    /// <value>The gas estimation mltiplier.</value>
    public float GasEstimationMultiplier { get; set; }

    #endregion

    // ctor

    /// <summary>
    /// Initializes a new instance of the <see cref="SecretNetworkClient" /> class.
    /// </summary>
    /// <param name="grpcWebUrl">The GRPC web URL.</param>
    /// <param name="chainId">The chain identifier.</param>
    /// <param name="wallet">The wallet.</param>
    public SecretNetworkClient(string grpcWebUrl, string chainId, Wallet wallet) :
        this(new CreateClientOptions(grpcWebUrl, chainId, wallet))
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SecretNetworkClient" /> class.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="grpcChannelOptions">The GRPC channel options.</param>
    /// <param name="grpcMessageInterceptor">The GRPC message interceptor.</param>
    public SecretNetworkClient(CreateClientOptions options, GrpcChannelOptions grpcChannelOptions = null, Interceptor grpcMessageInterceptor = null)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (string.IsNullOrWhiteSpace(options.ChainId) || string.IsNullOrWhiteSpace(options.GrpcWebUrl))
        {
            throw new ArgumentOutOfRangeException("ChainId and GrpcWebUrl must have a value!");
        }

        _createClientOptions = options;
        AlwaysSimulateTransactions = _createClientOptions.AlwaysSimulateTransactions;
        GasEstimationMultiplier = _createClientOptions.GasEstimationMultiplier;
        TransactionApprovalCallback = _createClientOptions.TransactionApprovalCallback;

        if (grpcChannelOptions == null)
        {
            // Default Options
            grpcChannelOptions = new GrpcChannelOptions();
            grpcChannelOptions.HttpHandler = new GrpcWebHandler(new HttpClientHandler());
        }

        this._grpcChannel = GrpcChannel.ForAddress(options.GrpcWebUrl, grpcChannelOptions);
        if (grpcMessageInterceptor != null)
        {
            this._rpcMessageInterceptor = this._grpcChannel.Intercept(grpcMessageInterceptor);
        }
        
        if (_createClientOptions.EncryptionUtils != null)
        {
            _encryptionUtils = _createClientOptions.EncryptionUtils;
        }
        else if ((_createClientOptions.EncryptionSeed != null && _createClientOptions.EncryptionSeed?.Length > 0))
        {
            EncryptionSeed = _createClientOptions.EncryptionSeed;
            _encryptionUtils = new SecretEncryptionUtils(ChainId, Query.Registration, _createClientOptions.EncryptionSeed);
        }

        Wallet = _createClientOptions.Wallet;
    }

    /// <summary>
    /// Access to all query types.
    /// </summary>
    /// <value>The query.</value>
    public Queries Query
    {
        get
        {
            if (_queries == null)
            {
                _queries = new Queries(this, _grpcChannel, _rpcMessageInterceptor);
            }
            return _queries;
        }
    }

    /// <summary>
    /// A signer client (= with wallet) can broadcast transactions.
    /// Tx gives access to all transaction types.
    /// </summary>
    /// <value>The tx.</value>
    public TxClient Tx
    {
        get
        {
            if (_txClient == null)
            {
                _txClient = new TxClient(this, Query, _grpcChannel, _rpcMessageInterceptor);
            }
            return _txClient;
        }
    }

    /// <summary>
    /// A (amino) signer. for signing permits (needs an attached wallet).
    /// </summary>
    /// <value>The permit.</value>
    /// <exception cref="System.ArgumentNullException">Permit needs an attached wallet! Please add a wallet to SecretNetworkClient via CreateClientOptions</exception>
    public AccessControl.PermitSigner Permit
    {
        get
        {
            if (_permit == null)
            {
                if (Wallet != null)
                {
                    _permit = new AccessControl.PermitSigner(Wallet);
                }
                else
                {
                    throw new ArgumentNullException("Permit needs an attached wallet! Please add a wallet to SecretNetworkClient via CreateClientOptions");
                }                
            }
            return _permit;
        }
    }

    /// <summary>
    /// Prepares the and signs the specified messages.
    /// </summary>
    /// <param name="messages">The messages.</param>
    /// <param name="txOptions">The tx options.</param>
    /// <returns>System.ValueTuple&lt;System.Byte[], ComputeMsgToNonce&gt;.</returns>
    public async Task<byte[]> PrepareAndSign(MsgBase[] messages, TxOptions txOptions = null)
    {
        txOptions = txOptions ?? new TxOptions();

        var signResult = await Sign(messages, StdFee.FromTxOptions(txOptions), txOptions.Memo, txOptions.ExplicitSignerData);
        var txBytes = signResult.Encode();

        return txBytes;
    }

    /// <summary>
    /// Signs the specified messages.
    /// </summary>
    /// <param name="messages">The messages.</param>
    /// <param name="fee">The fee.</param>
    /// <param name="memo">The memo.</param>
    /// <param name="explicitSignerData">The explicit signer data.</param>
    /// <returns>System.ValueTuple&lt;TxRaw, ComputeMsgToNonce&gt;.</returns>
    public async Task<TxRaw> Sign(MsgBase[] messages, StdFee fee, string? memo = null, SignerData? explicitSignerData = null)
    {
        if (Wallet == null)
        {
            throw new Exception("Failed to retrieve account from signer");
        }

        SignerData signerData = explicitSignerData;
        if (signerData == null)
        {
            signerData = await GetSignerData(this.Wallet.Address);
        }

        // Since SecretNET currently not support Ledger, Amino signing is currently only used for permits
        return Wallet.WalletSignType == WalletSignType.DirectSigner
            ? await SignDirect(messages, fee, signerData, memo)
            : await SignAmino(messages, fee, signerData, memo);
    }

    /// <summary>
    /// Gets the signer data for the given wallet address.
    /// </summary>
    /// <param name="walletAddress">The wallet address.</param>
    /// <returns>SignerData.</returns>
    public async Task<SignerData> GetSignerData(string walletAddress)
    {
        var response = await Query.Auth.Account(walletAddress);
        if (response == null)
        {
            throw new Exception($"Cannot find account '${walletAddress}', make sure it has a balance.");
        }

        if (!response.IsProtoType("cosmos.auth.v1beta1.BaseAccount"))
        {
            throw new Exception($"Cannot sign with account of type '{response.Descriptor.FullName}', can only sign with 'BaseAccount'.");
        }

        var baseAccount = (Cosmos.Auth.V1Beta1.BaseAccount)response;

        var signerData = new SignerData()
        {
            AccountNumber = baseAccount.AccountNumber,
            Sequence = baseAccount.Sequence,
            ChainID = this.ChainId
        };

        return signerData;
    }

    /// <summary>
    /// Signs the transaction (direct).
    /// </summary>
    /// <param name="messages">The messages.</param>
    /// <param name="fee">The fee.</param>
    /// <param name="signerData">The signer data.</param>
    /// <param name="memo">The memo.</param>
    /// <returns>System.ValueTuple&lt;TxRaw, ComputeMsgToNonce&gt;.</returns>
    /// <exception cref="Exception">Failed to retrieve account from signer</exception>
    public async Task<TxRaw> SignDirect(MsgBase[] messages, StdFee fee, SignerData signerData, string? memo = null)
    {
        if (Wallet == null)
        {
            throw new Exception("Failed to retrieve account from signer");
        }

        var encryptionNonces = new ComputeMsgToNonce();
        
        var txBody = new TxBody();
        for (int i = 0; i < messages.Length; i++)
        {
            var msg = messages[i];
            if (msg is MsgExecuteContractBase)
            {
                ((MsgExecuteContractBase)msg).EncryptionUtils = await GetEncryptionUtils();
            }
            
            await PopulateCodeHash(msg);
            var protoMsg = await msg.ToProto();
            encryptionNonces.TryAdd(i, ExtractNonce(protoMsg));

            var any = Any.Pack(protoMsg,"");

            txBody.Messages.Add(any);
        }

        var txBodyBytes = txBody.Encode();
        var pubKey = EncodePubkey(EncodeSecp256k1Pubkey(Wallet.PublicKey));
        var authInfoBytes = MakeAuthInfoBytes(pubKey, signerData.Sequence, fee.Amount, ulong.Parse(fee.Gas), SignMode.Direct, fee.FeeGranter);
        var signDoc = MakeSignDocProto(txBodyBytes, authInfoBytes, ChainId, signerData.AccountNumber);
        var walletSign = await Wallet.SignDirect(signDoc, Wallet.Address);

        var txRaw = new TxRaw();
        txRaw.BodyBytes = signDoc.BodyBytes;
        txRaw.AuthInfoBytes = signDoc.AuthInfoBytes;
        txRaw.Signatures.Add(ByteString.FromBase64(walletSign.Signature));

        return txRaw;
    }

    /// <summary>
    /// Signs the transaction (amino).
    /// </summary>
    /// <param name="messages">The messages.</param>
    /// <param name="fee">The fee.</param>
    /// <param name="signerData">The signer data.</param>
    /// <param name="memo">The memo.</param>
    /// <returns>System.ValueTuple&lt;TxRaw, ComputeMsgToNonce&gt;.</returns>
    /// <exception cref="Exception">Failed to retrieve account from signer</exception>
    public async Task<TxRaw> SignAmino(MsgBase[] messages, StdFee fee, SignerData signerData, string? memo = null)
    {
        if (Wallet == null)
        {
            throw new Exception("Failed to retrieve account from signer");
        }        

        List<AminoMsg> msgs = new List<AminoMsg>();
        foreach (var msg in messages)
        {
            if (msg is MsgExecuteContractBase)
            {
                ((MsgExecuteContractBase)msg).EncryptionUtils = await GetEncryptionUtils();
            }
            msgs.Add(await msg.ToAmino());
        }

        var signDoc = MakeSignDocAmino(msgs.ToArray(), fee, ChainId, signerData.AccountNumber, signerData.Sequence, memo);
        var walletSign = await Wallet.SignAmino(signDoc, Wallet.Address);

        var encryptionNonces = new ComputeMsgToNonce();

        var txBody = new TxBody();
        for (int i = 0; i < messages.Length; i++)
        {
            var msg = messages[i];
            if (msg is MsgExecuteContractBase)
            {
                ((MsgExecuteContractBase)msg).EncryptionUtils = await GetEncryptionUtils();
            }

            await PopulateCodeHash(msg);
            var protoMsg = await msg.ToProto();
            encryptionNonces.TryAdd(i, ExtractNonce(protoMsg));

            var any = Any.Pack(protoMsg, "");

            txBody.Messages.Add(any);
        }

        var txBodyBytes = txBody.Encode();
        var pubKey = EncodePubkey(EncodeSecp256k1Pubkey(Wallet.PublicKey));
        var authInfoBytes = MakeAuthInfoBytes(pubKey, signerData.Sequence, fee.Amount, ulong.Parse(fee.Gas), SignMode.Direct);

        var txRaw = new TxRaw();
        txRaw.BodyBytes = ByteString.FromBase64(Convert.ToBase64String(txBodyBytes));
        txRaw.AuthInfoBytes = ByteString.FromBase64(Convert.ToBase64String(authInfoBytes));
        txRaw.Signatures.Add(ByteString.FromBase64(walletSign.Signature));

        return txRaw;
    }

    /// <summary>
    /// Gets the EncryptionUtils and uses the stored tx encryption key / seed (from which the encryption key for encrypting the transactions is generated) from the wallet or generates a new one (Keplr style) and stores it.
    /// If the EncryptionUtils were passed in the CreateClientOptions, they will be used.
    /// </summary>
    /// <returns>SecretEncryptionUtils.</returns>
    public async Task<SecretEncryptionUtils> GetEncryptionUtils()
    {
        byte[] txEncryptionKey = null;
        if ((_createClientOptions.EncryptionSeed != null && _createClientOptions.EncryptionSeed.Length == 32) || _createClientOptions.EncryptionUtils != null)
        {
            return _encryptionUtils;
        }
        else if (Wallet != null)
        {
            txEncryptionKey = await Wallet.GetTxEncryptionSeed(ChainId);
            if (txEncryptionKey == null || txEncryptionKey.Length == 0)
            {
                txEncryptionKey = await GetTxEncryptionSeed(Wallet, ChainId);
            }
            EncryptionSeed = txEncryptionKey;
        }

        if (_encryptionUtils == null)
        {
            _encryptionUtils = new SecretEncryptionUtils(ChainId, Query.Registration, seed: txEncryptionKey); // if seed is NULL a new random tx encryption key / seed is generated
        }

        return _encryptionUtils;
    }

    // static

    /// <summary>
    /// Gets the stored tx encryption key / seed (from which the encryption key for encrypting the transactions is generated) or generates a new one (Keplr style) and stores it.
    /// </summary>
    /// <param name="wallet">The wallet.</param>
    /// <param name="chainId">The chain identifier.</param>
    /// <returns>System.Byte[].</returns>
    public static async Task<byte[]> GetTxEncryptionSeed(IWallet wallet, string chainId)
    {
        var txEncryptionKey = await wallet.GetTxEncryptionSeed(chainId);
        if (txEncryptionKey != null && txEncryptionKey.Length > 0)
        {
            return txEncryptionKey;
        }

        // Keplr style TxEncryptionKey
        var signMsg = new
        {
            account_number = 0,
            chain_id = chainId,
            fee = new object[0],
            memo = "Create Keplr Secret encryption key. Only approve requests by Keplr.",
            msgs = new object[0],
            sequence = 0
        };
        var msgBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(signMsg));
        var signedMsg = await wallet.SignMessage(msgBytes);
        txEncryptionKey = Hashes.SHA256(Convert.FromBase64String(signedMsg.Signature));

        await wallet.SetTxEncryptionSeed(txEncryptionKey, chainId);

        return txEncryptionKey;
    }


    /// <summary>
    /// Converts the address to bytes.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <returns>System.Byte[].</returns>
    public static byte[] AddressToBytes(string address)
    {
        var bech32encoder = Encoders.Bech32("secret");
        var bech32Words = bech32encoder.DecodeDataRaw(address, out var encodingType);
        var bech32FromWords = bech32encoder.ConvertBits(bech32Words, 5, 8, false);
        return bech32FromWords;
    }

    /// <summary>
    /// Get the address from bytes (bech32).
    /// </summary>
    /// <param name="addressBytes">The address bytes.</param>
    /// <param name="prefix">The prefix.</param>
    /// <returns>System.String.</returns>
    public static string BytesToAddress(byte[] addressBytes, string prefix = "secret")
    {
        var bech32encoder = Encoders.Bech32(prefix);
        var bech32words = bech32encoder.ConvertBits(addressBytes, 8, 5);
        var address = bech32encoder.EncodeData(bech32words, Bech32EncodingType.BECH32);
        return address;
    }

    /// <summary>
    /// Converts a base64 encoded PubKey to an address.
    /// </summary>
    /// <param name="base64Pubkey">The base64 pubkey.</param>
    /// <returns>System.String.</returns>
    public static string Base64PubkeyToAddress(string base64Pubkey)
    {
        var pubkeyBytes = Convert.FromBase64String(base64Pubkey);
        return PubkeyToAddress(pubkeyBytes);
    }

    /// <summary>
    /// Converts a PubKey to an address.
    /// </summary>
    /// <param name="pubkeyBytes">The pubkey bytes.</param>
    /// <returns>System.String.</returns>
    public static string PubkeyToAddress(byte[] pubkeyBytes)
    {
        var bech32encoder = Encoders.Bech32("secret");
        var sha256Hash = Hashes.SHA256(pubkeyBytes.ToArray());
        var ripemd160Hash = Hashes.RIPEMD160(sha256Hash, 0, sha256Hash.Length);

        var bech32words = bech32encoder.ConvertBits(ripemd160Hash, 8, 5);
        var address = bech32encoder.EncodeData(bech32words, Bech32EncodingType.BECH32);
        return address;
    }


    // internal

    internal static StdSignature EncodeSecp256k1Signature(PubKey pubKey, byte[] signature)
    {
        if (signature.Length != 64)
        {
            throw new Exception("Signature must be 64 bytes long. Cosmos SDK uses a 2x32 byte fixed length encoding for the secp256k1 signature integers r and s.");
        }
        var result = new StdSignature();
        result.Signature = Convert.ToBase64String(signature);
        result.PubKey = EncodeSecp256k1Pubkey(pubKey);

        return result;
    }

    internal static EncodedPubKey EncodeSecp256k1Pubkey(PubKey pubKey)
    {
        var keyBytes = pubKey.ToBytes();
        if (keyBytes.Length != 33 || (keyBytes[0] != 0x02 && keyBytes[0] != 0x03))
        {
            throw new Exception("Public key must be compressed secp256k1, i.e. 33 bytes starting with 0x02 or 0x03");
        }

        return new EncodedPubKey()
        {
            Type = EncodedPubKeyTypeEnum.tendermint_PubKeySecp256k1,
            AsBase64 = Convert.ToBase64String(keyBytes)
        };
    }

    internal static SignDoc MakeSignDocProto(byte[] bodyBytes, byte[] authInfoBytes, string chainId, ulong accountNumber)
    {
        var signerDoc = new SignDoc();
        signerDoc.BodyBytes = ByteString.FromBase64(Convert.ToBase64String(bodyBytes));
        signerDoc.AuthInfoBytes = ByteString.FromBase64(Convert.ToBase64String(authInfoBytes));
        signerDoc.ChainId = chainId;
        signerDoc.AccountNumber = accountNumber;

        return signerDoc;
    }

    internal static StdSignDoc MakeSignDocAmino(AminoMsg[] msgs, StdFee fee, string chainId, ulong accountNumber, ulong sequence, string? memo = null)
    {
        var feeAmino = fee.ConvertToStdFeeAmino();

        var signerDoc = new StdSignDoc();
        signerDoc.ChainId = chainId;
        signerDoc.AccountNumber = Convert.ToString(accountNumber);
        signerDoc.Sequence = Convert.ToString(sequence);
        signerDoc.Fee = feeAmino;
        signerDoc.Msgs = msgs;
        signerDoc.Memo = memo;

        return signerDoc;

    }

    internal static byte[] MakeAuthInfoBytes(Any encodedPubKey, ulong sequence, Cosmos.Base.V1Beta1.Coin[] feeAmount, ulong gasLimit, SignMode signMode = SignMode.Direct, string feeGranter = null)
    {
        var signers = new Dictionary<Any, ulong>();
        signers.Add(encodedPubKey, sequence);

        return MakeAuthInfoBytes(signers, feeAmount, gasLimit, signMode);
    }

    internal static byte[] MakeAuthInfoBytes(Dictionary<Any, ulong> signers, Cosmos.Base.V1Beta1.Coin[] feeAmount, ulong gasLimit, SignMode signMode = SignMode.Direct, string feeGranter = null)
    {
        var authInfo = new AuthInfo();
        var signerInfos = MakeSignerInfos(signers, signMode);
        authInfo.SignerInfos.Add(signerInfos);
        authInfo.Fee = new Fee()
        {
            GasLimit = gasLimit
        };
        authInfo.Fee.Amount.Add(feeAmount);
        if (!String.IsNullOrWhiteSpace(feeGranter))
        {
            authInfo.Fee.Granter = feeGranter;
        }        

        return authInfo.Encode();
    }

    internal static SignerInfo[] MakeSignerInfos(Dictionary<Any, ulong> signers, SignMode signMode = SignMode.Direct)
    {
        var signerInfos = new List<SignerInfo>();
        foreach (var signer in signers)
        {
            var signerInfo = new SignerInfo();
            signerInfo.PublicKey = signer.Key;
            signerInfo.ModeInfo = new ModeInfo()
            {
                Single = new ModeInfo.Types.Single()
                {
                    Mode = signMode
                }
            };
            signerInfo.Sequence = signer.Value;

            signerInfos.Add(signerInfo);
        }
        return signerInfos.ToArray();
    }    

    internal static Any EncodePubkey(EncodedPubKey pubKey)
    {
        if (pubKey.Type == EncodedPubKeyTypeEnum.tendermint_PubKeySecp256k1)
        {
            var key = new Cosmos.Crypto.Secp256K1.PubKey();
            key.Key = ByteString.FromBase64(pubKey.AsBase64);

            return Any.Pack(key,"");
        }
        return null;
    }

    // private

    

    private byte[] ExtractNonce(IMessage protoMsg)
    {       
        if (protoMsg.IsProtoType(MsgGrantAuthorization.MsgExecuteContract))
        {
            var msg = (Secret.Compute.V1Beta1.MsgExecuteContract)protoMsg;
            var slice = new ArraySegment<byte>(msg.Msg.Span.ToArray(), 0, 32).ToArray();
            return slice;
        }else if (protoMsg.IsProtoType(MsgGrantAuthorization.MsgInstantiateContract))
        {
            var msg = (Secret.Compute.V1Beta1.MsgInstantiateContract)protoMsg;
            var slice = new ArraySegment<byte>(msg.InitMsg.Span.ToArray(), 0, 32).ToArray();
            return slice;
        }
        return new byte[0];
    }

    private async Task PopulateCodeHash(Tx.MsgBase msg)
{
        if (msg.MsgType.IsProtoType(MsgGrantAuthorization.MsgExecuteContract))    
        {
            var msgExecuteContract = (Tx.MsgExecuteContract)msg;
            if (string.IsNullOrWhiteSpace(msgExecuteContract.CodeHash))
            {
                msgExecuteContract.CodeHash = await Query.Compute.GetCodeHash(msgExecuteContract.ContractAddress);
            }
        }
        else if (msg.MsgType.IsProtoType(MsgGrantAuthorization.MsgInstantiateContract))
        {
            var msgInstantiateContract = (Tx.MsgInstantiateContract)msg;
            if (string.IsNullOrWhiteSpace(msgInstantiateContract.CodeHash))
            {
                msgInstantiateContract.CodeHash = await Query.Compute.GetCodeHashByCodeId(msgInstantiateContract.CodeId);
            }
        }
    }

}

