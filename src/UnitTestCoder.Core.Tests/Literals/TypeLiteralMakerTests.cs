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

            _typeLiteralMaker.Literal(arg).ShouldBe("typeof(System.String)");
        }

        [TestMethod]
        public void TypeLiteralMakerSubType()
        {
            var arg = typeof(System.Globalization.Calendar);

            _typeLiteralMaker.Literal(arg).ShouldBe("typeof(System.Globalization.Calendar)");
        }

        [TestMethod]
        public void TypeLiteralMakerNestedType()
        {
            var arg = typeof(Nested.Subclass);

            _typeLiteralMaker.Literal(arg).ShouldBe("typeof(UnitTestCoder.Core.Tests.Literals.TypeLiteralMakerTests.Nested.Subclass)");
        }

        [TestMethod]
        public void TypeLiteralMakerNestedGenericType()
        {
            var arg = typeof(Nested.GenericSub<>);

            _typeLiteralMaker.Literal(arg).ShouldBe("typeof(UnitTestCoder.Core.Tests.Literals.TypeLiteralMakerTests.Nested.GenericSub<>)");
        }


        public partial class Nested
        {
            public class Subclass
            {
            }

            public class GenericSub<T>
            {
            }
        }

        [TestMethod]
        public void TypeLiteralMakerGenericOpenConstructed()
        {
            var arg = typeof(IDictionary<,>);

            _typeLiteralMaker.Literal(arg).ShouldBe("typeof(System.Collections.Generic.IDictionary<,>)");
        }

        [TestMethod]
        public void TypeLiteralMakerGenericClosedConstructed()
        {
            var arg = typeof(List<string>);

            _typeLiteralMaker.Literal(arg).ShouldBe("typeof(System.Collections.Generic.List<System.String>)");
        }
    }
}
