using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTestCoder.Core.Literal
{
    public partial class ValueLiteralMaker
    {
        private string enumLiteral(object arg, Type type)
        {
            // If enum is in a nested class use dot notation not plus.
            string typeFullName = type.FullName.Replace("+", ".");

            // Normally we can get the name of the enum value from Enum.GetName() - this is
            // if a single enum value has been used.
            string singleName = Enum.GetName(type, arg);

            if(singleName != null)
                return $"{typeFullName}.{singleName}";

            // Enum number doesn't match a single enum name. It could be a combination of
            // bitflags or just an unknown value - C# allows any number in an enum regardless
            // of what the enum names are.

            // If this is a Flags enum then see if we can compose from individual bitflags
            string flagsText = enumFlagsLiteral();

            if(flagsText != null)
                return flagsText;

            // Otherwise cast the numeric literal to the enum type e.g. (EnumType)1234
            return enumFallback();



            /// <summary>
            /// Produce text similar to "Enum.Flag1 | Enum.Flag2" for an enum type marked
            /// with the [Flags] attribute.
            /// </summary>
            /// <param name="arg"></param>
            /// <param name="type"></param>
            /// <param name="typeFullName"></param>
            /// <returns></returns>

            string enumFlagsLiteral()
            {
                // If not marked with [Flags] we can't do this.
                if(!(type.GetCustomAttributes(true)?.Any(x => x is FlagsAttribute) ?? false))
                    return null;

                // ulong is the largest possible type an enum can be in C#, so widen the
                // value we have to 64 bits unsigned.
                ulong value = toUlong(Convert.ChangeType(arg, type.GetEnumUnderlyingType()));

                // It is impossible to build bitflags from zero. Normally there would be a None
                // item in the enum to cope with this case.
                if(value == 0ul)
                    return null;

                var flags = type
                    .GetFields()
                    .Where(x => x.IsLiteral)
                    .Select(x => new
                    {
                        x.Name,
                        Value = toUlong(x.GetRawConstantValue()),
                    })
                    // This picks out powers of two only (excluding zero).
                    // So if there are composite flags in the enum they will be excluded.
                    .Where(x => x.Value != 0ul && (x.Value & (x.Value - 1ul)) == 0ul)
                    .ToList();

                var flagTexts = new List<string>();

                foreach(var flag in flags)
                {
                    // Is this flag set?
                    if((value & flag.Value) != 0ul)
                    {
                        // Add the text of the flag name to the result
                        flagTexts.Add($"{typeFullName}.{flag.Name}");

                        // Clear this flag, we will reduce value down to zero.
                        // This also has the effect of - if we have duplicate enum
                        // entries with the same bitfield value, use the first one only.
                        value &= ~flag.Value;
                    }

                    // If we get down to zero, we have a result to return.
                    if(value == 0ul)
                    {
                        return String.Join(" | ", flagTexts);
                    }
                }

                // If we fall out with the value still non-zero, we were unable to produce
                // a result.
                return null;
            }


            string enumFallback()
            {
                // If we can't calulate a sensible text literal, just cast the number to the
                // correct enum type.
                var number = Convert.ChangeType(arg, type.GetEnumUnderlyingType());

                switch(number)
                {
                    case int x: return fmt($"{x}", x < 0);
                    case uint x: return fmt($"{x}u");
                    case long x: return fmt($"{x}L", x < 0L);
                    case ulong x: return fmt($"{x}ul");
                    case short x: return fmt($"{x}", x < 0);
                    case ushort x: return fmt($"{x}");
                    case sbyte x: return fmt($"{x}", x < 0);
                    case byte x: return fmt($"{x}");
                    default:
                        throw new Exception($"{nameof(enumFallback)}: Unexpected enum underlying type");
                }

                string fmt(string numLiteral, bool negative = false)
                {
                    // Cast the number to the appropriate enum type. If the number is negative
                    // then we have to enclose the negative number in brackets too.
                    return $"({typeFullName})"
                        + (negative ? $"({numLiteral})" : numLiteral);
                }
            }

            /// <summary>
            /// Widen the arg to 64 bits unsigned so we can treat all enums as 64-bit bitfields.
            /// We mask off the unused bits so they don't show up as being set if we started
            /// with a signed, negative value.
            /// </summary>
            /// <param name="num"></param>
            /// <returns></returns>
            ulong toUlong(object num)
            {
                switch(num)
                {
                    case byte x:    return (ulong)x & 0x000000FFul;
                    case sbyte x:   return (ulong)x & 0x000000FFul;
                    case short x:   return (ulong)x & 0x0000FFFFul;
                    case ushort x:  return (ulong)x & 0x0000FFFFul;
                    case int x:     return (ulong)x & 0xFFFFFFFFul;
                    case uint x:    return (ulong)x & 0xFFFFFFFFul;
                    case long x:    return (ulong)x;
                    case ulong x:   return x;
                    default:
                        throw new Exception($"{nameof(toUlong)}: Unexpected enum underlying type");
                }
            }

        }



    }
}
