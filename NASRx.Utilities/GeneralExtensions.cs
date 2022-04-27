using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace NASRx.Utilities
{
    public static class GeneralExtensions
    {
        public static T GetAttribute<T>(this MemberInfo property) where T : Attribute
            => property?.GetCustomAttributes(typeof(T), true).Cast<T>()?.FirstOrDefault();

        public static string GetDescription(this Enum enumerator)
        {
            try
            {
                var fieldInfo = enumerator.GetType().GetField(enumerator.ToString());
                var attribute = fieldInfo.GetAttribute<DescriptionAttribute>();
                return attribute != null ? attribute.Description : enumerator.ToString();
            }
            catch { return enumerator.ToString(); }
        }

        public static Type GetNullUnderlyingType(this Type type)
            => type.IsNullable() ? Nullable.GetUnderlyingType(type) : type;

        public static string GetValueByAttribute<T>(this object @object) where T : Attribute
        {
            try
            {
                var property = @object.GetType().GetProperties().FirstOrDefault(p => GetAttribute<T>(p) != null);
                return property?.GetValue(@object)?.ToString();
            }
            catch { return null; }
        }

        public static bool IsNullable(this Type type)
            => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
            => source == null || !source.Any();

        public static string OnlyNumbers(this string value)
            => string.IsNullOrEmpty(value) ? string.Empty : string.Join(string.Empty, value.Where(char.IsDigit).ToArray());

        public static T ToEnum<T>(this int value, T @default = default) where T : struct
            => value.ToString().ToEnum(@default);

        public static T ToEnum<T>(this string value, T @default = default) where T : struct
            => string.IsNullOrEmpty(value) ? @default : (T)Enum.Parse(typeof(T), value, true);
    }
}