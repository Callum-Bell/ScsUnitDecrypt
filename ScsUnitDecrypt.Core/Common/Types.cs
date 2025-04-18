using System.Globalization;
using ScsUnitDecrypt.Core.Utils;

namespace ScsUnitDecrypt.Core.Common
{
    internal struct Float2
    {
        internal float X { get; set; }
        internal float Y { get; set; }

        public string GetComment()
        {
            return
                $"({X.ToString(CultureInfo.CreateSpecificCulture("en-US"))}f, {Y.ToString(CultureInfo.CreateSpecificCulture("en-US"))}f)";
        }

        public override string ToString()
        {
            return $"({SiiUtils.FloatToSiiFormat(X)}, {SiiUtils.FloatToSiiFormat(Y)})";
        }
    }

    internal struct Float3
    {
        internal float X { get; set; }
        internal float Y { get; set; }
        internal float Z { get; set; }

        public string GetComment()
        {
            return
                $"({X.ToString(CultureInfo.CreateSpecificCulture("en-US"))}f, {Y.ToString(CultureInfo.CreateSpecificCulture("en-US"))}f, " +
                $"{Z.ToString(CultureInfo.CreateSpecificCulture("en-US"))}f)";
        }

        public override string ToString()
        {
            return $"({SiiUtils.FloatToSiiFormat(X)}, {SiiUtils.FloatToSiiFormat(Y)}, {SiiUtils.FloatToSiiFormat(Z)})";
        }
    }

    internal struct Float4
    {
        internal float X { get; set; }
        internal float Y { get; set; }
        internal float Z { get; set; }
        internal float W { get; set; }

        public string GetComment()
        {
            return
                $"({X.ToString(CultureInfo.CreateSpecificCulture("en-US"))}f, {Y.ToString(CultureInfo.CreateSpecificCulture("en-US"))}f, " +
                $"{Z.ToString(CultureInfo.CreateSpecificCulture("en-US"))}f, {W.ToString(CultureInfo.CreateSpecificCulture("en-US"))}f)";
        }

        public override string ToString()
        {
            return
                $"({SiiUtils.FloatToSiiFormat(X)}; {SiiUtils.FloatToSiiFormat(Y)}, {SiiUtils.FloatToSiiFormat(Z)}, {SiiUtils.FloatToSiiFormat(W)})";
        }
    }

    internal struct Int2
    {
        internal int X { get; set; }
        internal int Y { get; set; }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }

    internal struct Int3
    {
        internal int X { get; set; }
        internal int Y { get; set; }
        internal int Z { get; set; }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }
    }

    internal struct Float7
    {
        internal float F1 { get; set; }
        internal float F2 { get; set; }
        internal float F3 { get; set; }
        internal float F4 { get; set; }
        internal float F5 { get; set; }
        internal float F6 { get; set; }
        internal float F7 { get; set; }

        public string GetComment()
        {
            return
                $"({F1.ToString(CultureInfo.CreateSpecificCulture("en-US"))}f, {F2.ToString(CultureInfo.CreateSpecificCulture("en-US"))}f, " +
                $"{F3.ToString(CultureInfo.CreateSpecificCulture("en-US"))}f) ({F4.ToString(CultureInfo.CreateSpecificCulture("en-US"))}f; " +
                $"{F5.ToString(CultureInfo.CreateSpecificCulture("en-US"))}f, {F6.ToString(CultureInfo.CreateSpecificCulture("en-US"))}f, " +
                $"{F7.ToString(CultureInfo.CreateSpecificCulture("en-US"))}f)";
        }

        public override string ToString()
        {
            return
                $"({SiiUtils.FloatToSiiFormat(F1)}, {SiiUtils.FloatToSiiFormat(F2)}, {SiiUtils.FloatToSiiFormat(F3)}) " +
                $"({SiiUtils.FloatToSiiFormat(F4)}; {SiiUtils.FloatToSiiFormat(F5)}, {SiiUtils.FloatToSiiFormat(F6)}, {SiiUtils.FloatToSiiFormat(F7)}) ";
        }
    }

    internal struct Float8
    {
        internal float F1 { get; set; }
        internal float F2 { get; set; }
        internal float F3 { get; set; }
        internal float F4 { get; set; }
        internal float F5 { get; set; }
        internal float F6 { get; set; }
        internal float F7 { get; set; }
        internal float F8 { get; set; }

        public string GetComment()
        {
            var bias = (int) F4;
            var f1Bias = F1 + (((bias & 0xFFF) - 2048) << 9);
            var f3Bias = F3 + ((((bias >> 12) & 0xFFF) - 2048) << 9);

            return
                $"({f1Bias.ToString(CultureInfo.CreateSpecificCulture("en-US"))}f, {F2.ToString(CultureInfo.CreateSpecificCulture("en-US"))}f, " +
                $"{f3Bias.ToString(CultureInfo.CreateSpecificCulture("en-US"))}f) ({F5.ToString(CultureInfo.CreateSpecificCulture("en-US"))}f; " +
                $"{F6.ToString(CultureInfo.CreateSpecificCulture("en-US"))}f, {F7.ToString(CultureInfo.CreateSpecificCulture("en-US"))}f, " +
                $"{F8.ToString(CultureInfo.CreateSpecificCulture("en-US"))}f)";
        }

        public override string ToString()
        {
            var bias = (int) F4;
            var f1Bias = F1 + (((bias & 0xFFF) - 2048) << 9);
            var f3Bias = F3 + ((((bias >> 12) & 0xFFF) - 2048) << 9);

            return
                $"({SiiUtils.FloatToSiiFormat(f1Bias)}, {SiiUtils.FloatToSiiFormat(F2)}, {SiiUtils.FloatToSiiFormat(f3Bias)}) " +
                $"({SiiUtils.FloatToSiiFormat(F5)}; {SiiUtils.FloatToSiiFormat(F6)}, {SiiUtils.FloatToSiiFormat(F7)}, {SiiUtils.FloatToSiiFormat(F8)})";
        }
    }

    internal struct Quaternion
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public override string ToString()
        {
            return $"({X}, {Y}, {Z}, {W})";
        }
    }
}