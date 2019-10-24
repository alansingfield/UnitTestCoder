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
        /// <summary>
        /// Creates a Shouldly test for each property in the object model passed in.
        /// Handles circular references.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg"></param>
        /// <param name="lvalue"></param>
        /// <param name="indent"></param>
        /// <param name="nofollow"></param>
        public static void Gen<T>(T arg,
            string lvalue = "model",
            int indent = 3,
            Func<PropertyInfo, bool> nofollow = null)
        {
            string text = prepare(arg, lvalue, indent, nofollow);

            // Writing the text to console causes it to show in the MSTest output window.
            Console.WriteLine(text);
        }

        /// <summary>
        /// Use this as opposed to Gen() if you are running tests in parallel and want to capture all
        /// the generated tests at once. The results from Console.WriteLine() get mixed up when multiple
        /// threads are involved.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg"></param>
        /// <param name="lvalue"></param>
        /// <param name="indent"></param>
        /// <param name="nofollow"></param>
        public static void Throw<T>(T arg,
           string lvalue = "model",
           int indent = 3,
           Func<PropertyInfo, bool> nofollow = null)
        {
            string text = prepare(arg, lvalue, indent, nofollow);

            throw new Exception(text);
        }

        private static string prepare<T>(
            T arg, 
            string lvalue, 
            int indent, 
            Func<PropertyInfo, bool> nofollow)
        {
            // A bit of poor-mans DI here to instantiate all our components.
            var valueLiteralMaker = new ValueLiteralMaker();
            var typeNameLiteralMaker = new TypeNameLiteralMaker();
            var typeLiteralMaker = new TypeLiteralMaker(typeNameLiteralMaker);
            var objectDecomposer = new ObjectDecomposer(valueLiteralMaker, typeLiteralMaker);

            var shouldlyTestMaker = new ShouldlyTestMaker(
                objectDecomposer,
                valueLiteralMaker);

            var indenter = new Indenter(offset: indent);

            var coder = new ShouldlyTestCoder(
                shouldlyTestMaker,
                objectDecomposer,
                valueLiteralMaker,
                indenter);

            return coder.Code(arg, lvalue, nofollow);
        }
    }
}
