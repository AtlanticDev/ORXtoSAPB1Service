using Dapper.FluentMap.Dommel.Mapping;
using NASRx.Model;

namespace NASRx.ModelConfig
{
    internal class BaseMapping<TMDL, TPK> : DommelEntityMap<TMDL> where TMDL : Entity<TPK>
    {
        internal BaseMapping(string tableName = null, string idColumnName = null)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                tableName = typeof(TMDL).Name;

            ToTable(tableName);

            var map = Map(t => t.Id).IsKey();
            if (typeof(TPK) == typeof(int))
                map = map.IsIdentity();

            if (!string.IsNullOrWhiteSpace(idColumnName))
                map.ToColumn(idColumnName);
        }
    }
}