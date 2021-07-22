using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using UnitTestCoder.Core.Literal;
using Shouldly;

namespace UnitTestCoder.Core.Tests.Literals
{
    [TestClass]
    public class ValueLiteralMakerEnumTests
    {
        private IValueLiteralMaker _valueLiteralMaker;

        [TestInitialize]
        public void Init()
        {
            _valueLiteralMaker = new ValueLiteralMaker();
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

            _valueLiteralMaker.Literal(arg).ShouldBe("UnitTestCoder.Core.Tests.Literals.ValueLiteralMakerEnumTests.Nested.NestedEnum.Default");
        }

        public partial class Nested
        {
            public enum NestedEnum
            {
                Default = 1
            }
        }

        private string run(object arg)
        {
            string literal = _valueLiteralMaker.Literal(arg);

            if(!literal.Contains("UnitTestCoder.Core.Tests.Literals."))
                throw new Exception("Expected namespace in text");

            // Clear the namespace out of the text to make test results more readable.
            return literal.Replace("UnitTestCoder.Core.Tests.Literals.", "");
        }


        [TestMethod]
        public void ValueLiteralMakerFlagsI32Standard()
        {
            run(FlagsI32Standard.None).ShouldBe("FlagsI32Standard.None");
            run(FlagsI32Standard.First).ShouldBe("FlagsI32Standard.First");
            run(FlagsI32Standard.Last).ShouldBe("FlagsI32Standard.Last");
            run(FlagsI32Standard.BC).ShouldBe("FlagsI32Standard.BC");
            run(FlagsI32Standard.First | FlagsI32Standard.Last).ShouldBe("FlagsI32Standard.First | FlagsI32Standard.Last");
            run((FlagsI32Standard)8).ShouldBe("(FlagsI32Standard)8");
            run((FlagsI32Standard)(-8)).ShouldBe("(FlagsI32Standard)(-8)");
        }

        [TestMethod]
        public void ValueLiteralMakerFlagsI32Quirk()
        {
            run((FlagsI32Quirk)0).ShouldBe("(FlagsI32Quirk)0");
            run(FlagsI32Quirk.First).ShouldBe("FlagsI32Quirk.First");
            run(FlagsI32Quirk.Last).ShouldBe("FlagsI32Quirk.Last");
            run(FlagsI32Quirk.BDuplicate).ShouldBe("FlagsI32Quirk.B");
            run(FlagsI32Quirk.BDuplicate | FlagsI32Quirk.First).ShouldBe("FlagsI32Quirk.First | FlagsI32Quirk.B");
            run(FlagsI32Quirk.First | FlagsI32Quirk.Last).ShouldBe("FlagsI32Quirk.First | FlagsI32Quirk.Last");
            run((FlagsI32Quirk)8).ShouldBe("(FlagsI32Quirk)8");
            run((FlagsI32Quirk)(-8)).ShouldBe("(FlagsI32Quirk)(-8)");
        }



        [TestMethod]
        public void ValueLiteralMakerFlagsU32Standard()
        {
            run(FlagsU32Standard.None).ShouldBe("FlagsU32Standard.None");
            run(FlagsU32Standard.First).ShouldBe("FlagsU32Standard.First");
            run(FlagsU32Standard.Last).ShouldBe("FlagsU32Standard.Last");
            run(FlagsU32Standard.BC).ShouldBe("FlagsU32Standard.BC");
            run(FlagsU32Standard.First | FlagsU32Standard.Last).ShouldBe("FlagsU32Standard.First | FlagsU32Standard.Last");
            run((FlagsU32Standard)8u).ShouldBe("(FlagsU32Standard)8u");
        }

        [TestMethod]
        public void ValueLiteralMakerFlagsU32Quirk()
        {
            run((FlagsU32Quirk)0u).ShouldBe("(FlagsU32Quirk)0u");
            run(FlagsU32Quirk.First).ShouldBe("FlagsU32Quirk.First");
            run(FlagsU32Quirk.Last).ShouldBe("FlagsU32Quirk.Last");
            run(FlagsU32Quirk.BDuplicate).ShouldBe("FlagsU32Quirk.B");
            run(FlagsU32Quirk.BDuplicate | FlagsU32Quirk.First).ShouldBe("FlagsU32Quirk.First | FlagsU32Quirk.B");
            run(FlagsU32Quirk.First | FlagsU32Quirk.Last).ShouldBe("FlagsU32Quirk.First | FlagsU32Quirk.Last");
            run((FlagsU32Quirk)8u).ShouldBe("(FlagsU32Quirk)8u");
        }

