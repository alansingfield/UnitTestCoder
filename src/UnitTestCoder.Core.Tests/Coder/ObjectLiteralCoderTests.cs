using DryIoc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using UnitTestCoder.Core.Coder;
using UnitTestCoder.Core.Formatting;
using UnitTestCoder.Core.Tests.DryIoc;
using Shouldly;
using UnitTestCoder.Core.Literal;
using NSubstitute;
using System.Reflection;

namespace UnitTestCoder.Core.Tests.Coder
{
    [TestClass]
    public class ObjectLiteralCoderTests
    {
        [TestMethod]
        public void ObjectLiteralCoderCallsMaker()
        {
            var container = open();

            var coder = container.Resolve<IObjectLiteralCoder>();
            var maker = container.Resolve<IObjectLiteralMaker>();

            var input = new { A = 123 };
            Func<PropertyInfo, bool> noFollowFunc = x => false;

            maker.MakeObjectLiteral(input, noFollowFunc).Returns("new { A = 123 }");

            var result = coder.Code(input, "input", noFollowFunc);

            maker.Received(1).MakeObjectLiteral(input, noFollowFunc);

            result.ShouldBe("var input = new { A = 123 };");
        }

        private IContainer open()
        {
            var container = new Container().WithNSubstituteFallback(Reuse.Singleton);

            container.Register<IObjectLiteralCoder, ObjectLiteralCoder>(Reuse.Singleton);

            container.Register<IIndenter, Indenter>(Reuse.Singleton);

            return container;
        }
    }
}
