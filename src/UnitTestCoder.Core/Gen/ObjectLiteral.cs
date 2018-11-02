using System;
using System.Collections.Generic;
using System.Text;
using UnitTestCoder.Core.Coder;
using UnitTestCoder.Core.Formatting;
using UnitTestCoder.Core.Literal;

namespace UnitTestCoder.Core.Gen
{
    public static class ObjectLiteral
    {
        public static void Gen(object arg, string lvalue = "model", int indent = 3)
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

            string text = objectLiteralCoder.Code(arg, lvalue);

            Console.WriteLine(text);
        }
    }
}
