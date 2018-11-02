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
        public static void Gen(object arg, string lvalue = "this", int indent = 2)
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

            string text = objectPopulationCoder.Code(arg, lvalue);

            Console.WriteLine(text);
        }
    }
}
