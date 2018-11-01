using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using UnitTestCoder.Core.Decomposer;
using UnitTestCoder.Core.Literal;
using UnitTestCoder.Shouldly.Coder;
using Shouldly;
using System.Linq;

namespace UnitTestCoder.Shouldly.Tests.Coder
{
    [TestClass]
    public class ShouldlyTestCoderTests
    {
        private ShouldlyTestCoder _shouldlyTestCoder;

        [TestInitialize]
        public void Init()
        {
            _shouldlyTestCoder = new ShouldlyTestCoder(
                new ObjectDecomposer(
                    new ValueLiteralMaker()),
                new ValueLiteralMaker());
        }

        [TestMethod]
        public void ShouldlyTestCoderStringNoQuote()
        {
            var x = _shouldlyTestCoder.GenerateShouldBes("myObj", "hello");
            x.ShouldHaveSingleItem();
            x.Single().ShouldBe(@"myObj.ShouldBe(""hello"");");
        }

        [TestMethod]
        public void ShouldlyTestCoderStringQuote()
        {
            var x = _shouldlyTestCoder.GenerateShouldBes("myObj", @"with""a quote");
            x.ShouldHaveSingleItem();
            x.Single().ShouldBe(@"myObj.ShouldBe(@""with""""a quote"");");
        }

        [TestMethod]
        public void ShouldlyTestCoderStringBackslash()
        {
            var x = _shouldlyTestCoder.GenerateShouldBes("myObj", @"back\slash");
            x.ShouldHaveSingleItem();
            x.Single().ShouldBe(@"myObj.ShouldBe(@""back\slash"");");
        }

        [TestMethod]
        public void ShouldlyTestCoderIntLiteral()
        {
            var x = _shouldlyTestCoder.GenerateShouldBes("myObj", 456);
            x.ShouldHaveSingleItem();
            x.Single().ShouldBe(@"myObj.ShouldBe(456);");
        }

        [TestMethod]
        public void ShouldlyTestCoderNull()
        {
            object myObj = null;
            var x = _shouldlyTestCoder.GenerateShouldBes("myObj", myObj);
            x.ShouldHaveSingleItem();
            x.Single().ShouldBe(@"myObj.ShouldBe(null);");
        }


        [TestMethod]
        public void ShouldlyTestCoderDecimalLiteral()
        {
            var x = _shouldlyTestCoder.GenerateShouldBes("myObj", 123m);
            x.ShouldHaveSingleItem();
            x.Single().ShouldBe(@"myObj.ShouldBe(123m);");
        }


        [TestMethod]
        public void ShouldlyTestCoderByteArrayLiteral()
        {
            var x = _shouldlyTestCoder.GenerateShouldBes("myObj", new byte[] { 0x01, 0x02, });
            x.ShouldHaveSingleItem();
            x.Single().ShouldBe(@"myObj.ShouldBe(new byte[] { 0x01, 0x02, });");
        }

        [TestMethod]
        public void ShouldlyTestCoderDateLiteral()
        {
            var x = _shouldlyTestCoder.GenerateShouldBes("myDate", new DateTime(2001, 12, 25, 14, 39, 20, 123));
            x.ShouldHaveSingleItem();
            x.Single().ShouldBe(@"myDate.ShouldBe(DateTime.Parse(""2001-12-25T14:39:20.1230000""));");
        }

        [TestMethod]
        public void ShouldlyTestCoderGuidLiteral()
        {
            var x = _shouldlyTestCoder.GenerateShouldBes("myGuid", Guid.Parse("bb86ee63-ca99-42a5-8c3f-1eb0fed04018"));
            x.ShouldHaveSingleItem();
            x.Single().ShouldBe(@"myGuid.ShouldBe(Guid.Parse(""bb86ee63-ca99-42a5-8c3f-1eb0fed04018""));");
        }

        [TestMethod]
        public void ShouldlyTestCoderArrayOfStrings()
        {
            var x = _shouldlyTestCoder.GenerateShouldBes("myArray", new[] {
                "string1",
                "string2"
            }).ToList();

            x.Count().ShouldBe(4);

            x[0].ShouldBe(@"myArray.ShouldNotBeNull();");
            x[1].ShouldBe(@"myArray.Count().ShouldBe(2);");
            x[2].ShouldBe(@"myArray[0].ShouldBe(""string1"");");
            x[3].ShouldBe(@"myArray[1].ShouldBe(""string2"");");
        }

        [TestMethod]
        public void ShouldlyTestCoderDictionaryOfStrings()
        {
            var x = _shouldlyTestCoder.GenerateShouldBes("myDict", new Dictionary<string, string>()
            {
                ["A"] = "Apple",
                ["B"] = "Banana",
            }).ToList();

            x.Count().ShouldBe(4);

            x[0].ShouldBe(@"myDict.ShouldNotBeNull();");
            x[1].ShouldBe(@"myDict.Count().ShouldBe(2);");
            x[2].ShouldBe(@"myDict[""A""].ShouldBe(""Apple"");");
            x[3].ShouldBe(@"myDict[""B""].ShouldBe(""Banana"");");
        }

        [TestMethod]
        public void ShouldlyTestCoderDictionaryOfIntsByString()
        {
            var x = _shouldlyTestCoder.GenerateShouldBes("myDict", new Dictionary<string, int>()
            {
                ["A"] = 1,
                ["B"] = 2,
            }).ToList();

            x.Count().ShouldBe(4);

            x[0].ShouldBe(@"myDict.ShouldNotBeNull();");
            x[1].ShouldBe(@"myDict.Count().ShouldBe(2);");
            x[2].ShouldBe(@"myDict[""A""].ShouldBe(1);");
            x[3].ShouldBe(@"myDict[""B""].ShouldBe(2);");
        }

        [TestMethod]
        public void ShouldlyTestCoderDictionaryOfStringsByInt()
        {
            var x = _shouldlyTestCoder.GenerateShouldBes("myDict", new Dictionary<int, string>()
            {
                [1] = "A",
                [2] = "B",
            }).ToList();

            x.Count().ShouldBe(4);

            x[0].ShouldBe(@"myDict.ShouldNotBeNull();");
            x[1].ShouldBe(@"myDict.Count().ShouldBe(2);");
            x[2].ShouldBe(@"myDict[1].ShouldBe(""A"");");
            x[3].ShouldBe(@"myDict[2].ShouldBe(""B"");");
        }

        [TestMethod]
        public void ShouldlyTestCoderObjectProperties()
        {
            var x = _shouldlyTestCoder.GenerateShouldBes("myObj", new
            {
                Foo = 1234,
                Bar = "testing"
            }).ToList();

            x.Count().ShouldBe(2);

            x[0].ShouldBe(@"myObj.Foo.ShouldBe(1234);");
            x[1].ShouldBe(@"myObj.Bar.ShouldBe(""testing"");");
        }

        [TestMethod]
        public void ShouldlyTestCoderObjectNestedProperties()
        {
            var x = _shouldlyTestCoder.GenerateShouldBes("myObj", new
            {
                Foo = new
                {
                    Nested = 123
                }
            }).ToList();

            x.Count().ShouldBe(1);

            x[0].ShouldBe(@"myObj.Foo.Nested.ShouldBe(123);");
        }

        [TestMethod]
        public void ShouldlyTestCoderObjectNestedEnumerable()
        {
            var x = _shouldlyTestCoder.GenerateShouldBes("myObj", new
            {
                Foo = new[]
                {
                    new { Nested = 123 }
                }
            }).ToList();

            x.Count().ShouldBe(3);

            x[0].ShouldBe(@"myObj.Foo.ShouldNotBeNull();");
            x[1].ShouldBe(@"myObj.Foo.Count().ShouldBe(1);");
            x[2].ShouldBe(@"myObj.Foo[0].Nested.ShouldBe(123);");
        }

        [TestMethod]
        public void ShouldlyTestCoderSameObjectReferencedTwice()
        {
            var sharedItem = new
            {
                Bar = 123
            };

            var testData = new
            {
                Item1 = sharedItem,
                Item2 = sharedItem
            };

            var x = _shouldlyTestCoder.GenerateShouldBes("myObj", testData).ToList();

            //CodeGenUtil.ShouldlyTest(x, "x");

            x.ShouldNotBeNull();
            x.Count().ShouldBe(2);
            x[0].ShouldBe("myObj.Item1.Bar.ShouldBe(123);");
            x[1].ShouldBe("myObj.Item2.ShouldBe(myObj.Item1);");
        }

        [TestMethod]
        public void ShouldlyTestCoderNoFollow()
        {
            var arg = new NoFollowTestClass()
            {
                IncludeThis = 123,
                ExcludeThis = 345
            };

            var x = _shouldlyTestCoder.GenerateShouldBes("arg", arg,
                nofollow: t => t.Name == "ExcludeThis").ToList();

            //CodeGenUtil.ShouldlyTest(x, "x");

            x.ShouldNotBeNull();
            x.Count().ShouldBe(1);
            x[0].ShouldBe("arg.IncludeThis.ShouldBe(123);");
        }

        public class NoFollowTestClass
        {
            public int IncludeThis { get; set; }
            public int ExcludeThis { get; set; }
        }
    }
}

