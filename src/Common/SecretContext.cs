using SecretNET.Crypto;

namespace SecretNET.Common;

internal class SecretContext
{
    static readonly Lazy<Context> _Instance = new Lazy<Context>(CreateInstance, true);
    public static Context Instance => _Instance.Value;
    static Context CreateInstance()
    {
        var gen = new ECMultGenContext();
        gen.Blind(RandomUtils.GetBytes(32));
        return new Context(new ECMultContext(), gen);
    }
}

