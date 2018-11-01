using System;
using System.Collections.Generic;
using System.Text;
using UnitTestCoder.Core.Formatting;
using UnitTestCoder.Core.Literal;

namespace UnitTestCoder.Core.Coder
{
    public class ObjectLiteralCoder : IObjectLiteralCoder
    {
        private readonly IObjectLiteralMaker objectLiteralMaker;
        private readonly IValueLiteralMaker valueLiteralMaker;
        private readonly ITypeNameLiteralMaker typeNameLiteralMaker;
        private readonly IIndenter indenter;

        public ObjectLiteralCoder(
            IObjectLiteralMaker objectLiteralMaker,
            IValueLiteralMaker valueLiteralMaker,
            ITypeNameLiteralMaker typeNameLiteralMaker,
            IIndenter indenter)
        {
            this.objectLiteralMaker = objectLiteralMaker;
            this.valueLiteralMaker = valueLiteralMaker;
            this.typeNameLiteralMaker = typeNameLiteralMaker;
            this.indenter = indenter;
        }

        public string Code(object arg, string lvalue)
        {
            string declaration = String.IsNullOrWhiteSpace(lvalue) ? "" : $"var {lvalue} = ";

            string literal = objectLiteralMaker.MakeObjectLiteral(arg);

            return indenter.Indent($"{declaration}{literal};", 0);
        }
    }
}
