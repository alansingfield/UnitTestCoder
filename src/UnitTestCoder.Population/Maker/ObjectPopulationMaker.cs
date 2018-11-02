using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnitTestCoder.Core.Decomposer;
using UnitTestCoder.Core.Literal;

namespace UnitTestCoder.Population.Maker
{
    public class ObjectPopulationMaker : IObjectPopulationMaker
    {
        private readonly IObjectDecomposer objectDecomposer;
        private readonly IValueLiteralMaker valueLiteralMaker;
        private readonly ITypeNameLiteralMaker typeNameLiteralMaker;

        public ObjectPopulationMaker(
            IObjectDecomposer objectDecomposer,
            IValueLiteralMaker valueLiteralMaker,
            ITypeNameLiteralMaker typeNameLiteralMaker)
        {
            this.objectDecomposer = objectDecomposer;
            this.valueLiteralMaker = valueLiteralMaker;
            this.typeNameLiteralMaker = typeNameLiteralMaker;
        }

        //public string Populate(object arg, string lvalue)
        //{
        //    return String.Join("\r\n", populate(lvalue, arg));
        //}

        public IEnumerable<string> Populate(object arg, string lvalue)
        {
            var locals = new Stack<(string, string)>();
            int objectSkip = 0;
            bool firstObject = true;

            foreach(var block in objectDecomposer.Decompose(lvalue, arg,
                noFollowFunc: t => !t.CanWrite,
                byInstance: true))
            {
                if(objectSkip == 0)
                {
                    switch(block.BlockType)
                    {
                        case BlockTypeEnum.ObjectStart:
                            yield return objectStart();
                            break;

                        case BlockTypeEnum.ArrayStart:
                            yield return arrayStart();
                            break;

                        case BlockTypeEnum.Literal:
                            yield return literal();
                            break;

                        case BlockTypeEnum.Reference:
                            yield return reference();
                            break;

                        case BlockTypeEnum.ArrayEnd:
                            yield return arrayEnd();
                            break;
                    }
                }
                else
                {
                    switch(block.BlockType)
                    {
                        case BlockTypeEnum.ObjectStart:
                            objectSkip++;
                            break;

                        case BlockTypeEnum.ObjectEnd:
                            objectSkip--;
                            break;
                    }
                }

                string objectStart()
                {
                    // Can we construct this object with no parameters e.g. = new X()
                    bool hasParameterlessConstructor = block.DataType.GetConstructor(new Type[0]) != null;

                    string typename = typeNameLiteralMaker.Literal(block.DataType);
                    string lvalue2 = substituteLocal(block.LValue);

                    if(hasParameterlessConstructor || firstObject)
                    {
                        // CHEAT - we assume that the first object can always be constructed.
                        firstObject = false;

                        return $"{lvalue2} = new {typename}();";
                    }
                    else
                    {
                        objectSkip = 1;
                        return $"// {lvalue2} = new {typename}();";
                    }
                }

                string arrayStart()
                {
                    string lvalue2 = block.LValue;
                    if(!block.DataType.IsArray)
                    {
                        var genericType = block.DataType.GenericTypeArguments.First();
                        string typename = typeNameLiteralMaker.Literal(genericType);

                        if(lvalue2 == lvalue)
                        {
                            lvalue2 = "local0";
                        }
                        else
                        {
                            lvalue2 = lvalue2
                                .Replace("[", "_")
                                .Replace("]", "_")
                                .Replace(".", "_");
                        }

                        locals.Push((lvalue2, block.LValue));
                        return $"var {lvalue2} = new {typename}[{block.Count}];";
                    }
                    else
                    {
                        string typename = typeNameLiteralMaker.Literal(block.DataType.GetElementType());

                        return $"{block.LValue} = new {typename}[{block.Count}];";
                    }
                }

                string arrayEnd()
                {
                    if(!block.DataType.IsArray)
                    {
                        string typename = typeNameLiteralMaker.Literal(block.DataType);

                        var (local, original) = locals.Pop();
                        string lvalue2 = substituteLocal(original);
                        return $"{lvalue2} = new {typename}({local});";
                    }
                    return "";
                }

                string substituteLocal(string lvalue2)
                {
                    foreach(var (local, original) in locals)
                    {
                        if(lvalue2.StartsWith(original))
                        {
                            return local + lvalue2.Substring(original.Length);
                        }
                    }

                    return lvalue2;
                }

                string literal()
                {
                    string lvalue2 = substituteLocal(block.LValue);
                    return $"{lvalue2} = {block.RValue};";
                }

                string reference()
                {
                    string lvalue2 = substituteLocal(block.LValue);
                    string rvalue2 = substituteLocal(block.RValue);
                    return $"{lvalue2} = {rvalue2};";
                }
            }
        }
    }

}
