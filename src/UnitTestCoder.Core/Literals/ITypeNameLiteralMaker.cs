using System;

namespace UnitTestCoder.Core.Literal
{
    public interface ITypeNameLiteralMaker
    {
        string Literal(Type type);
    }
}