        [TestMethod]
        public void ValueLiteralMakerFlagsI64Standard()
        {
            run(FlagsI64Standard.None).ShouldBe("FlagsI64Standard.None");
            run(FlagsI64Standard.First).ShouldBe("FlagsI64Standard.First");
            run(FlagsI64Standard.Last).ShouldBe("FlagsI64Standard.Last");
            run(FlagsI64Standard.BC).ShouldBe("FlagsI64Standard.BC");
            run(FlagsI64Standard.First | FlagsI64Standard.Last).ShouldBe("FlagsI64Standard.First | FlagsI64Standard.Last");
            run((FlagsI64Standard)8L).ShouldBe("(FlagsI64Standard)8L");
            run((FlagsI64Standard)(-8L)).ShouldBe("(FlagsI64Standard)(-8L)");
        }

        [TestMethod]
        public void ValueLiteralMakerFlagsI64Quirk()
        {
            run((FlagsI64Quirk)0L).ShouldBe("(FlagsI64Quirk)0L");
            run(FlagsI64Quirk.First).ShouldBe("FlagsI64Quirk.First");
            run(FlagsI64Quirk.Last).ShouldBe("FlagsI64Quirk.Last");
            run(FlagsI64Quirk.BDuplicate).ShouldBe("FlagsI64Quirk.B");
            run(FlagsI64Quirk.BDuplicate | FlagsI64Quirk.First).ShouldBe("FlagsI64Quirk.First | FlagsI64Quirk.B");
            run(FlagsI64Quirk.First | FlagsI64Quirk.Last).ShouldBe("FlagsI64Quirk.First | FlagsI64Quirk.Last");
            run((FlagsI64Quirk)8L).ShouldBe("(FlagsI64Quirk)8L");
            run((FlagsI64Quirk)(-8L)).ShouldBe("(FlagsI64Quirk)(-8L)");
        }

        [TestMethod]
        public void ValueLiteralMakerFlagsU64Standard()
        {
            run(FlagsU64Standard.None).ShouldBe("FlagsU64Standard.None");
            run(FlagsU64Standard.First).ShouldBe("FlagsU64Standard.First");
            run(FlagsU64Standard.Last).ShouldBe("FlagsU64Standard.Last");
            run(FlagsU64Standard.BC).ShouldBe("FlagsU64Standard.BC");
            run(FlagsU64Standard.First | FlagsU64Standard.Last).ShouldBe("FlagsU64Standard.First | FlagsU64Standard.Last");
            run((FlagsU64Standard)8ul).ShouldBe("(FlagsU64Standard)8ul");
        }

        [TestMethod]
        public void ValueLiteralMakerFlagsU64Quirk()
        {
            run((FlagsU64Quirk)0ul).ShouldBe("(FlagsU64Quirk)0ul");
            run(FlagsU64Quirk.First).ShouldBe("FlagsU64Quirk.First");
            run(FlagsU64Quirk.Last).ShouldBe("FlagsU64Quirk.Last");
            run(FlagsU64Quirk.BDuplicate).ShouldBe("FlagsU64Quirk.B");
            run(FlagsU64Quirk.BDuplicate | FlagsU64Quirk.First).ShouldBe("FlagsU64Quirk.First | FlagsU64Quirk.B");
            run(FlagsU64Quirk.First | FlagsU64Quirk.Last).ShouldBe("FlagsU64Quirk.First | FlagsU64Quirk.Last");
            run((FlagsU64Quirk)8ul).ShouldBe("(FlagsU64Quirk)8ul");
        }

        [TestMethod]
        public void ValueLiteralMakerFlagsI16Standard()
        {
            run(FlagsI16Standard.None).ShouldBe("FlagsI16Standard.None");
            run(FlagsI16Standard.First).ShouldBe("FlagsI16Standard.First");
            run(FlagsI16Standard.Last).ShouldBe("FlagsI16Standard.Last");
            run(FlagsI16Standard.BC).ShouldBe("FlagsI16Standard.BC");
            run(FlagsI16Standard.First | FlagsI16Standard.Last).ShouldBe("FlagsI16Standard.First | FlagsI16Standard.Last");
            run((FlagsI16Standard)8).ShouldBe("(FlagsI16Standard)8");
            run((FlagsI16Standard)(-8)).ShouldBe("(FlagsI16Standard)(-8)");
        }

        [TestMethod]
        public void ValueLiteralMakerFlagsI16Quirk()
        {
            run((FlagsI16Quirk)0).ShouldBe("(FlagsI16Quirk)0");
            run(FlagsI16Quirk.First).ShouldBe("FlagsI16Quirk.First");
            run(FlagsI16Quirk.Last).ShouldBe("FlagsI16Quirk.Last");
            run(FlagsI16Quirk.BDuplicate).ShouldBe("FlagsI16Quirk.B");
            run(FlagsI16Quirk.BDuplicate | FlagsI16Quirk.First).ShouldBe("FlagsI16Quirk.First | FlagsI16Quirk.B");
            run(FlagsI16Quirk.First | FlagsI16Quirk.Last).ShouldBe("FlagsI16Quirk.First | FlagsI16Quirk.Last");
            run((FlagsI16Quirk)8).ShouldBe("(FlagsI16Quirk)8");
            run((FlagsI16Quirk)(-8)).ShouldBe("(FlagsI16Quirk)(-8)");
        }

