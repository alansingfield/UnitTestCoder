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

            _typeLiteralMaker.Literal(arg).ShouldBe("typeof(string)");
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
            var arg = typeof(UnitTestCoder.Core.Tests.Literals.TypeLiteralMakerTests.Nested.GenericSub<>);

            _typeLiteralMaker.Literal(arg).ShouldBe(
                "typeof(UnitTestCoder.Core.Tests.Literals.TypeLiteralMakerTests.Nested.GenericSub<>)");
        }

        [TestMethod]
        public void TypeLiteralMakerNestedSubWithinGeneric()
        {
            var arg = typeof(UnitTestCoder.Core.Tests.Literals.TypeLiteralMakerTests.Nested.GenericSub<>.SubWithinGeneric);

            _typeLiteralMaker.Literal(arg).ShouldBe(
                "typeof(UnitTestCoder.Core.Tests.Literals.TypeLiteralMakerTests.Nested.GenericSub<>.SubWithinGeneric)");
        }

        [TestMethod]
        public void TypeLiteralMakerNestedGenericWithinGeneric()
        {
            var arg = typeof(UnitTestCoder.Core.Tests.Literals.TypeLiteralMakerTests.Nested.GenericSub<>.GenericWithinGeneric<>);

            _typeLiteralMaker.Literal(arg).ShouldBe(
                "typeof(UnitTestCoder.Core.Tests.Literals.TypeLiteralMakerTests.Nested.GenericSub<>.GenericWithinGeneric<>)");
        }


        [TestMethod]
        public void TypeLiteralMakerNestedConstructedGenericWithinGeneric()
        {
            var arg = typeof(UnitTestCoder.Core.Tests.Literals.TypeLiteralMakerTests.Nested.GenericSub<int>.GenericWithinGeneric<string>);

            _typeLiteralMaker.Literal(arg).ShouldBe(
                "typeof(UnitTestCoder.Core.Tests.Literals.TypeLiteralMakerTests.Nested.GenericSub<int>.GenericWithinGeneric<string>)");
        }

        [TestMethod]
        public void TypeLiteralMakerNestedConstructedWithSelf()
        {
            var arg = typeof(UnitTestCoder.Core.Tests.Literals.TypeLiteralMakerTests.Nested.GenericSub<UnitTestCoder.Core.Tests.Literals.TypeLiteralMakerTests.Nested.Subclass>.GenericWithinGeneric<string>);

            _typeLiteralMaker.Literal(arg).ShouldBe(
                "typeof(UnitTestCoder.Core.Tests.Literals.TypeLiteralMakerTests.Nested.GenericSub<UnitTestCoder.Core.Tests.Literals.TypeLiteralMakerTests.Nested.Subclass>.GenericWithinGeneric<string>)");
        }


        public partial class Nested
        {
            public class Subclass
            {
            }

            public class GenericSub<T>
            {
                public class SubWithinGeneric { }

                public class GenericWithinGeneric<T2> { }
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

            _typeLiteralMaker.Literal(arg).ShouldBe("typeof(System.Collections.Generic.List<string>)");
        }
    }
}
