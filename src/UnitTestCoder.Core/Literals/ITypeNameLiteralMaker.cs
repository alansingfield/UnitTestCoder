using System;

namespace UnitTestCoder.Core.Literal
{
    public interface ITypeNameLiteralMaker
    {
        string Literal(Type type, bool fullyQualify = false);
        bool CanMake(Type type);
    }
}