using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnitTestCoder.Core.Decomposer;
using UnitTestCoder.Core.Literal;

namespace UnitTestCoder.Shouldly.Maker
{
    public class ShouldlyTestMaker : IShouldlyTestMaker
    {
        private readonly IObjectDecomposer objectDecomposer;
        private readonly IValueLiteralMaker valueLiteralMaker;

        public ShouldlyTestMaker(
            IObjectDecomposer objectDecomposer,
            IValueLiteralMaker valueLiteralMaker)
        {
            this.objectDecomposer = objectDecomposer;
            this.valueLiteralMaker = valueLiteralMaker;
        }

        public IEnumerable<string> GenerateShouldBes<T>(string lvalue, T arg, Func<PropertyInfo, bool> nofollow = null)
        {
            foreach(var block in stringListShortcut(objectDecomposer.Decompose(lvalue, arg, nofollow)))
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

        private IEnumerable<IBlock> stringListShortcut(IEnumerable<IBlock> blocks)
        {
            BlockTypeEnum? skipToBlockType = null;
            object skipToRawValue = null;

            foreach(var block in blocks)
            {
                bool processed = false;

                if(skipToBlockType != null)
                {
                    // If we are skipping loop round
                    if(block.BlockType != skipToBlockType || block.RawValue != skipToRawValue)
                        continue;

                    // Matched the skip target, don't skip any more.
                    skipToBlockType = null;
                    skipToRawValue = null;

                    // Don't return the final skipped item.
                    continue;
                }

                switch(block.BlockType)
                {
                    // If we are doing a test against a List<string> then substitute in a
                    // string array to compare to, and skip over all the individual string
                    // elements we will be sent. Note we are quite conservative in matching;
                    // I didn't use IEnumerable<string> as this could feasibly be implemented
                    // by an object with many more fields to test.
                    case BlockTypeEnum.ArrayStart:
                        if(block.RawValue is List<string>)
                        {
                            string[] array = ((List<string>)block.RawValue).ToArray();

                            yield return new Block()
                            {
                                BlockType = BlockTypeEnum.Literal,
                                DataType = typeof(string[]),
                                LValue = block.LValue,
                                RawValue = array,
                                RValue = valueLiteralMaker.Literal(array)
                            };

                            skipToBlockType = BlockTypeEnum.ArrayEnd;
                            skipToRawValue = block.RawValue;
                            
                            processed = true;
                        }
                        break;
                }

                if(!processed)
                    yield return block;
            }
        }
    }
}
