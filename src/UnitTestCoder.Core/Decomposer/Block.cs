using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestCoder.Core.Decomposer
{
    public class Block : IBlock
    {
        public BlockTypeEnum BlockType { get; set; }
        public string LValue { get; set; }
        public string RValue { get; set; }
        public int? Count { get; set; }
        public Type DataType { get; set; }
    }
}
