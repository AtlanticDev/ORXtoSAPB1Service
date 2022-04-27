using Dapper.FluentMap.Dommel.Resolvers;
using System;
using System.Reflection;

namespace NASRx.Repositories.Helpers
{
    internal class DbObjectNameResolver
    {
        private DommelColumnNameResolver ColumnResolver { get; }
        private DommelTableNameResolver TableResolver { get; }

        internal DbObjectNameResolver()
        {
            ColumnResolver = new DommelColumnNameResolver();
            TableResolver = new DommelTableNameResolver();
        }

        internal string GetColumnName(PropertyInfo property)
            => ColumnResolver.ResolveColumnName(property);

        internal string GetTableName(Type type)
            => TableResolver.ResolveTableName(type);
    }
}
