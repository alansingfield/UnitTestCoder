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
            return getNestedTypeName(type, fullyQualify, fullyQualify, 0);
        }


        private string getNestedTypeName(Type type, bool fullyQualify, bool fqSub, int depth)
        {
            if(type == null)
                throw new ArgumentNullException(nameof(type));

            if(depth > 50)
                throw new StackOverflowException();

            var nestings = new List<Type>();

            // If it is a nested type, look back through DeclaringType so we can get back to the
            // root type.
            while(type != null)
            {
                // TODO - limit nesting!!!
                nestings.Add(type);

                // Generic parameters e.g. T have the DeclaringType as the type they are part of
                // so we must exit to avoid infinite loop.
                if(type.IsGenericParameter)
                    break;

                type = type.DeclaringType;
            }

            // Reverse the order so the root type is first.

            var parts = nestings.AsEnumerable().Reverse().Select((x, idx) =>
                getFullTypeName(x,
                   fullyQualify: (fullyQualify && idx == 0),  // Only fully qualify first item (outermost type)
                   fqSub: fullyQualify,     // Sub elements need to be fully qualified?
                   depth: depth + 1)
            ).ToList();

            //var parts = new List<string>();


            //parts.Add(getFullTypeName(nestings.First(), 
            //    fullyQualify,
            //    fqSub: fullyQualify,
            //    depth: depth + 1));
            
            

            //while(type.IsNested)
            //{
            //    // Generic parameters e.g. T have the DeclaringType as the type they are part of
            //    // so we must exit to avoid infinite loop.
            //    if(type.IsGenericParameter)
            //        break;
                
            //    parts.Add(type.Name);

            //    // Get the parent type or null if there is no class nesting
            //    type = type.DeclaringType;
            //}

            //parts.Add(getFullTypeName(type, fullyQualify, depth: depth + 1));


            return String.Join(".",parts);
        }

        //https://stackoverflow.com/questions/1533115/get-generictype-name-in-good-format-using-reflection-on-c-sharp
        /// <summary>
        /// Get correct type name for generic type i.e. List&lt;string&gt; not List`1
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private string getFullTypeName(Type type, bool fullyQualify, bool fqSub, int depth)
        {
            // If this is a generic parameter (e.g. the T in List<T>) we should return an empty string
            // This is so we get an open generic type like typeof(IDictionary<,>)
            if(type.IsGenericParameter)
                return "floop";

            if(type.IsArray)
            {
                // Create [] for 1d array, [,] for 2d array and so on.
                string indexer =
                    "["
                    + new string(',', (type.GetArrayRank() - 1))
                    + "]";

                return getNestedTypeName(type.GetElementType(), fqSub, fqSub, depth + 1) 
                    + indexer;
            }

            // Simple type like int, string, nullable int etc?
            string simple = simpleTypeName(type);
            if(simple != null)
                return simple;


            string typeName = fullyQualify ? type.FullName : type.Name;

            // Non-generics, return the type name.
            if(!type.IsGenericType)
                return typeName;

            var gen = getNestedGenericArguments(type);
            if(gen.Count == 0)
                return typeName;

            // We will have something like List`T - we want the text before the backtick.
            string baseName = typeName.Substring(0, typeName.LastIndexOf("`"));

            var genericArgs = String.Join(",", gen
                .Select(g => getNestedTypeName(g, fqSub, fqSub, depth + 1)));

            return $"{baseName}<{genericArgs}>";
        }

        private List<Type> getNestedGenericArguments(Type type)
        {
            if(!type.IsNested)
                return type.GetGenericArguments().ToList();

            // https://stackoverflow.com/questions/55051292/getting-generic-type-from-the-declaringtype-of-a-nested-type-in-c-sharp-with-ref
            // For nested types, the generic arguments from the outer class are repeated on the inner.
            // So we need to skip over these. I'm doing this by position, not sure this is the official
            // way but it gives the right answer
            return type.GetGenericArguments().Skip(type.DeclaringType.GetGenericArguments().Length).ToList();
        }
        
        private string simpleTypeName(Type type)
        {
            if(type == typeof(string))
                return "string";

            if(type == typeof(object))
                return "object";

            var nullable = Nullable.GetUnderlyingType(type);
            var root = nullable ?? type;

            var builtIn = builtInType(Type.GetTypeCode(root));

            if(builtIn != null)
                return builtIn + ((nullable != null) ? "?" : "");

            return null;
        }

        private string builtInType(TypeCode typeCode)
        {
            switch(typeCode)
            {
                case TypeCode.Boolean:  return "bool";
                case TypeCode.Char:     return "char";
                case TypeCode.SByte:    return "sbyte";
                case TypeCode.Byte:     return "byte";
                case TypeCode.Int16:    return "short";
                case TypeCode.UInt16:   return "ushort";
                case TypeCode.Int32:    return "int";
                case TypeCode.UInt32:   return "uint";
                case TypeCode.Int64:    return "long";
                case TypeCode.UInt64:   return "ulong";
                case TypeCode.Single:   return "float";
                case TypeCode.Double:   return "double";
                case TypeCode.Decimal:  return "decimal";
                case TypeCode.DateTime: return "DateTime";
            }
            return null;
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
