using System;

namespace UnitTestCoder.Core.Decomposer
{
    public interface IBlock
    {
        BlockTypeEnum BlockType { get; set; }
        int? Count { get; set; }
        string LValue { get; set; }
        string RValue { get; set; }
        Type DataType { get; set; }
        object RawValue { get; set; }
    }
}