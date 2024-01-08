using System.Numerics;

namespace SessionVerifierClient;

public static class HashUtil
{
    // https://gist.github.com/ammaraskar/7b4a3f73bee9dc4136539644a0f27e63
    public static string ToMinecraftSHA1String(byte[] input)
    {
        Array.Reverse(input);
        BigInteger b = new BigInteger(input);
        // very annoyingly, BigInteger in C# tries to be smart and puts in
        // a leading 0 when formatting as a hex number to allow roundtripping 
        // of negative numbers, thus we have to trim it off.
        if (b < 0)
        {
            // toss in a negative sign if the interpreted number is negative
            return "-" + (-b).ToString("x").TrimStart('0');
        }
        else
        {
            return b.ToString("x").TrimStart('0');
        }
    }
}