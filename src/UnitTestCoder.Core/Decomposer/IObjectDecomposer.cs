using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnitTestCoder.Core.Decomposer
{
    public interface IObjectDecomposer
    {
        IEnumerable<IBlock> Decompose<T>(string lvalue, T arg, Func<PropertyInfo, bool> noFollowFunc = null, bool byInstance = false);
    }
}