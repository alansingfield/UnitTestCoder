using System;
using System.Reflection;

namespace UnitTestCoder.Shouldly.Coder
{
    public interface IShouldlyTestCoder
    {
        string Code<T>(T arg, string lvalue, Func<PropertyInfo, bool> nofollow = null);
    }
}