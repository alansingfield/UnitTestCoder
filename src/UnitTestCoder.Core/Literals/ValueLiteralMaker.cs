using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestCoder.Core.Literal
{
    public class ValueLiteralMaker : IValueLiteralMaker
    {
        public string Literal(object arg)
        {
            if(arg == null)
                return "null";

            var type = arg.GetType();

            if(type == typeof(string))
            {
                return stringLiteral(arg);
            }

            if(type == typeof(int) || type == (typeof(int?)))
            {
                return arg.ToString();
            }

            if(type == typeof(decimal) || type == (typeof(decimal?)))
            {
                return $"{((decimal)arg).ToString(CultureInfo.InvariantCulture)}m";
            }

            if(type == typeof(double) || type == (typeof(double?)))
            {
                return $"{((double)arg).ToString(CultureInfo.InvariantCulture)}d";
            }

            if(type == typeof(DateTime) || type == (typeof(DateTime?)))
            {
                return $@"DateTime.Parse(""{((DateTime)arg).ToString("O")}"")";
            }

            if(type == typeof(TimeSpan) || type == (typeof(TimeSpan?)))
            {
                return $@"TimeSpan.Parse(""{((TimeSpan)arg).ToString("g", CultureInfo.InvariantCulture)}"")";
            }

            if(type == typeof(bool) || type == (typeof(bool?)))
            {
                return ((bool)arg) ? "true" : "false";
            }

            if(type == typeof(Guid) || type == (typeof(Guid?)))
            {
                return $@"Guid.Parse(""{arg}"")";
            }

            if(type == typeof(byte[]))
            {
                return byteArrayLiteral(arg);
            }

            if(type == typeof(string[]))
            {
                return stringArrayLiteral(arg);
            }

            if(type.IsEnum)
            {
                // If enum is in a nested class use dot notation not plus.
                string typeFullName = type.FullName.Replace("+", ".");

                return $"{typeFullName}.{Enum.GetName(type, arg)}";
            }

            throw new Exception("Unexpected data type");
        }

        private string stringLiteral(object arg)
        {
            string s = (string)arg;
            if(s.Contains(@"""") || s.Contains(@"\"))
                return $@"@""{s.Replace(@"""", @"""""")}""";

            return $@"""{stringEscape(s)}""";
        }

        private string byteArrayLiteral(object arg)
        {
            byte[] bytes = (byte[])arg;

            // new byte[] { 0x01, 0x04 }

            int len = bytes.Length;

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
                    bytes.Select((x, i) => $"0x{x:X2},{(separator(i))}"))
                + "}";
        }

        private string stringArrayLiteral(object arg)
        {
            var array = (string[])arg;

            if(array.Length == 0)
                return "new string[0]";

            var literals = array.Select(x => stringLiteral(x)).ToList();

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

        public bool CanMake(Type type)
        {
            if(type.IsValueType || type == typeof(string) || type == typeof(byte[]) || type == typeof(string[]))
                return true;

            return false;
        }
    }
}
