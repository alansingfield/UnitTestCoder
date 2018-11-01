using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnitTestCoder.Core.Decomposer;
using UnitTestCoder.Core.Literal;

namespace UnitTestCoder.Shouldly.Coder
{
    public class ShouldlyTestCoder
    {
        private readonly IObjectDecomposer objectDecomposer;
        private readonly IValueLiteralMaker valueLiteralMaker;

        public ShouldlyTestCoder(
            IObjectDecomposer objectDecomposer,
            IValueLiteralMaker valueLiteralMaker)
        {
            this.objectDecomposer = objectDecomposer;
            this.valueLiteralMaker = valueLiteralMaker;
        }

        public IEnumerable<string> GenerateShouldBes<T>(string lvalue, T arg, Func<PropertyInfo, bool> nofollow = null)
        {
            foreach(var block in objectDecomposer.Decompose(lvalue, arg, nofollow))
            {
                switch(block.BlockType)
                {
                    case BlockTypeEnum.ArrayStart:
                        yield return $"{block.LValue}.ShouldNotBeNull();";
                        yield return $"{block.LValue}.Count().ShouldBe({block.Count});";
                        break;

                    case BlockTypeEnum.DictionaryStart:
                        yield return $"{block.LValue}.ShouldNotBeNull();";
                        yield return $"{block.LValue}.Count().ShouldBe({block.Count});";
                        break;

                    case BlockTypeEnum.Literal:
                    case BlockTypeEnum.Reference:
                        yield return $"{block.LValue}.ShouldBe({block.RValue});";
                        break;
                }
            }
        }
    }
}
