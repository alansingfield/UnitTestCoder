using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTestCoder.Core.Coder
{
    public interface IIndenter
    {
        string Indent(string arg, int level);
    }
}
