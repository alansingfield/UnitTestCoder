﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using UnitTestCoder.Core.Literal;

namespace UnitTestCoder.Core.Tests.Literals
{
    [TestClass]
    public class ValueLiteralMakerTests
    {
        private IValueLiteralMaker _valueLiteralMaker;

        [TestInitialize]
        public void Init()
        {
            _valueLiteralMaker = new ValueLiteralMaker();
        }

        [TestMethod]
        public void ValueLiteralMakerStringStandard()
        {
            _valueLiteralMaker.Literal("Hello").ShouldBe(@"""Hello""");
        }

        [TestMethod]
        public void ValueLiteralMakerStringBackslash()
        {
            _valueLiteralMaker.Literal(@"Back\Slash").ShouldBe(@"@""Back\Slash""");
        }

        [TestMethod]
        public void ValueLiteralMakerStringQuote()
        {
            _valueLiteralMaker.Literal(@"""quoted""").ShouldBe(@"@""""""quoted""""""");
        }


        [TestMethod]
        public void ValueLiteralMakerStringCrlf()
        {
            _valueLiteralMaker.Literal("Carriage\r\nReturn").ShouldBe(@"""Carriage\r\nReturn""");
        }

        [TestMethod]
        public void ValueLiteralMakerInt()
        {
            _valueLiteralMaker.CanMake(typeof(int)).ShouldBe(true);
            _valueLiteralMaker.CanMake(typeof(int?)).ShouldBe(true);
            _valueLiteralMaker.Literal(int.MaxValue).ShouldBe("2147483647");
            _valueLiteralMaker.Literal(int.MinValue).ShouldBe("-2147483648");
        }
        [TestMethod]
        public void ValueLiteralMakerLong()
        {
            _valueLiteralMaker.CanMake(typeof(long)).ShouldBe(true);
            _valueLiteralMaker.CanMake(typeof(long?)).ShouldBe(true);
            _valueLiteralMaker.Literal(long.MaxValue).ShouldBe("9223372036854775807L");
            _valueLiteralMaker.Literal(long.MinValue).ShouldBe("-9223372036854775808L");
        }

        [TestMethod]
        public void ValueLiteralMakerULong()
        {
            _valueLiteralMaker.CanMake(typeof(ulong)).ShouldBe(true);
            _valueLiteralMaker.CanMake(typeof(ulong?)).ShouldBe(true);
            _valueLiteralMaker.Literal(ulong.MaxValue).ShouldBe("18446744073709551615ul");
        }


        [TestMethod]
        public void ValueLiteralMakerBoolean()
        {
            _valueLiteralMaker.CanMake(typeof(bool)).ShouldBe(true);
            _valueLiteralMaker.CanMake(typeof(bool?)).ShouldBe(true);

            _valueLiteralMaker.Literal(false).ShouldBe(@"false");
            _valueLiteralMaker.Literal(true).ShouldBe(@"true");
        }

        [TestMethod]
        public void ValueLiteralMakerDecimal()
        {
            _valueLiteralMaker.CanMake(typeof(decimal)).ShouldBe(true);
            _valueLiteralMaker.CanMake(typeof(decimal?)).ShouldBe(true);

            _valueLiteralMaker.Literal(1.23456m).ShouldBe("1.23456m");
            _valueLiteralMaker.Literal(decimal.MaxValue).ShouldBe("79228162514264337593543950335m");
            _valueLiteralMaker.Literal(decimal.MinValue).ShouldBe("-79228162514264337593543950335m");
        }

        [TestMethod]
        public void ValueLiteralMakerDouble()
        {
            _valueLiteralMaker.CanMake(typeof(double)).ShouldBe(true);
            _valueLiteralMaker.CanMake(typeof(double?)).ShouldBe(true);

            _valueLiteralMaker.Literal(1.23456d).ShouldBe("1.23456d");
            _valueLiteralMaker.Literal(double.MaxValue).ShouldBe("double.MaxValue");
            _valueLiteralMaker.Literal(double.MinValue).ShouldBe("double.MinValue");
        }

        [TestMethod]
        public void ValueLiteralMakerFloat()
        {
            _valueLiteralMaker.CanMake(typeof(float)).ShouldBe(true);
            _valueLiteralMaker.CanMake(typeof(float?)).ShouldBe(true);

            _valueLiteralMaker.Literal(1.23456f).ShouldBe("1.23456f");
            _valueLiteralMaker.Literal(float.MaxValue).ShouldBe("float.MaxValue");
            _valueLiteralMaker.Literal(float.MinValue).ShouldBe("float.MinValue");
        }


        [TestMethod]
        public void ValueLiteralMakerShort()
        {
            _valueLiteralMaker.CanMake(typeof(short)).ShouldBe(true);
            _valueLiteralMaker.CanMake(typeof(short?)).ShouldBe(true);

            _valueLiteralMaker.Literal((short)32767).ShouldBe("32767");
            _valueLiteralMaker.Literal((short)-32768).ShouldBe("-32768");
        }

        [TestMethod]
        public void ValueLiteralMakerUShort()
        {
            _valueLiteralMaker.CanMake(typeof(ushort)).ShouldBe(true);
            _valueLiteralMaker.CanMake(typeof(ushort?)).ShouldBe(true);

            _valueLiteralMaker.Literal((ushort)65535).ShouldBe("65535");
            _valueLiteralMaker.Literal((ushort)0).ShouldBe("0");
        }

        [TestMethod]
        public void ValueLiteralMakerByte()
        {
            _valueLiteralMaker.CanMake(typeof(byte)).ShouldBe(true);
            _valueLiteralMaker.CanMake(typeof(byte?)).ShouldBe(true);

            _valueLiteralMaker.Literal((byte)255).ShouldBe("255");
            _valueLiteralMaker.Literal((byte)0).ShouldBe("0");
        }

        [TestMethod]
        public void ValueLiteralMakerSByte()
        {
            _valueLiteralMaker.CanMake(typeof(sbyte)).ShouldBe(true);
            _valueLiteralMaker.CanMake(typeof(sbyte?)).ShouldBe(true);

            _valueLiteralMaker.Literal((sbyte)127).ShouldBe("127");
            _valueLiteralMaker.Literal((sbyte)-128).ShouldBe("-128");
        }


        [TestMethod]
        public void ValueLiteralMakerTimeSpan()
        {
            _valueLiteralMaker.CanMake(typeof(TimeSpan)).ShouldBe(true);
            _valueLiteralMaker.CanMake(typeof(TimeSpan?)).ShouldBe(true);

            _valueLiteralMaker.Literal(new TimeSpan(13,45,23)).ShouldBe(@"TimeSpan.Parse(""13:45:23"")");
        }

        [TestMethod]
        public void ValueLiteralMakerTimeSpanFull()
        {
            _valueLiteralMaker.Literal(new TimeSpan(365, 23, 58, 59, 123).Add(TimeSpan.FromTicks(4567)))
                .ShouldBe(@"TimeSpan.Parse(""365:23:58:59.1234567"")");
        }

        [TestMethod]
        public void ValueLiteralMakerDateTime()
        {
            _valueLiteralMaker.CanMake(typeof(DateTime)).ShouldBe(true);
            _valueLiteralMaker.CanMake(typeof(DateTime?)).ShouldBe(true);

            _valueLiteralMaker.Literal(DateTime.Parse("2014-03-09T15:59:23.1234567")).ShouldBe(@"DateTime.Parse(""2014-03-09T15:59:23.1234567"")");
        }

        #if NET6_0_OR_GREATER
        [TestMethod]
        public void ValueLiteralMakerTimeOnly()
        {
            _valueLiteralMaker.CanMake(typeof(TimeOnly)).ShouldBe(true);
            _valueLiteralMaker.CanMake(typeof(TimeOnly?)).ShouldBe(true);

            _valueLiteralMaker.Literal(new TimeOnly(13,45,23)).ShouldBe(@"TimeOnly.Parse(""13:45:23"")");
        }

        [TestMethod]
        public void ValueLiteralMakerTimeOnlyLeadingZero()
        {
            _valueLiteralMaker.CanMake(typeof(TimeOnly)).ShouldBe(true);
            _valueLiteralMaker.CanMake(typeof(TimeOnly?)).ShouldBe(true);

            _valueLiteralMaker.Literal(new TimeOnly(0,1,2)).ShouldBe(@"TimeOnly.Parse(""00:01:02"")");
        }
        
        [TestMethod]
        public void ValueLiteralMakerTimeOnlyFractional()
        {
            _valueLiteralMaker.CanMake(typeof(TimeOnly)).ShouldBe(true);
            _valueLiteralMaker.CanMake(typeof(TimeOnly?)).ShouldBe(true);

            _valueLiteralMaker.Literal(new TimeOnly(new TimeSpan(0, 0, 58, 59, 123).Add(TimeSpan.FromTicks(4567)).Ticks)).ShouldBe(@"TimeOnly.Parse(""00:58:59.1234567"")");
        }

        [TestMethod]
        public void ValueLiteralMakerDateOnly()
        {
            _valueLiteralMaker.CanMake(typeof(DateOnly)).ShouldBe(true);
            _valueLiteralMaker.CanMake(typeof(DateOnly?)).ShouldBe(true);

            _valueLiteralMaker.Literal(DateOnly.Parse("2014-03-09")).ShouldBe(@"DateOnly.Parse(""2014-03-09"")");
        }
        #endif

        [TestMethod]
        public void ValueLiteralMakerDateTimeOffset()
        {
            _valueLiteralMaker.CanMake(typeof(DateTimeOffset)).ShouldBe(true);
            _valueLiteralMaker.CanMake(typeof(DateTimeOffset?)).ShouldBe(true);

            _valueLiteralMaker.Literal(DateTimeOffset.Parse("2014-03-09T15:59:23.1234567+01:00"))
                          .ShouldBe(@"DateTimeOffset.Parse(""2014-03-09T15:59:23.1234567+01:00"")");
        }

        [TestMethod]
        public void ValueLiteralMakerGuid()
        {
            _valueLiteralMaker.CanMake(typeof(Guid)).ShouldBe(true);
            _valueLiteralMaker.CanMake(typeof(Guid?)).ShouldBe(true);

            _valueLiteralMaker.Literal(
                Guid.Parse("b64bd5fb-25da-454e-85c0-1ecc54743bfe"))
                .ShouldBe(@"Guid.Parse(""b64bd5fb-25da-454e-85c0-1ecc54743bfe"")");
        }

        [TestMethod]
        public void ValueLiteralMakerSmallByteArray()
        {
            _valueLiteralMaker.CanMake(typeof(byte[])).ShouldBe(true);

            _valueLiteralMaker.Literal(new byte[] { 0x01, 0x02, 0x03 }).ShouldBe("new byte[] { 0x01, 0x02, 0x03, }");
        }

        [TestMethod]
        public void ValueLiteralMakerEmptyByteArray()
        {
            _valueLiteralMaker.Literal(new byte[] { }).ShouldBe("new byte[] { }");
        }
        [TestMethod]
        public void ValueLiteralMakerLongByteArray()
        {
            byte[] data = new byte[] {
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
                0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F,
                0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F,
                0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F,
                0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F,
                0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E, 0x5F,
                0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x6F,
                0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F
            };
            
            _valueLiteralMaker.Literal(data).ShouldBe(@"new byte[] {
0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F,
0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F,
0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F,
0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F,
0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E, 0x5F,
0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x6F,
0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F,
}",
            StringCompareShould.IgnoreLineEndings
                );
        }

        [TestMethod]
        public void ValueLiteralMakerStringArray()
        {
            _valueLiteralMaker.CanMake(typeof(string[])).ShouldBe(true);

            string[] data = new string[] {
                "Apple",
                "Carriage\r\nReturn",
                "\"Cucumber\""
            };

            _valueLiteralMaker.Literal(data).ShouldBe(@"new[] { ""Apple"", ""Carriage\r\nReturn"", @""""""Cucumber"""""" }",
            StringCompareShould.IgnoreLineEndings
            );
        }

        [TestMethod]
        public void ValueLiteralMakerStringArrayMultiLine()
        {
            // If the resulting text would be over 80 chars we split into multiple lines.
            string[] data = new string[] {
                "1234567890",
                "1234567890",
                "1234567890",
                "1234567890",
                "1234567890",
                "1234567890",
                "1234567890",
                "1234567890",
                "1234567890",
            };

            _valueLiteralMaker.Literal(data).ShouldBe(@"new[] {
""1234567890"",
""1234567890"",
""1234567890"",
""1234567890"",
""1234567890"",
""1234567890"",
""1234567890"",
""1234567890"",
""1234567890"",
}",
            StringCompareShould.IgnoreLineEndings
            );
        }

        [TestMethod]
        public void ValueLiteralMakerStringArrayEmpty()
        {
            string[] data = new string[] { };

            _valueLiteralMaker.Literal(data).ShouldBe(@"new string[0]",
            StringCompareShould.IgnoreLineEndings
            );
        }

        [TestMethod]
        public void ValueLiteralMakerStringArrayNull()
        {
            _valueLiteralMaker.CanMake(typeof(string[])).ShouldBe(true);

            string[] data = new string[] {
                "Apple",
                null,
            };

            _valueLiteralMaker.Literal(data).ShouldBe(@"new[] { ""Apple"", null }",
            StringCompareShould.IgnoreLineEndings
            );
        }

        [TestMethod]
        public void ValueLiteralMakerEnum()
        {
            var arg = GCCollectionMode.Forced;

            _valueLiteralMaker.Literal(arg).ShouldBe("System.GCCollectionMode.Forced");
        }

        [TestMethod]
        public void ValueLiteralMakerEnumNested()
        {
            var arg = Nested.NestedEnum.Default;

            _valueLiteralMaker.Literal(arg).ShouldBe("UnitTestCoder.Core.Tests.Literals.ValueLiteralMakerTests.Nested.NestedEnum.Default");
        }

        public partial class Nested
        {
            public enum NestedEnum
            {
                Default = 1
            }
        }

    }
}