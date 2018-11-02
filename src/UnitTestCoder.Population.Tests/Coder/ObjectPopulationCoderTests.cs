using DryIoc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnitTestCoder.Core.Formatting;
using UnitTestCoder.Core.Tests.DryIoc;
using UnitTestCoder.Population.Coder;
using UnitTestCoder.Population.Maker;
using NSubstitute;
using Shouldly;

namespace UnitTestCoder.Population.Tests.Coder
{
    [TestClass]
    public class ObjectPopulationCoderTests
    {
        [TestMethod]
        public void ObjectPopulationCoderCallsMaker()
        {
            var container = open();

            var coder = container.Resolve<IObjectPopulationCoder>();
            var maker = container.Resolve<IObjectPopulationMaker>();

            maker.Populate("test", "lvalue").ReturnsForAnyArgs(new[] { "foo" });

            string result = coder.Code("test", "lvalue");

            maker.Received(1).Populate("test", "lvalue");

            result.ShouldBe("{\r\nfoo\r\n}\r\n");

        }
        private IContainer open()
        {
            var container = new Container().WithNSubstituteFallback(Reuse.Singleton);

            container.Register<IObjectPopulationCoder, ObjectPopulationCoder>(Reuse.Singleton);

            container.Register<IIndenter, Indenter>(Reuse.Singleton);

            return container;
        }
    }
}
