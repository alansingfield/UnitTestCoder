using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using UnitTestCoder.Core.Literal;

namespace UnitTestCoder.Core.Tests.Literals
{
    [TestClass]
    public class TypeNameLiteralMakerTests
    {
        private ITypeNameLiteralMaker _typeNameLiteralMaker;

        [TestInitialize]
        public void Init()
        {
            _typeNameLiteralMaker = new TypeNameLiteralMaker();
        }

        [TestMethod]
        public void TypeNameLiteralNormalClass()
        {
            _typeNameLiteralMaker
                .Literal(typeof(NormalClass))
                .ShouldBe("NormalClass");
        }

        [TestMethod]
        public void TypeNameLiteralNestedClass()
        {
            _typeNameLiteralMaker
                .Literal(typeof(NormalClass.NestedClass))
                .ShouldBe("NormalClass.NestedClass");
        }

        [TestMethod]
        public void TypeNameLiteralNestedClassInList()
        {
            _typeNameLiteralMaker
                .Literal(typeof(List<NormalClass.NestedClass>))
                .ShouldBe("List<NormalClass.NestedClass>");
        }
    }

    public class NormalClass
    {
        public int Field { get; set; }

        public class NestedClass
        {
            public int AnotherField { get; set; }
        }
    }

}
