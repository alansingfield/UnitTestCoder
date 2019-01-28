using System;

namespace UnitTestCoder.Core.Literal
{
    public interface IValueLiteralMaker
    {
        string Literal(object arg);
        bool CanMake(Type type);
    }
}