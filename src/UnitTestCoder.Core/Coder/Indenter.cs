using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTestCoder.Core.Coder
{
    public class Indenter : IIndenter
    {
        private readonly int spacing;
        private readonly bool tabs;
        private readonly int offset;

        public Indenter(int spacing = 4, bool tabs = false, int offset = 0)
        {
            this.spacing = spacing;
            this.tabs = tabs;
            this.offset = offset;
        }

        public string Indent(string arg, int level)
        {
            return new string(
                tabs ? '\t' : ' ', 
                (level + offset) * spacing
            ) 
            + arg;
        }
    }
}
