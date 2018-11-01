using System.Collections.Generic;

namespace UnitTestCoder.Core.Literal
{
    public interface IObjectLiteralMaker
    {
        string MakeObjectLiteral(object arg, int indent = 0);
    }
}