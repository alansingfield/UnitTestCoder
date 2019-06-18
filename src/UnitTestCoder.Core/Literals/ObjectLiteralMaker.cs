using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnitTestCoder.Core.Formatting;
using UnitTestCoder.Core.Collections;
using UnitTestCoder.Core.Decomposer;

namespace UnitTestCoder.Core.Literal
{
    public class ObjectLiteralMaker : IObjectLiteralMaker
    {
        private readonly IValueLiteralMaker valueLiteralMaker;
        private readonly ITypeNameLiteralMaker typeNameLiteralMaker;
        private readonly ITypeLiteralMaker typeLiteralMaker;
        private readonly IIndenter indenter;

        public ObjectLiteralMaker(
            IValueLiteralMaker valueLiteralMaker,
            ITypeNameLiteralMaker typeNameLiteralMaker,
            ITypeLiteralMaker typeLiteralMaker,
            IIndenter indenter)
        {
            this.valueLiteralMaker = valueLiteralMaker;
            this.typeNameLiteralMaker = typeNameLiteralMaker;
            this.typeLiteralMaker = typeLiteralMaker;
            this.indenter = indenter;
        }

        public string MakeObjectLiteral(object arg, Func<PropertyInfo, bool> noFollowFunc = null)
        {
            HashSet<object> seenObjects = new HashSet<object>();

            return String.Join("", objLiteral(arg, 0, noFollowFunc, seenObjects));
        }

        private IEnumerable<string> objLiteral(object arg, int nesting, Func<PropertyInfo, bool> noFollowFunc, HashSet<object> seenObjects)
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

                if(arg is Type)
                {
                    yield return typeLiteral((Type)arg);

                }
                else if(valueLiteralMaker.CanMake(type))
                {
                    yield return literal(arg);
                }
                else
                {
                    string typename = getFullTypeName(type);

                    // Check for circular reference
                    if(seenObjects.Contains(arg))
                        throw new Exception($"Circular reference detected for object of type '{typename}'");

                    seenObjects.Add(arg);

                    if(typeof(IEnumerable).IsAssignableFrom(type))
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

                            foreach(var result in objLiteral(item, nesting, noFollowFunc, seenObjects))
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
                        foreach(var (name, prop) in props)
                        {
                            // Check the nofollow() function
                            bool nofollow = (noFollowFunc?.Invoke(prop)) ?? false;

                            // Property must be writeable.
                            if(prop.CanWrite && !nofollow)
                            {
                                // Read the current value of the property
                                var getMethod = prop.GetGetMethod();
                                object val = getMethod.Invoke(arg, null);

                                yield return space();

                                yield return $"{name} = ";

                                foreach(var result in objLiteral(val, nesting, noFollowFunc, seenObjects))
                                {
                                    yield return result;
                                }
                                yield return ",\r\n";
                            }
                        }

                        nesting--;
                        yield return space();
                        yield return "}";
                    }
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

        private string typeLiteral(Type t)
        {
            if(typeLiteralMaker.CanMake(t))
                return typeLiteralMaker.Literal(t);
            else
                return $"typeof(/*{t}*/)";
        }
    }
}

