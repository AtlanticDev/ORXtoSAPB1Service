using Dapper;
using Dapper.FluentMap;
using NASRx.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace NASRx.Repositories.Helpers
{
    public static class DynamicParametersExtensions
    {
        private static bool IgnoreProperty(PropertyInfo property)
            => property.SetMethod == null || !IsMapped(property);

        private static bool IsMapped(MemberInfo member)
        {
            if (FluentMapper.EntityMaps.ContainsKey(member.DeclaringType))
            {
                var ownerClassMappings = FluentMapper.EntityMaps[member.DeclaringType];
                return ownerClassMappings.PropertyMaps.Any(map => map.PropertyInfo.Name == member.Name);
            }
            return false;
        }

        public static void AddTableValuedParameter<T>(this DynamicParameters source, string parameterName, string userDefinedTableTypeName, IEnumerable<T> values, bool validateMappings = false)
        {
            var table = new DataTable();
            var properties = typeof(T).GetProperties().ToArray();

            foreach (var property in properties)
            {
                if (validateMappings && IgnoreProperty(property))
                    continue;
                table.Columns.Add(property.Name, property.PropertyType.GetNullUnderlyingType());
            }

            foreach (var value in values)
            {
                var parameters = new List<object>();
                foreach (var property in properties)
                {
                    if (validateMappings && IgnoreProperty(property))
                        continue;
                    var paramValue = property.GetValue(value);
                    parameters.Add(paramValue);
                }
                table.Rows.Add(parameters.ToArray());
            }
            source.Add(parameterName, table.AsTableValuedParameter(userDefinedTableTypeName));
        }
    }
}