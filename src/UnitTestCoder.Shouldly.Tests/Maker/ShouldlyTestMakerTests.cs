using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using UnitTestCoder.Core.Decomposer;
using UnitTestCoder.Core.Literal;
using UnitTestCoder.Shouldly.Maker;
using Shouldly;
using System.Linq;

namespace UnitTestCoder.Shouldly.Tests.Maker
{
    [TestClass]
    public partial class ShouldlyTestMakerTests
    {
        private ShouldlyTestMaker _shouldlyTestMaker;

        [TestInitialize]
        public void Init()
        {
            _shouldlyTestMaker = new ShouldlyTestMaker(
                new ObjectDecomposer(
                    new ValueLiteralMaker(),
                    new TypeLiteralMaker(new TypeNameLiteralMaker())),
                new ValueLiteralMaker());
        }

        [TestMethod]
        public void ShouldlyTestMakerStringNoQuote()
        {
            var x = _shouldlyTestMaker.GenerateShouldBes("myObj", "hello");
            x.ShouldHaveSingleItem();
            x.Single().ShouldBe(@"myObj.ShouldBe(""hello"");");
        }

        [TestMethod]
        public void ShouldlyTestMakerStringQuote()
        {
            var x = _shouldlyTestMaker.GenerateShouldBes("myObj", @"with""a quote");
            x.ShouldHaveSingleItem();
            x.Single().ShouldBe(@"myObj.ShouldBe(@""with""""a quote"");");
        }

        [TestMethod]
        public void ShouldlyTestMakerStringBackslash()
        {
            var x = _shouldlyTestMaker.GenerateShouldBes("myObj", @"back\slash");
            x.ShouldHaveSingleItem();
            x.Single().ShouldBe(@"myObj.ShouldBe(@""back\slash"");");
        }

        [TestMethod]
        public void ShouldlyTestMakerIntLiteral()
        {
            var x = _shouldlyTestMaker.GenerateShouldBes("myObj", 456);
            x.ShouldHaveSingleItem();
            x.Single().ShouldBe(@"myObj.ShouldBe(456);");
        }

        [TestMethod]
        public void ShouldlyTestMakerNull()
        {
            object myObj = null;
            var x = _shouldlyTestMaker.GenerateShouldBes("myObj", myObj);
            x.ShouldHaveSingleItem();
            x.Single().ShouldBe(@"myObj.ShouldBe(null);");
        }


        [TestMethod]
        public void ShouldlyTestMakerDecimalLiteral()
        {
            var x = _shouldlyTestMaker.GenerateShouldBes("myObj", 123m);
            x.ShouldHaveSingleItem();
            x.Single().ShouldBe(@"myObj.ShouldBe(123m);");
        }


        [TestMethod]
        public void ShouldlyTestMakerByteArrayLiteral()
        {
            var x = _shouldlyTestMaker.GenerateShouldBes("myObj", new byte[] { 0x01, 0x02, });
            x.ShouldHaveSingleItem();
            x.Single().ShouldBe(@"myObj.ShouldBe(new byte[] { 0x01, 0x02, });");
        }

        [TestMethod]
        public void ShouldlyTestMakerStringArrayLiteral()
        {
            var x = _shouldlyTestMaker.GenerateShouldBes("myObj", new string[] { "banana", "carrot", });
            x.ShouldHaveSingleItem();
            x.Single().ShouldBe(@"myObj.ShouldBe(new[] { ""banana"", ""carrot"" });");
        }

        [TestMethod]
        public void ShouldlyTestMakerDateLiteral()
        {
            var x = _shouldlyTestMaker.GenerateShouldBes("myDate", new DateTime(2001, 12, 25, 14, 39, 20, 123));
            x.ShouldHaveSingleItem();
            x.Single().ShouldBe(@"myDate.ShouldBe(DateTime.Parse(""2001-12-25T14:39:20.1230000""));");
        }

        [TestMethod]
        public void ShouldlyTestMakerGuidLiteral()
        {
            var x = _shouldlyTestMaker.GenerateShouldBes("myGuid", Guid.Parse("bb86ee63-ca99-42a5-8c3f-1eb0fed04018"));
            x.ShouldHaveSingleItem();
            x.Single().ShouldBe(@"myGuid.ShouldBe(Guid.Parse(""bb86ee63-ca99-42a5-8c3f-1eb0fed04018""));");
        }

        [TestMethod]
        public void ShouldlyTestMakerArrayOfStrings()
        {
            var x = _shouldlyTestMaker.GenerateShouldBes("myArray", new[] {
                "string1",
                "string2"
            }).ToArray();

            x.Count().ShouldBe(1);

            x[0].ShouldBe(@"myArray.ShouldBe(new[] { ""string1"", ""string2"" });");
        }

        [TestMethod]
        public void ShouldlyTestMakerListOfStrings()
        {
            var x = _shouldlyTestMaker.GenerateShouldBes("myList", new List<string>() {
                "string1",
                "string2"
            }).ToList();

            x.Count().ShouldBe(1);

            x[0].ShouldBe(@"myList.ShouldBe(new[] { ""string1"", ""string2"" });");
        }




        [TestMethod]
        public void ShouldlyTestMakerDictionaryOfStrings()
        {
            var x = _shouldlyTestMaker.GenerateShouldBes("myDict", new Dictionary<string, string>()
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
        public void ShouldlyTestMakerDictionaryOfIntsByString()
        {
            var x = _shouldlyTestMaker.GenerateShouldBes("myDict", new Dictionary<string, int>()
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
        public void ShouldlyTestMakerDictionaryOfStringsByInt()
        {
            var x = _shouldlyTestMaker.GenerateShouldBes("myDict", new Dictionary<int, string>()
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
        public void ShouldlyTestMakerObjectProperties()
        {
            var x = _shouldlyTestMaker.GenerateShouldBes("myObj", new
            {
                Foo = 1234,
                Bar = "testing"
            }).ToList();

            x.Count().ShouldBe(2);

            x[0].ShouldBe(@"myObj.Foo.ShouldBe(1234);");
            x[1].ShouldBe(@"myObj.Bar.ShouldBe(""testing"");");
        }

        [TestMethod]
        public void ShouldlyTestMakerObjectNestedProperties()
        {
            var x = _shouldlyTestMaker.GenerateShouldBes("myObj", new
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
        public void ShouldlyTestMakerObjectNestedEnumerable()
        {
            var x = _shouldlyTestMaker.GenerateShouldBes("myObj", new
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
        public void ShouldlyTestMakerSameObjectReferencedTwice()
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

            var x = _shouldlyTestMaker.GenerateShouldBes("myObj", testData).ToList();

            //CodeGenUtil.ShouldlyTest(x, "x");

            x.ShouldNotBeNull();
            x.Count().ShouldBe(2);
            x[0].ShouldBe("myObj.Item1.Bar.ShouldBe(123);");
            x[1].ShouldBe("myObj.Item2.ShouldBe(myObj.Item1);");
        }

        [TestMethod]
        public void ShouldlyTestMakerNoFollow()
        {
            var arg = new NoFollowTestClass()
            {
                IncludeThis = 123,
                ExcludeThis = 345
            };

            var x = _shouldlyTestMaker.GenerateShouldBes("arg", arg,
                nofollow: t => t.Name == "ExcludeThis").ToList();

            //CodeGenUtil.ShouldlyTest(x, "x");

            x.ShouldNotBeNull();
            x.Count().ShouldBe(1);
            x[0].ShouldBe("arg.IncludeThis.ShouldBe(123);");
        }

        [TestMethod]
        public void ShouldlyTestMakerType()
        {
            var myObj = new
            {
                StringType = typeof(string)
            };

            var x = _shouldlyTestMaker.GenerateShouldBes("myObj", myObj).ToList();

            x.Count().ShouldBe(1);

            x[0].ShouldBe(@"myObj.StringType.ShouldBe(typeof(System.String));");
        }

        public class NoFollowTestClass
        {
            public int IncludeThis { get; set; }
            public int ExcludeThis { get; set; }
        }



        private class WithLazyProp
        {
            private readonly Lazy<int> lazyInt;

            public WithLazyProp(Lazy<int> lazyInt)
            {
                this.lazyInt = lazyInt;
            }

            public int IntVal => this.lazyInt.Value;
        }

        [TestMethod]
        public void ShouldlyTestMakerLazyProperty()
        {
            var myObj = new WithLazyProp(new Lazy<int>(() => 123));

            var x = _shouldlyTestMaker.GenerateShouldBes("myObj", myObj);
            x.ShouldHaveSingleItem();
            x.Single().ShouldBe(@"myObj.IntVal.ShouldBe(123);");
        }

        [TestMethod]
        public void ShouldlyTestMakerPropertyinfo()
        {
            // Pick any old property
            var propertyInfo = typeof(String).GetProperties()[0];

            // This will go into the wilderness picking out every type declared in the declaring
            // Assembly. We want this to complete rather than crashing so the user can see their
            // mistake (and add a nofollow).
            Should.NotThrow(() =>
            {
                var m = _shouldlyTestMaker.GenerateShouldBes("propertyInfo", propertyInfo).ToList();
            });
        }
    }
}

