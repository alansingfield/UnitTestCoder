using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnitTestCoder.Shouldly.Maker
{
    public interface IShouldlyTestMaker
    {
        IEnumerable<string> GenerateShouldBes<T>(string lvalue, T arg, Func<PropertyInfo, bool> nofollow = null);
    }
}