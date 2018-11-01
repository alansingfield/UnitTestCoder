using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using UnitTestCoder.Core.Literal;

namespace Didsbury.Tests.CodeGen
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
            _valueLiteralMaker.Literal(12345).ShouldBe(@"12345");
        }
        [TestMethod]
        public void ValueLiteralMakerBoolean()
        {
            _valueLiteralMaker.Literal(false).ShouldBe(@"false");
        }
        [TestMethod]
        public void ValueLiteralMakerDouble()
        {
            _valueLiteralMaker.Literal(2.34d).ShouldBe(@"2.34d");
        }
        [TestMethod]
        public void ValueLiteralMakerTimeSpan()
        {
            _valueLiteralMaker.Literal(TimeSpan.Parse("13:45:23")).ShouldBe(@"TimeSpan.Parse(""13:45:23"")");
        }
        [TestMethod]
        public void ValueLiteralMakerDateTime()
        {
            _valueLiteralMaker.Literal(DateTime.Parse("2014-03-09T15:59:23.1234567")).ShouldBe(@"DateTime.Parse(""2014-03-09T15:59:23.1234567"")");
        }
        [TestMethod]
        public void ValueLiteralMakerSmallByteArray()
        {
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
        public void ValueLiteralMakerEnum()
        {
            var arg = GCCollectionMode.Forced;

            _valueLiteralMaker.Literal(arg).ShouldBe("System.GCCollectionMode.Forced");
        }

        [TestMethod]
        public void ValueLiteralMakerEnumNested()
        {
            var arg = Nested.NestedEnum.Default;

            _valueLiteralMaker.Literal(arg).ShouldBe("Didsbury.Tests.CodeGen.ValueLiteralMakerTests.Nested.NestedEnum.Default");
        }

        public class Nested
        {
            public enum NestedEnum
            {
                Default = 1
            }
        }
    }
}
