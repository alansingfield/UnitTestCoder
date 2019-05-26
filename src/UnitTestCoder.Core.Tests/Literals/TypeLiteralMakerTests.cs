using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using UnitTestCoder.Core.Literal;

namespace UnitTestCoder.Core.Tests.Literals
{
    [TestClass]
    public class TypeLiteralMakerTests
    {
        private TypeLiteralMaker _typeLiteralMaker;

        [TestInitialize]
        public void Init()
        {
            _typeLiteralMaker = new TypeLiteralMaker(new TypeNameLiteralMaker());
        }

        [TestMethod]
        public void TypeLiteralMakerType()
        {
            var arg = typeof(string);

            _typeLiteralMaker.Literal(arg).ShouldBe("typeof(String)");
        }

        [TestMethod]
        public void ValueLiteralMakerSubType()
        {
            var arg = typeof(System.Globalization.Calendar);

            _typeLiteralMaker.Literal(arg).ShouldBe("typeof(Calendar)");
        }

        [TestMethod]
        public void ValueLiteralMakerNestedType()
        {
            var arg = typeof(Nested.Subclass);

            _typeLiteralMaker.Literal(arg).ShouldBe("typeof(ValueLiteralMakerTests.Nested.Subclass)");
        }

        public partial class Nested
        {
            public class Subclass
            {
            }
        }
    }
}
