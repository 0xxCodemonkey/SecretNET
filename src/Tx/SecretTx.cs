using Google.Protobuf.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Cosmos.Base.Abci.V1Beta1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretNET.Tx
{
    public class SecretTx
    {
        public TxResponse TxResponse { get; internal set; }

        #region EncryptedResponse reference props

        public long Height { get; internal set; }

        public string Codespace { get; internal set; }

        public uint Code { get; internal set; }

        public byte[] TxBytes { get; internal set; }

        public RepeatedField<Tendermint.Abci.Event> Events { get; internal set; }

        public long GasUsed { get; internal set; }

        public long GasWanted { get; internal set; }

        #endregion

        public string Txhash { get; internal set; }

        public string RawLog { get; internal set; }

        public List<CosmosJsonLog> JsonLog { get; internal set; }

        public List<CosmosArrayLog> ArrayLog { get; internal set; }

        public byte[][] Data { get; internal set; }

        public bool Success { get; internal set; } = false;

        public List<Exception> Exceptions { get; internal set; } = new List<Exception>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SecretTx"/> class.
        /// </summary>
        public SecretTx()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SecretTx"/> class.
        /// </summary>
        /// <param name="txHash">The tx hash.</param>
        public SecretTx(string txHash)
        {
            Txhash = txHash;
            Success = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SecretTx"/> class.
        /// </summary>
        /// <param name="response">The response.</param>
        public SecretTx(TxResponse response)
        {
            TxResponse = response;
            if (TxResponse != null)
            {
                Txhash = TxResponse?.Txhash;
                Codespace = TxResponse?.Codespace;
                Height = TxResponse.Height;
                Code = TxResponse.Code;
                TxBytes = TxResponse.Tx?.Value?.ToByteArray();
                Events = TxResponse?.Events;
                GasUsed = TxResponse.GasUsed;
                GasWanted = TxResponse.GasWanted;
            }
            else
            {
                Height = -1;
                Code = 666;
                GasUsed = -1;
                GasWanted = -1;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SecretTx"/> class.
        /// </summary>
        /// <param name="secretTx">The secret tx.</param>
        public SecretTx(SecretTx secretTx)
        {
            TxResponse = secretTx.TxResponse;
            Height = secretTx.Height;
            Codespace = secretTx.Codespace;
            Code = secretTx.Code;
            TxBytes = secretTx.TxBytes;
            Events = secretTx.Events;
            GasUsed = secretTx.GasUsed;
            GasWanted = secretTx.GasWanted;
            Txhash = secretTx.Txhash;
            RawLog = secretTx.RawLog;
            JsonLog = secretTx.JsonLog;
            ArrayLog = secretTx.ArrayLog;
            Data = secretTx.Data;
            Success = secretTx.Success;
            Exceptions = secretTx.Exceptions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SecretTx"/> class.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="txHash">The tx hash.</param>
        public SecretTx(Exception ex, string txHash)
        {
            Txhash = txHash;
            Exceptions.Add(ex);
        }

        /// <summary>
        /// Gets the response MSG.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msgIndex">Index of the MSG.</param>
        /// <returns>T.</returns>
        public T GetResponseMsg<T>(int msgIndex = 0)
        {
            var msgData = GetResponseData(msgIndex);
            if (msgData != null)
            {
                return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(msgData));
            }
            return default;
        }

        /// <summary>
        /// Gets the response json.
        /// </summary>
        /// <param name="msgIndex">Index of the MSG.</param>
        /// <param name="formated">if set to <c>true</c> [formated].</param>
        /// <returns>System.String.</returns>
        public string GetResponseJson(int msgIndex = 0, bool formated = true)
        {
            var msgData = GetResponseData(msgIndex);
            if (msgData != null)
            {
                var json = Encoding.UTF8.GetString(msgData);
                if (formated)
                {
                    return JToken.Parse(json).ToString();
                }
                return json;
            }
            return null;
        }

        /// <summary>
        /// Tries the find event value.
        /// </summary>
        /// <param name="eventKey">The event key.</param>
        /// <returns>System.String.</returns>
        public string TryFindEventValue(string eventKey)
        {
            string result = String.Empty;
            if (TxResponse?.Logs != null)
            {
                result = TxResponse.Logs
                .Where(a => a.Events != null && a.Events.Any())
                .Select(a => a.Events.FirstOrDefault(b => b.Attributes.Any(c => c.Key == eventKey)))
                .Select(a => a.Attributes.FirstOrDefault(b => b.Key == eventKey)?.Value)
                .FirstOrDefault();
            }
            return result;
        }

        // private
        internal byte[] GetResponseData(int msgIndex = 0)
        {
            if (TxResponse?.Tx != null)
            {
                var decodedTx = TxResponse.Tx.Unpack<Cosmos.Tx.V1Beta1.Tx>();
                if (decodedTx?.Body?.Messages?.Count -1 >= msgIndex)
                {
                    var rawMsg = decodedTx.Body.Messages[msgIndex];
                    if (rawMsg.TypeUrl.IsProtoType(MsgGrantAuthorization.MsgExecuteContract))
                    {
                        var msg = Secret.Compute.V1Beta1.MsgExecuteContractResponse.Parser.ParseFrom(Data[msgIndex]);
                        if (msg != null)
                        {
                            return msg.Data.ToArray();
                        }
                    }
                    else
                    {
                        return Data[msgIndex];
                    }
                }
            }
            return null;
        }
    }

    public class SingleSecretTx<T> : SecretTx
    {
        public T Response { get; set; }

        public SingleSecretTx(SecretTx secretTx) : base(secretTx)
        {
            ParseData();
        }

        private void ParseData()
        {
            try
            {
                var msgData = GetResponseData(0);
                if (msgData != null)
                {
                    var jsonData = Encoding.UTF8.GetString(msgData);
                    Response = JsonConvert.DeserializeObject<T>(jsonData);
                }
            }
            catch (Exception ex)
            {
                Exceptions = Exceptions != null ? Exceptions : new List<Exception>();
                Exceptions.Add(ex);
            }
        }

    }
}
