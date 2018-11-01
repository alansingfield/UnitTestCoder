using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using UnitTestCoder.Core.Formatting;
using UnitTestCoder.Core.Literal;

namespace UnitTestCoder.Core.Tests.Literals
{
    [TestClass]
    public class ObjectLiteralMakerTests
    {
        private IObjectLiteralMaker _objectLiteralMaker;

        [TestInitialize]
        public void Init()
        {
            _objectLiteralMaker = new ObjectLiteralMaker(
                new ValueLiteralMaker(),
                new TypeNameLiteralMaker(),
                new Indenter());
        }

        [TestMethod]
        public void ObjectLiteralMakerSimpleObject()
        {
            var result = makeObjectLiteral(new SimpleObject()
            {
                A = 4,
                B = 5
            });

            result.ShouldBe(normalise(@"new SimpleObject() { A = 4, B = 5, }"));
        }

        [TestMethod]
        public void ObjectLiteralMakerArrayOfSimpleObject()
        {
            var result = makeObjectLiteral(new[] {
                new SimpleObject()
                {
                    A = 1,
                    B = 2
                },
                    new SimpleObject()
                {
                    A = 3,
                    B = 4
                },
            });

            result.ShouldBe(normalise(@"new SimpleObject[] {
                new SimpleObject() { A = 1, B = 2, },
                new SimpleObject() { A = 3, B = 4, },
            }"));
        }

        [TestMethod]
        public void ObjectLiteralMakerListOfSimpleObject()
        {
            var result = makeObjectLiteral(new List<SimpleObject>() {
                new SimpleObject() { A = 4, B = 5, },
            });

            result.ShouldBe(normalise(@"new List<SimpleObject>() {
                new SimpleObject() { A = 4, B = 5, },
                }"));
        }

        [TestMethod]
        public void ObjectLiteralMakerObjectProperty()
        {
            var result = makeObjectLiteral(new ContainingObject() {
                Object1 = new SimpleObject() { A = 4, B = 5, },
            });

            result.ShouldBe(normalise(
                @"new ContainingObject() {
                    Object1 = new SimpleObject() { A = 4, B = 5, }, }"));
        }

        [TestMethod]
        public void ObjectLiteralMakerNormalise()
        {
            normalise("A  B").ShouldBe("A B");
            normalise("A\r\nB").ShouldBe("A B");
            normalise(" A\r\nB ").ShouldBe("A B");
            normalise(" A\r\nB \r\n").ShouldBe("A B");
            normalise("\r A\r\nB \r\n").ShouldBe("A B");
        }

        [TestMethod]
        public void ObjectLiteralMakerInterface()
        {
            IInterfaceAB ab = new ObjectABC()
            {
                A = 1,
                B = 2,
                C = 3
            };

            var result = makeObjectLiteral(ab);

            // Should use the actual instance type not the property type.
            result.ShouldBe(normalise(@"new ObjectABC() { A = 1, B = 2, C = 3, }"));
        }

        private string makeObjectLiteral(object arg)
        {
            return normalise(_objectLiteralMaker.MakeObjectLiteral(arg));
        }

        private static Regex _normaliseRegex = new Regex(@"\s+");

        private string normalise(string arg)
        {
            return _normaliseRegex.Replace(arg, " ").Trim();
        }


    }
    public class SimpleObject
    {
        public int A { get; set; }
        public int B { get; set; }
    }
    public class ObjectABC : IInterfaceAB
    {
        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }
    }

    public interface IInterfaceAB
    {
        int A { get; set; }
        int B { get; set; }
    }

    public class ContainingObject
    {
        public SimpleObject Object1 { get; set; }
    }

}
