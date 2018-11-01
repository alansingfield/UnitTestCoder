﻿using System;
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
                string s = (string)arg;
                if(s.Contains(@"""") || s.Contains(@"\"))
                    return $@"@""{s.Replace(@"""", @"""""")}""";

                return $@"""{stringEscape(s)}""";
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

            if(type.IsEnum)
            {
                // If enum is in a nested class use dot notation not plus.
                string typeFullName = type.FullName.Replace("+", ".");

                return $"{typeFullName}.{Enum.GetName(type, arg)}";
            }

            throw new Exception("Unexpected data type");
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