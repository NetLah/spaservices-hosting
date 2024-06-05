#if NET8_0_OR_GREATER
using System.Buffers;
#endif

namespace NetLah.Extensions.SpaServices.Hosting;

// https://github.com/dotnet/aspnetcore/blob/main/src/Shared/ServerInfrastructure/HttpCharacters.cs
internal static class HttpCharacters
{
#if NET8_0_OR_GREATER
    private const string AlphaNumeric = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    private static readonly SearchValues<char> _allowedTokenChars = SearchValues.Create("!#$%&'*+-.^_`|~" + AlphaNumeric);

    public static int IndexOfInvalidTokenChar(ReadOnlySpan<char> span) => span.IndexOfAnyExcept(_allowedTokenChars);
#else
    private const int _tableSize = 128;
    private static readonly bool[] _alphaNumeric = InitializeAlphaNumeric();
    private static readonly bool[] _token = InitializeToken();
    private static bool[] InitializeAlphaNumeric()
    {
        // ALPHA and DIGIT https://tools.ietf.org/html/rfc5234#appendix-B.1
        var alphaNumeric = new bool[_tableSize];
        for (var c = '0'; c <= '9'; c++)
        {
            alphaNumeric[c] = true;
        }
        for (var c = 'A'; c <= 'Z'; c++)
        {
            alphaNumeric[c] = true;
        }
        for (var c = 'a'; c <= 'z'; c++)
        {
            alphaNumeric[c] = true;
        }
        return alphaNumeric;
    }

    private static bool[] InitializeToken()
    {
        // tchar https://tools.ietf.org/html/rfc7230#appendix-B
        var token = new bool[_tableSize];
        Array.Copy(_alphaNumeric, token, _tableSize);
        token['!'] = true;
        token['#'] = true;
        token['$'] = true;
        token['%'] = true;
        token['&'] = true;
        token['\''] = true;
        token['*'] = true;
        token['+'] = true;
        token['-'] = true;
        token['.'] = true;
        token['^'] = true;
        token['_'] = true;
        token['`'] = true;
        token['|'] = true;
        token['~'] = true;
        return token;
    }

    public static int IndexOfInvalidTokenChar(string s)
    {
        var token = _token;

        for (var i = 0; i < s.Length; i++)
        {
            var c = s[i];
            if (c >= (uint)token.Length || !token[c])
            {
                return i;
            }
        }

        return -1;
    }
#endif
}
