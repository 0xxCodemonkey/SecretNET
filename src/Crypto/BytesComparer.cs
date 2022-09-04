namespace SecretNET.Crypto;

internal class BytesComparer : Comparer<byte[]>
{

	private static readonly BytesComparer _Instance = new BytesComparer();
    internal static BytesComparer Instance
	{
		get
		{
			return _Instance;
		}
	}
	private BytesComparer()
	{

	}
	public override int Compare(byte[] x, byte[] y)
	{
		var len = Math.Min(x.Length, y.Length);
		for (var i = 0; i < len; i++)
		{
			var c = x[i].CompareTo(y[i]);
			if (c != 0)
			{
				return c;
			}
		}

		return x.Length.CompareTo(y.Length);
	}

	public int Compare(ReadOnlySpan<byte> x, ReadOnlySpan<byte> y)
	{
		var len = Math.Min(x.Length, y.Length);
		for (var i = 0; i < len; i++)
		{
			var c = x[i].CompareTo(y[i]);
			if (c != 0)
			{
				return c;
			}
		}

		return x.Length.CompareTo(y.Length);
	}
}
