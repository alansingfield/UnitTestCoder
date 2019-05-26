using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;

namespace UnitTestCoder.Core.Literal
{
    public class TypeNameLiteralMaker : ITypeNameLiteralMaker
    {
        public string Literal(Type type)
        {
            return getNestedTypeName(type, 0);
        }

        private string getNestedTypeName(Type type, int depth)
        {
            if(depth > 50)
                throw new StackOverflowException();

            var parts = new List<string>();
            while(type != null)
            {
                parts.Add(getFullTypeName(type, depth+1));

                // Get the parent type or null if there is no class nesting
                type = type.IsNested ? type.DeclaringType : null;
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
        private string getFullTypeName(Type type, int depth)
        {
            if(!type.IsGenericType)
                return type.Name;

            StringBuilder sb = new StringBuilder();

            sb.Append(type.Name.Substring(0, type.Name.LastIndexOf("`")));
            sb.Append(type.GetGenericArguments().Aggregate("<",

                delegate (string aggregate, Type t)
                {
                    return aggregate + (aggregate == "<" ? "" : ",") + getNestedTypeName(t, depth+1);
                }
                ));
            sb.Append(">");

            return sb.ToString();
        }

        public bool CanMake(Type type)
        {
            if(type == null)
                throw new NullReferenceException(nameof(type));

            // Can't do typeof(List<T>)
            if(type.ContainsGenericParameters)
                return false;

            var compilerGenerated = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false);
            if(compilerGenerated.Any())
                return false;

            return true;
        }
    }
}
