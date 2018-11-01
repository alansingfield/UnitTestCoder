using DryIoc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using UnitTestCoder.Core.Tests.DryIoc;
using UnitTestCoder.Shouldly.Coder;
using UnitTestCoder.Shouldly.Maker;
using NSubstitute;
using System.Reflection;
using Shouldly;
using UnitTestCoder.Core.Coder;

namespace UnitTestCoder.Shouldly.Tests.Coder
{
    [TestClass]
    public class ShouldlyTestCoderTests
    {
        [TestMethod]
        public void ShouldlyTestCoderCallsMaker()
        {
            var container = open();

            var coder = container.Resolve<IShouldlyTestCoder>();
            var maker = container.Resolve<IShouldlyTestMaker>();

            // Create a dummy func that we can match on
            Func<PropertyInfo, bool> noFollowFunc = prop => true;

            maker.GenerateShouldBes("lvalue", "test", noFollowFunc).ReturnsForAnyArgs(new[] { "foo" });

            string result = coder.Code("test", "lvalue", noFollowFunc);

            maker.Received(1).GenerateShouldBes("lvalue", "test", noFollowFunc);

            result.ShouldBe("{\r\nfoo\r\n}\r\n");
        }

        private IContainer open()
        {
            var container = new Container().WithNSubstituteFallback(Reuse.Singleton);

            container.Register<IShouldlyTestCoder, ShouldlyTestCoder>(Reuse.Singleton);

            container.Register<IIndenter, Indenter>(Reuse.Singleton);

            return container;
        }
    }
}
