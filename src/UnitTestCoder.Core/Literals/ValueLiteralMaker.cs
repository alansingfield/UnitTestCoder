using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestCoder.Core.Literal
{
    public partial class ValueLiteralMaker : IValueLiteralMaker
    {
        public string Literal(object arg)
        {
            if(arg == null)
                return "null";

            var type = arg.GetType();

            switch(arg)
            {
                case string x:          return stringLiteral(x);

                case int x:             return $"{x}";
                case uint x:            return $"{x}u";
                case long x:            return $"{x}L";
                case ulong x:           return $"{x}ul";

                case short x:           return $"{x}";
                case ushort x:          return $"{x}";
                case byte x:            return $"{x}";
                case sbyte x:           return $"{x}";

                case decimal x:         return $"{x.ToString(CultureInfo.InvariantCulture)}m";
                case double x:          return doubleLiteral(x);
                case float x:           return floatLiteral(x);

                case DateTime x:        return $@"DateTime.Parse(""{x.ToString("O")}"")";
                case DateTimeOffset x:  return $@"DateTimeOffset.Parse(""{x.ToString("O")}"")";
                case TimeSpan x:        return $@"TimeSpan.Parse(""{x.ToString("g",
                                                CultureInfo.InvariantCulture)}"")";

#if NET6_0_OR_GREATER
                case DateOnly x:        return $@"DateOnly.Parse(""{x.ToString("O")}"")";
                case TimeOnly x:        return timeOnlyLiteral(x);
#endif
                case bool x:            return x ? "true" : "false";
                case Guid x:            return $@"Guid.Parse(""{x}"")";
                case Enum x:            return enumLiteral(x, type);

                case byte[] x:          return byteArrayLiteral(x);
                case string[] x:        return stringArrayLiteral(x);
            }

            throw new Exception($"Unexpected data type {type}");
        }

        #if NET6_0_OR_GREATER
        private static string timeOnlyLiteral(TimeOnly x)
        {
            string literal = x.ToString("O");

            // If 00:01:02.000000 just want 00:01:02
            if(x.Millisecond == 0)
                literal = literal.Substring(0, 8);
            
            return $@"TimeOnly.Parse(""{literal}"")";
        }
        #endif

        public bool CanMake(Type type)
        {
            Type t = Nullable.GetUnderlyingType(type) ?? type;

            return (
                   t == typeof(string)

                || t == typeof(int)
                || t == typeof(uint)
                || t == typeof(long)
                || t == typeof(ulong)

                || t == typeof(short)
                || t == typeof(ushort)
                || t == typeof(byte)
                || t == typeof(sbyte)

                || t == typeof(decimal)
                || t == typeof(double)
                || t == typeof(float)

                || t == typeof(DateTime)
                || t == typeof(DateTimeOffset)
                || t == typeof(TimeSpan)

#if NET6_0_OR_GREATER
                || t == typeof(DateOnly)
                || t == typeof(TimeOnly)
#endif
                || t == typeof(bool)
                || t == typeof(Guid)
                || t.IsEnum

                || t == typeof(byte[])
                || t == typeof(string[])
              );
        }

        private string doubleLiteral(double x)
        {
            // Values very close to Double.MaxValue / MinValue are not compilable
            // as G15 rounded literals because the rounding sends them higher than
            // the maximum / lower than the minimum. It's clearer to write MinValue
            // and MaxValue anyway...
            switch(x)
            {
                case double.MaxValue: return "double.MaxValue";
                case double.MinValue: return "double.MinValue";
                default: return $"{x.ToString(CultureInfo.InvariantCulture)}d";
            }
        }

        private string floatLiteral(float x)
        {
            switch(x)
            {
                case float.MaxValue: return "float.MaxValue";
                case float.MinValue: return "float.MinValue";
                default: return $"{x.ToString(CultureInfo.InvariantCulture)}f";
            }
        }


        private string stringLiteral(string arg)
        {
            string s = (string)arg;
            if(s.Contains(@"""") || s.Contains(@"\"))
                return $@"@""{s.Replace(@"""", @"""""")}""";

            return $@"""{stringEscape(s)}""";
        }

        private string byteArrayLiteral(byte[] arg)
        {
            int len = arg.Length;

            bool useLineBreaks = len > 16;

            string separator(int index)
            {
                if(!useLineBreaks)
                    return " ";

                if(index == len - 1)    // Last item always gets a line break.
                    return "\r\n";

                if(index % 16 == 15)
                    return "\r\n";

                return " ";
            }

            return "new byte[] {"
                + (useLineBreaks ? "\r\n" : " ")
                + String.Join("",
                    arg.Select((x, i) => $"0x{x:X2},{(separator(i))}"))
                + "}";
        }

        private string stringArrayLiteral(object arg)
        {
            var array = (string[])arg;

            if(array.Length == 0)
                return "new string[0]";

            var literals = array.Select(x => x != null ? stringLiteral(x) : "null").ToList();

            // Use line breaks if result would be over 80 chars
            int totalLen = 0;
            bool useLineBreaks = false;
            foreach(var l in literals)
            {
                totalLen += l.Length;
                if(totalLen > 80)
                {
                    useLineBreaks = true;
                    break;
                }
            }

            return "new[] {"
                + (useLineBreaks ? "\r\n" : " ")
                + String.Join(useLineBreaks ? ",\r\n" : ", ", literals)
                + (useLineBreaks ? ",\r\n" : " ")
                + "}";
        }

        private string stringEscape(string arg)
        {
            if(arg.Contains("\\")) arg = arg.Replace("\\", @"\\");
            if(arg.Contains("\r")) arg = arg.Replace("\r", @"\r");
            if(arg.Contains("\n")) arg = arg.Replace("\n", @"\n");
            if(arg.Contains("\t")) arg = arg.Replace("\t", @"\t");

            return arg;
        }


    }
}
