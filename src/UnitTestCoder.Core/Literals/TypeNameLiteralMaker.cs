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
                parts.Add(getFullTypeName(type, fullyQualify, depth: depth + 1));

                // Generic parameters e.g. T have the DeclaringType as the type they are part of
                // so we must exit to avoid infinite loop.
                if(type.IsGenericParameter)
                    break;

                // Reached the end of the nesting.
                if(!type.IsNested)
                    break;

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
        private string getFullTypeName(Type type, bool fullyQualify, int depth)
        {
            string typeName = fullyQualify && !type.IsNested ? type.FullName : type.Name;

            // If this is a generic parameter (e.g. the T in List<T>) we should return an empty string
            // This is so we get an open generic type like typeof(IDictionary<,>)
            if(type.IsGenericParameter)
                return "";

            if(!type.IsGenericType)
                return typeName;

            // We will have something like List`T - we want the text before the backtick.
            string baseName = typeName.Substring(0, typeName.LastIndexOf("`"));

            var genericArgs = String.Join(",", type.GetGenericArguments()
                .Select(g => getNestedTypeName(g, fullyQualify, depth + 1)));

            return $"{baseName}<{genericArgs}>";
        }

        public bool CanMake(Type type)
        {
            if(type == null)
                throw new NullReferenceException(nameof(type));

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
