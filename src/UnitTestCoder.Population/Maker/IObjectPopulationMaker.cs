using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTestCoder.Population.Maker
{
    public interface IObjectPopulationMaker
    {
        IEnumerable<string> Populate(object arg, string lvalue);
    }
}
