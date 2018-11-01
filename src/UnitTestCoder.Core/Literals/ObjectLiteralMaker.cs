using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestCoder.Core.Formatting;

namespace UnitTestCoder.Core.Literal
{
    public class ObjectLiteralMaker : IObjectLiteralMaker
    {
        private readonly IValueLiteralMaker valueLiteralMaker;
        private readonly ITypeNameLiteralMaker typeNameLiteralMaker;
        private readonly IIndenter indenter;

        public ObjectLiteralMaker(IValueLiteralMaker valueLiteralMaker,
            ITypeNameLiteralMaker typeNameLiteralMaker,
            IIndenter indenter)
        {
            this.valueLiteralMaker = valueLiteralMaker;
            this.typeNameLiteralMaker = typeNameLiteralMaker;
            this.indenter = indenter;
        }

        public string MakeObjectLiteral(object arg)
        {
            return String.Join("", objLiteral(arg, nesting: 0));
        }

        private IEnumerable<string> objLiteral(object arg, int nesting)
        {
            string space(string text = "")
            {
                return indenter.Indent(text, nesting);
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
                    nesting++;


                    // Iterate through enumerables
                    foreach(object item in list)
                    {
                        yield return space();

                        foreach(var result in objLiteral(item, nesting))
                        {
                            yield return result;
                        }

                        yield return ",\r\n";
                    }

                    nesting--;
                    yield return space();
                    yield return "}";
                }
                else
                {
                    var props = type.GetProperties().ToDictionary(propertyInfo => propertyInfo.Name);

                    yield return $"new {typename}()\r\n";

                    yield return space();
                    yield return "{\r\n";
                    nesting++;

                    // Iterate through properties
                    foreach(var prop in props)
                    {
                        var getMethod = prop.Value.GetGetMethod();
                        object val = getMethod.Invoke(arg, null);

                        yield return space();

                        yield return $"{prop.Key} = ";

                        foreach(var result in objLiteral(val, nesting))
                        {
                            yield return result;
                        }
                        yield return ",\r\n";
                    }

                    nesting--; 
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

