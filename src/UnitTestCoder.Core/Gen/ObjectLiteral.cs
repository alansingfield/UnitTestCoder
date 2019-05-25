using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnitTestCoder.Core.Coder;
using UnitTestCoder.Core.Formatting;
using UnitTestCoder.Core.Literal;

namespace UnitTestCoder.Core.Gen
{
    public static class ObjectLiteral
    {
        /// <summary>
        /// Generates a C# object literal from the arg provided. Cannot handle circular
        /// references, use ObjectPopulation.Gen() for this instead.
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="lvalue"></param>
        /// <param name="indent"></param>
        /// <param name="nofollow"></param>
        public static void Gen(
            object arg, 
            string lvalue = "model", 
            int indent = 3, 
            Func<PropertyInfo, bool> nofollow = null)
        {
            string text = prepare(arg, lvalue, indent, nofollow);

            Console.WriteLine(text);
        }

        /// <summary>
        /// Use this as opposed to Gen() if you are running tests in parallel and want to capture all
        /// the generated tests at once. The results from Console.WriteLine() get mixed up when multiple
        /// threads are involved.
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="lvalue"></param>
        /// <param name="indent"></param>
        /// <param name="nofollow"></param>
        public static void Throw(
            object arg,
            string lvalue = "model",
            int indent = 3,
            Func<PropertyInfo, bool> nofollow = null)
        {
            string text = prepare(arg, lvalue, indent, nofollow);

            throw new Exception(text);
        }

        private static string prepare(
            object arg,
            string lvalue, 
            int indent,
            Func<PropertyInfo, bool> nofollow)
        {
            var indenter = new Indenter(offset: indent);

            var valueLiteralMaker = new ValueLiteralMaker();
            var typeNameLiteralMaker = new TypeNameLiteralMaker();
            var objectLiteralMaker = new ObjectLiteralMaker(
                valueLiteralMaker,
                typeNameLiteralMaker,
                indenter);

            var objectLiteralCoder = new ObjectLiteralCoder(
                objectLiteralMaker,
                indenter);

            string text = objectLiteralCoder.Code(arg, lvalue, nofollow);
            return text;
        }
    }
}