        [TestMethod]
        public void ValueLiteralMakerFlagsU16Standard()
        {
            run(FlagsU16Standard.None).ShouldBe("FlagsU16Standard.None");
            run(FlagsU16Standard.First).ShouldBe("FlagsU16Standard.First");
            run(FlagsU16Standard.Last).ShouldBe("FlagsU16Standard.Last");
            run(FlagsU16Standard.BC).ShouldBe("FlagsU16Standard.BC");
            run(FlagsU16Standard.First | FlagsU16Standard.Last).ShouldBe("FlagsU16Standard.First | FlagsU16Standard.Last");
            run((FlagsU16Standard)8).ShouldBe("(FlagsU16Standard)8");
        }

        [TestMethod]
        public void ValueLiteralMakerFlagsU16Quirk()
        {
            run((FlagsU16Quirk)0).ShouldBe("(FlagsU16Quirk)0");
            run(FlagsU16Quirk.First).ShouldBe("FlagsU16Quirk.First");
            run(FlagsU16Quirk.Last).ShouldBe("FlagsU16Quirk.Last");
            run(FlagsU16Quirk.BDuplicate).ShouldBe("FlagsU16Quirk.B");
            run(FlagsU16Quirk.BDuplicate | FlagsU16Quirk.First).ShouldBe("FlagsU16Quirk.First | FlagsU16Quirk.B");
            run(FlagsU16Quirk.First | FlagsU16Quirk.Last).ShouldBe("FlagsU16Quirk.First | FlagsU16Quirk.Last");
            run((FlagsU16Quirk)8).ShouldBe("(FlagsU16Quirk)8");
        }

        [TestMethod]
        public void ValueLiteralMakerFlagsS8Standard()
        {
            run(FlagsS8Standard.None).ShouldBe("FlagsS8Standard.None");
            run(FlagsS8Standard.First).ShouldBe("FlagsS8Standard.First");
            run(FlagsS8Standard.Last).ShouldBe("FlagsS8Standard.Last");
            run(FlagsS8Standard.BC).ShouldBe("FlagsS8Standard.BC");
            run(FlagsS8Standard.First | FlagsS8Standard.Last).ShouldBe("FlagsS8Standard.First | FlagsS8Standard.Last");
            run((FlagsS8Standard)8).ShouldBe("(FlagsS8Standard)8");
            run((FlagsS8Standard)(-8)).ShouldBe("(FlagsS8Standard)(-8)");
        }

        [TestMethod]
        public void ValueLiteralMakerFlagsS8Quirk()
        {
            run((FlagsS8Quirk)0).ShouldBe("(FlagsS8Quirk)0");
            run(FlagsS8Quirk.First).ShouldBe("FlagsS8Quirk.First");
            run(FlagsS8Quirk.Last).ShouldBe("FlagsS8Quirk.Last");
            run(FlagsS8Quirk.BDuplicate).ShouldBe("FlagsS8Quirk.B");
            run(FlagsS8Quirk.BDuplicate | FlagsS8Quirk.First).ShouldBe("FlagsS8Quirk.First | FlagsS8Quirk.B");
            run(FlagsS8Quirk.First | FlagsS8Quirk.Last).ShouldBe("FlagsS8Quirk.First | FlagsS8Quirk.Last");
            run((FlagsS8Quirk)8).ShouldBe("(FlagsS8Quirk)8");
            run((FlagsS8Quirk)(-8)).ShouldBe("(FlagsS8Quirk)(-8)");
        }

        [TestMethod]
        public void ValueLiteralMakerFlagsU8Standard()
        {
            run(FlagsU8Standard.None).ShouldBe("FlagsU8Standard.None");
            run(FlagsU8Standard.First).ShouldBe("FlagsU8Standard.First");
            run(FlagsU8Standard.Last).ShouldBe("FlagsU8Standard.Last");
            run(FlagsU8Standard.BC).ShouldBe("FlagsU8Standard.BC");
            run(FlagsU8Standard.First | FlagsU8Standard.Last).ShouldBe("FlagsU8Standard.First | FlagsU8Standard.Last");
            run((FlagsU8Standard)8).ShouldBe("(FlagsU8Standard)8");
        }

