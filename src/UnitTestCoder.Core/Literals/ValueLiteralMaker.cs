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

            switch(arg)
            {
                case string x:      return stringLiteral(x);
                case int x:         return intLiteral(x);
                case decimal x:     return decimalLiteral(x);
                case double x:      return doubleLiteral(x);
                case DateTime x:    return dateTimeLiteral(x);
                case TimeSpan x:    return timeSpanLiteral(x);
                case bool x:        return boolLiteral(x);
                case Guid x:        return guidLiteral(x);
                case byte[] x:      return byteArrayLiteral(x);
                case string[] x:    return stringArrayLiteral(x);
                case Enum x:        return enumLiteral(x, type);
            }

            throw new Exception($"Unexpected data type {type}");
        }

        public bool CanMake(Type type)
        {
            if(
                   type == typeof(string)
                || type == typeof(int)
                || type == typeof(int?)
                || type == typeof(decimal)
                || type == typeof(decimal?)
                || type == typeof(double)
                || type == typeof(double?)
                || type == typeof(DateTime)
                || type == typeof(DateTime?)
                || type == typeof(TimeSpan)
                || type == typeof(TimeSpan?)
                || type == typeof(bool)
                || type == typeof(bool?)
                || type == typeof(Guid)
                || type == typeof(Guid?)
                || type == typeof(byte[])
                || type == typeof(string[])
                || type.IsEnum
              )
                return true;

            return false;
        }

        private static string intLiteral(object arg)
        {
            return arg.ToString();
        }

        private static string decimalLiteral(object arg)
        {
            return $"{((decimal)arg).ToString(CultureInfo.InvariantCulture)}m";
        }

        private static string doubleLiteral(object arg)
        {
            return $"{((double)arg).ToString(CultureInfo.InvariantCulture)}d";
        }

        private static string dateTimeLiteral(object arg)
        {
            return $@"DateTime.Parse(""{((DateTime)arg).ToString("O")}"")";
        }

        private static string timeSpanLiteral(object arg)
        {
            return $@"TimeSpan.Parse(""{((TimeSpan)arg).ToString("g", CultureInfo.InvariantCulture)}"")";
        }

        private static string boolLiteral(object arg)
        {
            return ((bool)arg) ? "true" : "false";
        }

        private static string guidLiteral(object arg)
        {
            return $@"Guid.Parse(""{arg}"")";
        }


        private static string enumLiteral(object arg, Type type)
        {
            // If enum is in a nested class use dot notation not plus.
            string typeFullName = type.FullName.Replace("+", ".");

            return $"{typeFullName}.{Enum.GetName(type, arg)}";
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


    }
}
