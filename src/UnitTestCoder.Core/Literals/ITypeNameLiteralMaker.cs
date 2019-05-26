﻿using System;

namespace UnitTestCoder.Core.Literal
{
    public interface ITypeNameLiteralMaker
    {
        string Literal(Type type);
        bool CanMake(Type type);
    }
}