using System;
using System.Collections.Generic;
using System.Text;
using UnitTestCoder.Core.Decomposer;
using UnitTestCoder.Core.Formatting;
using UnitTestCoder.Core.Literal;
using UnitTestCoder.Population.Coder;
using UnitTestCoder.Population.Maker;

namespace UnitTestCoder.Population.Gen
{
    public static class ObjectPopulation
    {
        /// <summary>
        /// Generates individual C# statements to populate an object model
        /// property by property. Handles circular references.
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="lvalue"></param>
        /// <param name="indent"></param>
        public static void Gen(object arg, string lvalue = "this", int indent = 2)
        {
            string text = prepare(arg, lvalue, indent);

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
        public static void Throw(object arg, string lvalue = "this", int indent = 2)
        {
            string text = prepare(arg, lvalue, indent);

            throw new Exception(text);
        }

        private static string prepare(object arg, string lvalue, int indent)
        {
            var valueLiteralMaker = new ValueLiteralMaker();
            var typeNameLiteralMaker = new TypeNameLiteralMaker();
            var objectDecomposer = new ObjectDecomposer(valueLiteralMaker);
            var indenter = new Indenter(offset: indent);

            var objectPopulationMaker = new ObjectPopulationMaker(
                objectDecomposer,
                valueLiteralMaker,
                typeNameLiteralMaker);

            var objectPopulationCoder = new ObjectPopulationCoder(
                objectPopulationMaker,
                indenter);

            return objectPopulationCoder.Code(arg, lvalue);
        }
    }
}
