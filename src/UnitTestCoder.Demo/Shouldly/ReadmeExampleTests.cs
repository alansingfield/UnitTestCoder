using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using UnitTestCoder.Shouldly.Gen;
using Shouldly;

namespace UnitTestCoder.Demo.Shouldly
{
    [TestClass]
    public class ReadmeExampleTests
    {
        [TestMethod]
        public void ReadmeExample1()
        {
            var result = new
            {
                X = 1 + 2,
                Y = "Hello " + "World"
            };

            ShouldlyTest.Gen(result, nameof(result));
        }

        [TestMethod]
        public void ReadmeExample2()
        {
            var result = new
            {
                X = 1 + 2,
                Y = "Hello " + "World"
            };

            // ShouldlyTest.Gen(result, nameof(result));

            {
                result.X.ShouldBe(3);
                result.Y.ShouldBe("Hello World");
            }
        }
    }
}
