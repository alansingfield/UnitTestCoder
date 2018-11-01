using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using UnitTestCoder.Core.Gen;
using UnitTestCoder.Shouldly.Gen;
using Shouldly;

namespace UnitTestCoder.Demo.ObjectLiterals
{
    public class MyItem
    {
        public int Number { get; set; }
        public string Text { get; set; }
    }

    [TestClass]
    public class ObjectLiteralExampleTests
    {
        private MyItem create(int n) => new MyItem() { Number = n, Text = n.ToString() };
        private MyItem process(MyItem x) => new MyItem() { Number = x.Number + 1, Text = x.Text + "X" };

        [TestMethod]
        public void ObjectLiteralExample1()
        {
            var item = create(20);

            ObjectLiteral.Gen(item, nameof(item));
        }

        [TestMethod]
        public void ObjectLiteralExample2()
        {
            var item = new MyItem()
            {
                Number = 20,
                Text = "20",
            };

            var result = process(item);

            ShouldlyTest.Gen(result, nameof(result));

            {
                result.Number.ShouldBe(21);
                result.Text.ShouldBe("20X");
            }
        }
    }
}
