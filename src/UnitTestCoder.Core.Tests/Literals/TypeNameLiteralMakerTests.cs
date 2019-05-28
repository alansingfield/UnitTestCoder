using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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

        [TestMethod]
        public void TypeNameLiteralFullNormalClass()
        {
            _typeNameLiteralMaker
                .Literal(typeof(NormalClass), fullyQualify:true)
                .ShouldBe("UnitTestCoder.Core.Tests.Literals.NormalClass");
        }

        [TestMethod]
        public void TypeNameLiteralFullNestedClass()
        {
            _typeNameLiteralMaker
                .Literal(typeof(NormalClass.NestedClass), fullyQualify: true)
                .ShouldBe("UnitTestCoder.Core.Tests.Literals.NormalClass.NestedClass");
        }

        [TestMethod]
        public void TypeNameLiteralFullNestedClassInList()
        {
            _typeNameLiteralMaker
                .Literal(typeof(List<NormalClass.NestedClass>), fullyQualify: true)
                .ShouldBe("System.Collections.Generic.List<UnitTestCoder.Core.Tests.Literals.NormalClass.NestedClass>");
        }


        [TestMethod]
        public void TypeNameLiteralCannotMakeAnonymousType()
        {
            var p = new { A = 123 };

            _typeNameLiteralMaker
                .CanMake(p.GetType())
                .ShouldBe(false);
        }

        [TestMethod]
        public void TypeNameLiteralCannotMakePrivateNestedType()
        {
            var x = typeof(PrivateClass);

            _typeNameLiteralMaker
                .CanMake(x)
                .ShouldBe(false);
        }

        private class PrivateClass
        {
            public int B { get; set; }
        }


        [TestMethod]
        public void TypeNameLiteralCannotMakeExternalPrivateType()
        {
            var corelib = Assembly.GetAssembly(typeof(string));

            // Find a private, non-anonymous type somewhere in mscorlib.
            // (Interop is one)
            Type privateType = corelib.DefinedTypes.First(
                x => x.IsNotPublic
                && !x.GetCustomAttributes<CompilerGeneratedAttribute>().Any());

            _typeNameLiteralMaker
                .CanMake(privateType)
                .ShouldBe(false);
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
