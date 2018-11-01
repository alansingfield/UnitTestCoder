using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnitTestCoder.Core.Formatting;
using UnitTestCoder.Core.Decomposer;
using UnitTestCoder.Core.Literal;
using UnitTestCoder.Shouldly.Coder;
using UnitTestCoder.Shouldly.Maker;

namespace UnitTestCoder.Shouldly.Gen
{
    public static class ShouldlyTest
    {
        public static void Gen<T>(T arg,
            string lvalue = "model",
            int indent = 3,
            Func<PropertyInfo, bool> nofollow = null)
        {
            // A bit of poor-mans DI here to instantiate all our components.

            var valueLiteralMaker = new ValueLiteralMaker();
            var objectDecomposer = new ObjectDecomposer(valueLiteralMaker);

            var shouldlyTestMaker = new ShouldlyTestMaker(
                objectDecomposer,
                valueLiteralMaker);

            var indenter = new Indenter(offset: indent);

            var coder = new ShouldlyTestCoder(
                shouldlyTestMaker,
                objectDecomposer,
                valueLiteralMaker,
                indenter);

            string text = coder.Code(arg, lvalue, nofollow);

            // Writing the text to console causes it to show in the MSTest output window.
            Console.WriteLine(text);
        }
    }
}
