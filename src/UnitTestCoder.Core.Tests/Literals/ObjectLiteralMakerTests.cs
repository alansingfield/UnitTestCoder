using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
            var indenter = new Indenter();
            var valueLiteralMaker = new ValueLiteralMaker();
            var typeNameLiteralMaker = new TypeNameLiteralMaker();
            var typeLiteralMaker = new TypeLiteralMaker(typeNameLiteralMaker);

            _objectLiteralMaker = new ObjectLiteralMaker(
                valueLiteralMaker,
                typeNameLiteralMaker,
                typeLiteralMaker,
                indenter);
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
        public void ObjectLiteralMakerStringArray()
        {
            var result = makeObjectLiteral(new string[]
                {
                    "A",
                    "B",
                });

            result.ShouldBe(normalise(@"new[] { ""A"", ""B"" }"));
        }

        [TestMethod]
        public void ObjectLiteralMakerStringList()
        {
            var result = makeObjectLiteral(new List<string>()
                {
                    "A",
                    "B",
                });

            result.ShouldBe(normalise(@"new List<String>() { ""A"", ""B"", }"));
        }

        [TestMethod]
        public void ObjectLiteralMakerEmptyString()
        {
            var result = makeObjectLiteral(new string[]
                {});

            result.ShouldBe(normalise(@"new string[0]"));
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


        [TestMethod]
        public void ObjectLiteralMakerCircularRefSkip()
        {
            var x = new CircularRefObject() { A = 10 };

            var result = makeObjectLiteral(x, noFollowFunc: y => y.Name == nameof(CircularRefObject.BadRef));
            result.ShouldBe("new CircularRefObject() { A = 10, }");
        }

        [TestMethod]
        public void ObjectLiteralMakerCircularRefSelf()
        {
            var x = new CircularRefObject();
            x.BadRef = x;

            Should.Throw(() =>
            {
                makeObjectLiteral(x);
            }, typeof(Exception)).Message.ShouldBe("Circular reference detected for object of type 'CircularRefObject'");
        }

        [TestMethod]
        public void ObjectLiteralMakerCircularRefOther()
        {
            var x = new CircularRefObject();
            var y = new CircularRefObject();
            x.BadRef = y;
            y.BadRef = x;

            Should.Throw(() =>
            {
                makeObjectLiteral(x);
            }, typeof(Exception)).Message.ShouldBe("Circular reference detected for object of type 'CircularRefObject'");
        }

        [TestMethod]
        public void ObjectLiteralMakerSkipReadOnly()
        {
            var pq = new PqObject() { P = 20 };

            pq.P.ShouldBe(20);
            pq.Q.ShouldBe(21);

            var result = makeObjectLiteral(pq);
            result.ShouldBe("new PqObject() { P = 20, }");
        }

        [TestMethod]
        public void ObjectLiteralMakerNoFollow()
        {
            var m = new ObjectABC() { A = 1, B = 2, C = 3 };

            var result = makeObjectLiteral(m, noFollowFunc: x => x.Name == "B" && x.DeclaringType == typeof(ObjectABC));

            result.ShouldBe("new ObjectABC() { A = 1, C = 3, }");
        }

        [TestMethod]
        public void ObjectLiteralMakerTypeof()
        {
            var m = typeof(string);

            var result = makeObjectLiteral(m);
            result.ShouldBe("typeof(System.String)");
        }

        [TestMethod]
        public void ObjectLiteralMakerTypeofObject()
        {
            var pq = new TypeofObject() { T = typeof(string) };

            var result = makeObjectLiteral(pq);
            result.ShouldBe("new TypeofObject() { T = typeof(System.String), }");
        }

        [TestMethod]
        public void ObjectLiteralMakerTypeofInvalid()
        {
            var corelib = Assembly.GetAssembly(typeof(string));

            // Find a private, non-anonymous type somewhere in mscorlib.
            // This should generate a commented-out typename.
            Type privateType = corelib.DefinedTypes.First(
                x => x.IsNotPublic
                && !x.GetCustomAttributes<CompilerGeneratedAttribute>().Any()
                && x.Namespace != null);

            var pq = new TypeofObject() { T = privateType };

            var result = makeObjectLiteral(pq);
            result.ShouldBe("new TypeofObject() { T = typeof(/*FxResources.System.Private.CoreLib.SR*/), }");
        }

        private string makeObjectLiteral(object arg, Func<PropertyInfo, bool> noFollowFunc = null)
        {
            return normalise(_objectLiteralMaker.MakeObjectLiteral(arg, noFollowFunc));
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

    public class CircularRefObject
    {
        public int A { get; set; }
        public CircularRefObject BadRef { get; set; }
    }

    public class PqObject
    {
        public int P { get; set; }
        public int Q => P + 1;
    }

    public class TypeofObject
    {
        public Type T { get; set; }
    }
}
