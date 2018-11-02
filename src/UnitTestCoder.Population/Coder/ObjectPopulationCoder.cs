using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnitTestCoder.Core.Formatting;
using UnitTestCoder.Population.Maker;

namespace UnitTestCoder.Population.Coder
{
    public class ObjectPopulationCoder : IObjectPopulationCoder
    {
        private readonly IObjectPopulationMaker objectPopulationMaker;
        private readonly IIndenter indenter;

        public ObjectPopulationCoder(
            IObjectPopulationMaker objectPopulationMaker,
            IIndenter indenter)
        {
            this.objectPopulationMaker = objectPopulationMaker;
            this.indenter = indenter;
        }

        public string Code(object arg, string lvalue)
        {
            var lines = new[] { "{" }
                .Concat(objectPopulationMaker.Populate(arg, lvalue))
                .Concat(new[] { "}", "" });

            return String.Join("\r\n", lines.Select(x => indenter.Indent(x, 0)));
        }
    }
}
