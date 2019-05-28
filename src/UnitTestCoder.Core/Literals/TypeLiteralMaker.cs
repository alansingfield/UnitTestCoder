using System;
using System.Collections.Generic;
using System.Text;
using UnitTestCoder.Core.Literal;

namespace UnitTestCoder.Core.Literal
{
    public class TypeLiteralMaker : ITypeLiteralMaker
    {
        private readonly ITypeNameLiteralMaker typeNameLiteralMaker;

        public TypeLiteralMaker(ITypeNameLiteralMaker typeNameLiteralMaker)
        {
            this.typeNameLiteralMaker = typeNameLiteralMaker;
        }

        public string Literal(Type type)
        {
            string typename = typeNameLiteralMaker.Literal(type, fullyQualify:true);

            return $"typeof({typename})";
        }

        public bool CanMake(Type type)
        {
            return typeNameLiteralMaker.CanMake(type);
        }
    }
}
