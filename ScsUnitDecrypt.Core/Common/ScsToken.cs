using System.Text;

namespace ScsUnitDecrypt.Core.Common
{
    internal struct UlDiv
    {
        internal ulong Quot;
        internal ulong Rem;
    }

    internal static class ScsToken
    {
        private static readonly char[] Letters =
        {
            '\0', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b',
            'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o',
            'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '_'
        };

        private static ulong PowUl(int num)
        {
            ulong res = 1;
            for (var i = 0; num > i; i++) res *= (ulong) Letters.Length;

            return res;
        }

        private static int GetIdChar(char letter)
        {
            for (var i = 0; i < Letters.Length; i++)
                if (letter == Letters[i])
                    return i;

            return 0;
        }

        private static UlDiv Div(ulong num, ulong divider)
        {
            var res = new UlDiv {Rem = num % divider, Quot = num / divider};
            return res;
        }

        internal static ulong StringToToken(string text)
        {
            ulong res = 0;
            var len = text.Length;
            for (var i = 0; i < len; i++) res += PowUl(i) * (ulong) GetIdChar(text.ToLower()[i]);

            return res;
        }

        internal static string TokenToString(ulong token)
        {
            var sb = new StringBuilder();
            for (var i = 0; token != 0; i++)
            {
                var res = Div(token, 38);
                token = res.Quot;
                sb.Append(Letters[res.Rem]);
            }

            return sb.ToString();
        }
    }
}