        [TestMethod]
        public void ValueLiteralMakerFlagsU8Quirk()
        {
            run((FlagsU8Quirk)0).ShouldBe("(FlagsU8Quirk)0");
            run(FlagsU8Quirk.First).ShouldBe("FlagsU8Quirk.First");
            run(FlagsU8Quirk.Last).ShouldBe("FlagsU8Quirk.Last");
            run(FlagsU8Quirk.BDuplicate).ShouldBe("FlagsU8Quirk.B");
            run(FlagsU8Quirk.BDuplicate | FlagsU8Quirk.First).ShouldBe("FlagsU8Quirk.First | FlagsU8Quirk.B");
            run(FlagsU8Quirk.First | FlagsU8Quirk.Last).ShouldBe("FlagsU8Quirk.First | FlagsU8Quirk.Last");
            run((FlagsU8Quirk)8).ShouldBe("(FlagsU8Quirk)8");
        }
    }

    [Flags]
    public enum FlagsI32Standard : int
    {
        None = 0,
        First = 1,
        B = 2,
        C = 4,
        BC = 6,
        Last = int.MinValue,
    }

    [Flags]
    public enum FlagsI32Quirk : int
    {
        // No None entry
        First = 1,
        B = 2,
        C = 4,
        BC = 6,
        BDuplicate = 2,   // Reuse same number
        NoBitField = 10,
        Last = int.MinValue,
    }


    [Flags]
    public enum FlagsU32Standard : uint
    {
        None = 0u,
        First = 1u,
        B = 2u,
        C = 4u,
        BC = 6u,
        Last = 0x8000_0000,
    }

    [Flags]
    public enum FlagsU32Quirk : uint
    {
        // No None entry
        First = 1u,
        B = 2u,
        C = 4u,
        BC = 6u,
        BDuplicate = 2u,   // Reuse same number
        NoBitField = 10u,
        Last = 0x8000_0000,
    }


    [Flags]
    public enum FlagsI64Standard : long
    {
        None = 0L,
        First = 1L,
        B = 2L,
        C = 4L,
        BC = 6L,
        Last = long.MinValue,
    }

    [Flags]
    public enum FlagsI64Quirk : long
    {
        // No None entry
        First = 1L,
        B = 2L,
        C = 4L,
        BC = 6L,
        BDuplicate = 2L,   // Reuse same number
        NoBitField = 10L,
        Last = long.MinValue,
    }


    [Flags]
    public enum FlagsU64Standard : ulong
    {
        None = 0ul,
        First = 1ul,
        B = 2ul,
        C = 4ul,
        BC = 6ul,
        Last = 0x8000_0000_0000_0000,
    }

    [Flags]
    public enum FlagsU64Quirk : ulong
    {
        // No None entry
        First = 1ul,
        B = 2ul,
        C = 4ul,
        BC = 6ul,
        BDuplicate = 2ul,   // Reuse same number
        NoBitField = 10ul,
        Last = 0x8000_0000_0000_0000,
    }


    [Flags]
    public enum FlagsI16Standard : short
    {
        None = 0,
        First = 1,
        B = 2,
        C = 4,
        BC = 6,
        Last = short.MinValue,
    }

    [Flags]
    public enum FlagsI16Quirk : short
    {
        // No None entry
        First = 1,
        B = 2,
        C = 4,
        BC = 6,
        BDuplicate = 2,   // Reuse same number
        NoBitField = 10,
        Last = short.MinValue,
    }


    [Flags]
    public enum FlagsU16Standard : ushort
    {
        None = 0,
        First = 1,
        B = 2,
        C = 4,
        BC = 6,
        Last = 0x8000,
    }

    [Flags]
    public enum FlagsU16Quirk : ushort
    {
        // No None entry
        First = 1,
        B = 2,
        C = 4,
        BC = 6,
        BDuplicate = 2,   // Reuse same number
        NoBitField = 10,
        Last = 0x8000,
    }


    [Flags]
    public enum FlagsS8Standard : sbyte
    {
        None = 0,
        First = 1,
        B = 2,
        C = 4,
        BC = 6,
        Last = sbyte.MinValue,
    }

    [Flags]
    public enum FlagsS8Quirk : sbyte
    {
        // No None entry
        First = 1,
        B = 2,
        C = 4,
        BC = 6,
        BDuplicate = 2,   // Reuse same number
        NoBitField = 10,
        Last = sbyte.MinValue,
    }


    [Flags]
    public enum FlagsU8Standard : byte
    {
        None = 0,
        First = 1,
        B = 2,
        C = 4,
        BC = 6,
        Last = 0x80,
    }

    [Flags]
    public enum FlagsU8Quirk : byte
    {
        // No None entry
        First = 1,
        B = 2,
        C = 4,
        BC = 6,
        BDuplicate = 2,   // Reuse same number
        NoBitField = 10,
        Last = 0x80,
    }
}
