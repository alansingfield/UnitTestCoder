using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestCoder.Core.Literal
{
    public class TypeNameLiteralMaker : ITypeNameLiteralMaker
    {
        public string Literal(Type type)
        {
            return getNestedTypeName(type);
        }

        private string getNestedTypeName(Type type)
        {
            var parts = new List<string>();
            while(type != null)
            {
                parts.Add(getFullTypeName(type));

                // Get the parent type or null if there is no class nesting
                type = type.DeclaringType;
            }

            // Results are in inner to outer order, reverse so we get Parent.Child.Grandchild
            return String.Join(".", Enumerable.Reverse(parts));
        }

        //https://stackoverflow.com/questions/1533115/get-generictype-name-in-good-format-using-reflection-on-c-sharp
        /// <summary>
        /// Get correct type name for generic type i.e. List&lt;string&gt; not List`1
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private string getFullTypeName(Type type)
        {
            if(!type.IsGenericType)
                return type.Name;

            StringBuilder sb = new StringBuilder();

            sb.Append(type.Name.Substring(0, type.Name.LastIndexOf("`")));
            sb.Append(type.GetGenericArguments().Aggregate("<",

                delegate (string aggregate, Type t)
                {
                    return aggregate + (aggregate == "<" ? "" : ",") + getNestedTypeName(t);
                }
                ));
            sb.Append(">");

            return sb.ToString();
        }
    }
}
