using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTestCoder.Core.Formatting
{
    public interface IIndenter
    {
        string Indent(string arg, int level);
    }
}
