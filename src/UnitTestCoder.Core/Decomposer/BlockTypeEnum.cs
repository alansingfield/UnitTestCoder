using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestCoder.Core.Decomposer
{
    public enum BlockTypeEnum
    {
        Literal,
        Reference,
        ArrayStart,
        ArrayEnd,
        ObjectStart,
        ObjectEnd,
        DictionaryStart,
        DictionaryEnd,
    }
}
