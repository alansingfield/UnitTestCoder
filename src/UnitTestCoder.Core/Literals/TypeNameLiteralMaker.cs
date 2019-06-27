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
            return getNestedTypeName(type, type, fullyQualify, fullyQualify, 0);
        }


        private string getNestedTypeName(
            Type type,
            Type target, 
            bool fullyQualify, 
            bool fqSub, 
            int depth)
        {
            if(type == null)
                throw new ArgumentNullException(nameof(type));

            if(depth > 50)
                throw new StackOverflowException();

            var nestings = new List<Type>();

            // If it is a nested type, look back through DeclaringType so we can get back to the
            // root type.

            int nestCount = 0;
            while(type != null)
            {
                // Limit nesting
                nestCount++;
                if(nestCount > 50)
                    throw new StackOverflowException();

                nestings.Add(type);

                // Generic parameters e.g. T have the DeclaringType as the type they are part of
                // so we must exit to avoid infinite loop.
                if(type.IsGenericParameter)
                    break;

                // Go to the declaring type (the parent in the nesting relationship)
                type = type.DeclaringType;
            }

            // Reverse the order so the root type is first.

            var parts = nestings
                .AsEnumerable()
                .Reverse()
                .Select((x, idx) =>
                    getFullTypeName(
                        x,
                        target,
                        fullyQualify:   (fullyQualify && idx == 0),  // Only fully qualify first item (outermost type)
                        fqSub:          fullyQualify,     // Sub elements need to be fully qualified?
                        depth:          depth + 1)
                ).ToList();


            return String.Join(".",parts);
        }

        //https://stackoverflow.com/questions/1533115/get-generictype-name-in-good-format-using-reflection-on-c-sharp
        /// <summary>
        /// Get correct type name for generic type i.e. List&lt;string&gt; not List`1
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private string getFullTypeName(
            Type type,
            Type target,
            bool fullyQualify,
            bool fqSub, int depth)
        {
            // If this is a generic parameter (e.g. the T in List<T>) we should return an empty string
            // This is so we get an open generic type like typeof(IDictionary<,>)
            if(type.IsGenericParameter && !target.IsConstructedGenericType)
                return "";

            if(type.IsArray)
            {
                // Create [] for 1d array, [,] for 2d array and so on.
                string indexer =
                    "["
                    + new string(',', (type.GetArrayRank() - 1))
                    + "]";

                return getNestedTypeName(type.GetElementType(), target, fqSub, fqSub, depth + 1) 
                    + indexer;
            }

            // Simple type like int, string, nullable int etc?
            string simple = simpleTypeName(type);
            if(simple != null)
                return simple;

            string typeName = fullyQualify ? type.FullName : type.Name;

            // Find which generic arguments are required at this level
            var gen = getNestedGenericArguments(type, target);
            if(gen.Count == 0)
                return typeName;    // not generic.

            // We will have something like List`T - we want the text before the backtick.
            string baseName = typeName.Substring(0, typeName.LastIndexOf("`"));

            // Recursive call to get the name of each generic parameter.
            var genericArgs = String.Join(",", gen
                .Select(g => getNestedTypeName(g, type, fqSub, fqSub, depth + 1)));

            // Produce text like List<string>
            return $"{baseName}<{genericArgs}>";
        }

        private List<Type> getNestedGenericArguments(Type type, Type target)
        {
            // https://stackoverflow.com/questions/55051292/getting-generic-type-from-the-declaringtype-of-a-nested-type-in-c-sharp-with-ref

            // GetGenericArguments() needs a bit of a boost for nested constructed generic types
            // such as Class1<string>.Class2<int>
            // We need to use the target type to get the resolved type names, but use the positions
            // within each nesting level.

            // target is: Class1<string>.Class2<int>; this will give us string,int
            var targetArgs = target.GetGenericArguments();

            // When looking at Class1<> we want to fill in the generic argument with string
            // When looking at Class2<> we want to fill in the generic argument with int
            int startIdx = type.DeclaringType?.GetGenericArguments().Length ?? 0;
            int count = type.GetGenericArguments().Length;

            return targetArgs.Skip(startIdx).Take(count).ToList();
        }
        
        private string simpleTypeName(Type type)
        {
            if(type == typeof(string))
                return "string";

            if(type == typeof(object))
                return "object";

            var nullable = Nullable.GetUnderlyingType(type);
            var target = nullable ?? type;

            var builtIn = builtInType(Type.GetTypeCode(target));

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
