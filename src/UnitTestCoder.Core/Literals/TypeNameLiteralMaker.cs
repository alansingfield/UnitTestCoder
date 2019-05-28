using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.Reflection;

namespace UnitTestCoder.Core.Literal
{
    public class TypeNameLiteralMaker : ITypeNameLiteralMaker
    {
        public string Literal(Type type, bool fullyQualify = false)
        {
            return getNestedTypeName(type, fullyQualify, 0);
        }

        private string getNestedTypeName(Type type, bool fullyQualify, int depth)
        {
            if(depth > 50)
                throw new StackOverflowException();

            var parts = new List<string>();
            while(type != null)
            {
                parts.Add(getFullTypeName(type, fullyQualify, depth + 1));

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
        private string getFullTypeName(Type type, bool fullyQualify, int depth)
        {
            string typeName = fullyQualify && !type.IsNested ? type.FullName : type.Name;

            if(!type.IsGenericType)
                return typeName;

            StringBuilder sb = new StringBuilder();

            sb.Append(typeName.Substring(0, typeName.LastIndexOf("`")));
            sb.Append(type.GetGenericArguments().Aggregate("<",

                delegate (string aggregate, Type t)
                {
                    return aggregate + (aggregate == "<" ? "" : ",") + getNestedTypeName(t, fullyQualify, depth + 1);
                }
                ));
            sb.Append(">");

            return sb.ToString();
        }

        public bool CanMake(Type type)
        {
            if(type == null)
                throw new NullReferenceException(nameof(type));

            // Can't do typeof(List<T>) (how would you define T?)
            if(type.ContainsGenericParameters)
                return false;

            // Can't do anonymous types
            var compilerGenerated = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false);
            if(compilerGenerated.Any())
                return false;

            // Disallow private types (normal or nested)
            if((!type.IsNested && !type.IsPublic) || (type.IsNested && !type.IsNestedPublic))
                return false;

            return true;
        }
    }
}
