using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using UnitTestCoder.Core.Literal;
using UnitTestCoder.Core.Decomposer;
using UnitTestCoder.Shouldly.Maker;
using UnitTestCoder.Core.Formatting;

namespace UnitTestCoder.Shouldly.Coder
{
    public class ShouldlyTestCoder : IShouldlyTestCoder
    {
        private readonly IShouldlyTestMaker shouldlyTestMaker;
        private readonly IObjectDecomposer objectDecomposer;
        private readonly IValueLiteralMaker valueLiteralMaker;
        private readonly IIndenter indenter;

        public ShouldlyTestCoder(
            IShouldlyTestMaker shouldlyTestMaker,
            IObjectDecomposer objectDecomposer,
            IValueLiteralMaker valueLiteralMaker,
            IIndenter indenter
        )
        {
            this.shouldlyTestMaker = shouldlyTestMaker;
            this.objectDecomposer = objectDecomposer;
            this.valueLiteralMaker = valueLiteralMaker;
            this.indenter = indenter;
        }

        public string Code<T>(T arg,
            string lvalue,
            Func<PropertyInfo, bool> nofollow = null)
        {
            var lines = new List<string>();
            lines.Add("{");
            lines.AddRange(shouldlyTestMaker.GenerateShouldBes(lvalue, arg, nofollow));
            lines.Add("}");

            string text = String.Join("\r\n",
                lines.Select(x => indenter.Indent(x, 0)));

            return (text + "\r\n");
        }
    }
}
