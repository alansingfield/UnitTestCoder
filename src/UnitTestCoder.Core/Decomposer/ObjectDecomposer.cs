﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnitTestCoder.Core.Collections;
using UnitTestCoder.Core.Literal;

namespace UnitTestCoder.Core.Decomposer
{
    public class ObjectDecomposer : IObjectDecomposer
    {
        private readonly IValueLiteralMaker valueLiteralMaker;

        public ObjectDecomposer(IValueLiteralMaker valueLiteralMaker)
        {
            this.valueLiteralMaker = valueLiteralMaker;
        }
        
        public IEnumerable<IBlock> Decompose<T>(
            string lvalue, 
            T arg,
            Func<PropertyInfo, bool> noFollowFunc = null,
            bool byInstance = false)
        {
            var seenObjects = new Dictionary<object, string>();

            Type declaredType = byInstance ? arg?.GetType() : typeof(T);

            return decompose(lvalue, arg, declaredType, noFollowFunc, byInstance, seenObjects);
        }

        private IEnumerable<IBlock> decompose(
            string lvalue,
            object arg,
            Type declaredType,
            Func<PropertyInfo, bool> noFollowFunc,
            bool byInstance,
            Dictionary<object, string> seenObjects)
        {

            if(arg == null)
            {
                yield return literal(lvalue, "null"); // $"{lvalue}.ShouldBeNull();";
            }
            else
            {
                var instanceType = arg.GetType();
                var type = declaredType;

                if(type.IsValueType || type == typeof(string) || type == typeof(byte[]))
                {
                    string rvalue = valueLiteralMaker.Literal(arg);
                    yield return literal(lvalue, rvalue);// $"{lvalue}.ShouldBe({rvalue});";
                }
                else
                {
                    // If we've seen an object before, link back to it instead of returning
                    // each and every property.
                    if(seenObjects.TryGetValue(arg, out string existing))
                    {
                        yield return reference(lvalue, existing); // $"{lvalue}.ShouldBe({existing});";
                    }
                    else
                    {
                        // Remember this object in case we see it again.
                        seenObjects.Add(arg, lvalue);

                        if(typeof(IDictionary).IsAssignableFrom(type))
                        {
                            var dict = ((IDictionary)arg);

                            yield return dictionaryStart(lvalue, type, dict.Count);

                            var elementType = getDictionaryElementType(declaredType);

                            foreach(object key in dict.Keys)
                            {
                                object value = dict[key];

                                string newLValue = $"{lvalue}[{valueLiteralMaker.Literal(key)}]";

                                foreach(var result in decompose(
                                    newLValue,
                                    value,
                                    byInstance ? value?.GetType() : elementType,
                                    noFollowFunc,
                                    byInstance,
                                    seenObjects))
                                {
                                    yield return result;
                                }

                            }

                            yield return dictionaryEnd(lvalue, type, dict.Count);
                        }
                        else if(typeof(IEnumerable).IsAssignableFrom(type))
                        {
                            var list = ((IEnumerable)arg).Cast<object>().ToList();

                            yield return arrayStart(lvalue, type, list.Count);                       //$"{lvalue}.ShouldNotBeNull();";
                                                                                                     //yield return $"{lvalue}.Count().ShouldBe({list.Count()});";

                            var elementType = getElementType(declaredType);

                            // Iterate through enumerables
                            int idx = 0;
                            foreach(object item in list)
                            {
                                string newLValue = $"{lvalue}[{idx}]";
                                foreach(var result in decompose(
                                    newLValue,
                                    item,
                                    byInstance ? item?.GetType() : elementType,
                                    noFollowFunc,
                                    byInstance,
                                    seenObjects
                                    ))
                                {
                                    yield return result;
                                }

                                idx++;
                            }

                            yield return arrayEnd(lvalue, type, list.Count);
                        }
                        else
                        {
                            yield return objectStart(lvalue, type);

                            var props = type.GetProperties().ToDictionary(propertyInfo => propertyInfo.Name);

                            // Iterate through properties
                            foreach(var (propertyName, prop) in props)
                            {
                                // Skip over any property that the noFollowFunc tells us to ignore.
                                if(noFollowFunc?.Invoke(prop) ?? false)
                                    continue;

                                var getMethod = prop.GetGetMethod();
                                object val = getMethod.Invoke(arg, null);

                                string newLValue = $"{lvalue}.{propertyName}";

                                Type elementType = byInstance ? val?.GetType() : prop.PropertyType;

                                foreach(var result in decompose(
                                    newLValue,
                                    val,
                                    elementType,
                                    noFollowFunc,
                                    byInstance,
                                    seenObjects))
                                {
                                    yield return result;
                                }
                            }

                            yield return objectEnd(lvalue, type);
                        }
                    }
                }
            }
        }

        private IBlock literal(string lvalue, string rvalue)
        {
            return new Block()
            {
                LValue = lvalue,
                RValue = rvalue,
                BlockType = BlockTypeEnum.Literal
            };
        }

        private IBlock reference(string lvalue, string rvalue)
        {
            return new Block()
            {
                LValue = lvalue,
                RValue = rvalue,
                BlockType = BlockTypeEnum.Reference
            };
        }

        private IBlock arrayStart(string lvalue, Type dataType, int count)
        {
            return new Block()
            {
                LValue = lvalue,
                Count = count,
                DataType = dataType,
                BlockType = BlockTypeEnum.ArrayStart
            };
        }
        private IBlock arrayEnd(string lvalue, Type dataType, int count)
        {
            return new Block()
            {
                LValue = lvalue,
                Count = count,
                DataType = dataType,
                BlockType = BlockTypeEnum.ArrayEnd
            };
        }

        private IBlock dictionaryStart(string lvalue, Type dataType, int count)
        {
            return new Block()
            {
                LValue = lvalue,
                Count = count,
                DataType = dataType,
                BlockType = BlockTypeEnum.DictionaryStart
            };
        }

        private IBlock dictionaryEnd(string lvalue, Type dataType, int count)
        {
            return new Block()
            {
                LValue = lvalue,
                Count = count,
                DataType = dataType,
                BlockType = BlockTypeEnum.DictionaryEnd
            };
        }

        private IBlock objectStart(string lvalue, Type dataType)
        {
            return new Block()
            {
                LValue = lvalue,
                DataType = dataType,
                BlockType = BlockTypeEnum.ObjectStart
            };
        }
        private IBlock objectEnd(string lvalue, Type dataType)
        {
            return new Block()
            {
                LValue = lvalue,
                DataType = dataType,
                BlockType = BlockTypeEnum.ObjectEnd
            };
        }

        // https://stackoverflow.com/questions/906499/getting-type-t-from-ienumerablet
        private Type getElementType(Type type)
        {
            // Type is Array
            // short-circuit if you expect lots of arrays 
            if(type.IsArray)
                return type.GetElementType();

            // type is IEnumerable<T>;
            if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return type.GetGenericArguments()[0];

            // type implements/extends IEnumerable<T>;
            var enumType = type.GetInterfaces()
                                    .Where(t => t.IsGenericType &&
                                           t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                                    .Select(t => t.GenericTypeArguments[0]).FirstOrDefault();
            return enumType ?? type;
        }

        private Type getDictionaryElementType(Type type)
        {
            if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                return type.GetGenericArguments()[1];

            var enumType = type.GetInterfaces()
                                    .Where(t => t.IsGenericType &&
                                           t.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                                    .Select(t => t.GenericTypeArguments[1]).FirstOrDefault();
            return enumType ?? type;
        }

    }


}