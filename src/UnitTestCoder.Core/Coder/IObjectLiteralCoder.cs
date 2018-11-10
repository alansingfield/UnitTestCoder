using System;
using System.Reflection;

namespace UnitTestCoder.Core.Coder
{
    public interface IObjectLiteralCoder
    {
        string Code(object arg, string lvalue, Func<PropertyInfo, bool> noFollowFunc = null);
    }
}