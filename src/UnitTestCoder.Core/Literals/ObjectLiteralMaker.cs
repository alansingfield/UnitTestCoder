﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestCoder.Core.Literal
{
    public class ObjectLiteralMaker : IObjectLiteralMaker
    {
        private readonly IValueLiteralMaker valueLiteralMaker;
        private readonly ITypeNameLiteralMaker typeNameLiteralMaker;

        public ObjectLiteralMaker(IValueLiteralMaker valueLiteralMaker,
            ITypeNameLiteralMaker typeNameLiteralMaker)
        {
            this.valueLiteralMaker = valueLiteralMaker;
            this.typeNameLiteralMaker = typeNameLiteralMaker;
        }

        public string MakeObjectLiteral(object arg, int indent = 0)
        {
            return String.Join("", objLiteral(arg, indent));
        }
        private IEnumerable<string> objLiteral(object arg, int indent)
        {
            string space()
            {
                return new string(' ', indent * 4);
            }

            if(arg == null)
            {
                yield return $"null";
            }
            else
            {
                var type = arg.GetType();
                string typename = getFullTypeName(type);

                if(type.IsValueType || type == typeof(string))
                {
                    yield return literal(arg);
                }
                else if(typeof(IEnumerable).IsAssignableFrom(type))
                {
                    var list = ((IEnumerable)arg).Cast<object>().ToList();

                    string brackets = type.IsArray ? "" : "()";

                    yield return $"new {typename}{brackets}" + "\r\n";

                    yield return space();
                    yield return "{\r\n";
                    indent++;


                    // Iterate through enumerables
                    foreach(object item in list)
                    {
                        yield return space();

                        foreach(var result in objLiteral(item, indent))
                        {
                            yield return result;
                        }

                        yield return ",\r\n";
                    }

                    indent--;
                    yield return space();
                    yield return "}";
                }
                else
                {
                    var props = type.GetProperties().ToDictionary(propertyInfo => propertyInfo.Name);

                    yield return $"new {typename}()\r\n";

                    yield return space();
                    yield return "{\r\n";
                    indent++;

                    // Iterate through properties
                    foreach(var prop in props)
                    {
                        var getMethod = prop.Value.GetGetMethod();
                        object val = getMethod.Invoke(arg, null);

                        yield return space();

                        yield return $"{prop.Key} = ";

                        foreach(var result in objLiteral(val, indent))
                        {
                            yield return result;
                        }
                        yield return ",\r\n";
                    }

                    indent--; 
                    yield return space();
                    yield return "}";
                }
            }
        }



        private string literal(object arg)
        {
            return valueLiteralMaker.Literal(arg);
        }

        private string getFullTypeName(Type t)
        {
            return typeNameLiteralMaker.Literal(t);
        }
    }
}

