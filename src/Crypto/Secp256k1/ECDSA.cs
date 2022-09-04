#nullable enable

namespace SecretNET.Crypto.Secp256k1
{
    class ECDSA
	{
		static readonly Lazy<ECDSA> _Instance = new Lazy<ECDSA>(CreateInstance, true);
		static ECDSA CreateInstance()
		{
			return new ECDSA();
		}
		public static ECDSA Instance => _Instance.Value;

		private readonly ECMultContext ctx;
		public ECDSA() : this(null)
		{

		}
		public ECDSA(ECMultContext? ctx)
		{
			this.ctx = ctx ?? ECMultContext.Instance;
		}
	}
}
#nullable restore
