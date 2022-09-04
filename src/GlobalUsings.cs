global using System.Text;
global using System.Diagnostics;
global using System.Globalization;
global using Newtonsoft.Json;

// gRPC
global using Grpc.Core;
global using Grpc.Core.Interceptors;
global using Grpc.Net.Client;
global using Grpc.Net.Client.Web;

// Protobuf
global using Google.Protobuf;
global using Google.Protobuf.WellKnownTypes;

// SecretNET
global using SecretNET;
global using SecretNET.Api;
global using SecretNET.Common;
global using SecretNET.Common.Storage;
global using SecretNET.Crypto.BIP39;
global using SecretNET.Crypto.DataEncoders;
global using SecretNET.Common.Crypto;
global using SecretNET.Crypto.BouncyCastle;
global using SecretNET.Crypto.Secp256k1;

// cosmos
global using Cosmos.Base.V1Beta1;
global using Cosmos.Tx.V1Beta1;