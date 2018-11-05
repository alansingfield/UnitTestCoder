using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnitTestCoder.Core.Literal
{
    public interface IObjectLiteralMaker
    {
        string MakeObjectLiteral(object arg, Func<PropertyInfo, bool> noFollowFunc = null);
    }
}