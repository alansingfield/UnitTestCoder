using System;

namespace UnitTestCoder.Core.Literal
{
    public interface ITypeLiteralMaker
    {
        bool CanMake(Type type);
        string Literal(Type type);
    }
}