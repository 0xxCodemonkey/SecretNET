using Google.Protobuf.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretNET.Tx
{
    public class SecretTx
    {
        public GetTxResponse EncryptedResponse { get; internal set; }

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

        public SecretTx()
        {

        }

        public SecretTx(string txHash)
        {
            Txhash = txHash;
            Success = true;
        }

        public SecretTx(GetTxResponse response)
        {
            EncryptedResponse = response;
            if (EncryptedResponse?.TxResponse != null)
            {
                Txhash = EncryptedResponse?.TxResponse?.Txhash;
                Codespace = EncryptedResponse?.TxResponse?.Codespace;
                Height = EncryptedResponse.TxResponse.Height;
                Code = EncryptedResponse.TxResponse.Code;
                TxBytes = EncryptedResponse.TxResponse.Tx.Value.ToByteArray();
                Events = EncryptedResponse.TxResponse.Events;
                GasUsed = EncryptedResponse.TxResponse.GasUsed;
                GasWanted = EncryptedResponse.TxResponse.GasWanted;
            }
            else
            {
                Height = -1;
                Code = 666;
                GasUsed = -1;
                GasWanted = -1;
            }
        }

        public SecretTx(SecretTx secretTx)
        {
            EncryptedResponse = secretTx.EncryptedResponse;
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

        public SecretTx(Exception ex, string txHash)
        {
            Txhash = txHash;
            Exceptions.Add(ex);
        }

        public T GetResponseMsg<T>(int msgIndex = 0)
        {
            if (Data != null && Data.Count() > msgIndex)
            {
                return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(Data[msgIndex]));
            }
            return default;
        }

        public string GetResponseJson(int msgIndex = 0, bool formated = true)
        {
            if (Data != null && Data.Count() > msgIndex)
            {
                var json = Encoding.UTF8.GetString(Data[msgIndex]);
                if (formated)
                {
                    return JToken.Parse(json).ToString();
                }
                return json;
            }
            return null;
        }

        public string TryFindEventValue(string eventKey)
        {
            string result = String.Empty;
            if (EncryptedResponse?.TxResponse?.Logs != null)
            {
                result = EncryptedResponse.TxResponse.Logs
                .Where(a => a.Events != null && a.Events.Any())
                .Select(a => a.Events.FirstOrDefault(b => b.Attributes.Any(c => c.Key == eventKey)))
                .Select(a => a.Attributes.FirstOrDefault(b => b.Key == eventKey)?.Value)
                .FirstOrDefault();
            }
            return result;
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
                if (Data != null && Data.Count() > 0)
                {
                    var jsonData = Encoding.UTF8.GetString(Data[0]);
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
