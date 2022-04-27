using Dapper;
using NASRx.Infra.Abstractions;
using NASRx.Model;
using NASRx.Model.Annotation;
using NASRx.Repositories.Abstractions;
using NASRx.Repositories.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NASRx.Repositories.Concretes
{
    public abstract class GenericRepository<TMDL, TPK> : IGenericRepository<TMDL, TPK> where TMDL : Entity<TPK>
    {
        public GenericRepository(IContext context)
        {
            Context = context;
            NameResolver = new DbObjectNameResolver();
        }

        protected IContext Context { get; }

        internal DbObjectNameResolver NameResolver { get; }

        private DynamicParameters AddParameter(TMDL entity, bool update, DynamicParameters result, PropertyInfo property)
        {
            if (property.Name != nameof(Entity<TPK>.Id))
            {
                result.Add(name: $"@{NameResolver.GetColumnName(property)}", value: property.GetValue(entity));
                return result;
            }

            result = AddPrimaryKeyParameter(entity, update, result, property);
            return result;
        }

        private DynamicParameters AddPrimaryKeyParameter(TMDL entity, bool update, DynamicParameters result, PropertyInfo property)
        {
            if (update)
                result.Add(name: $"@{NameResolver.GetColumnName(property)}", value: property.GetValue(entity));
            else
                result.Add(name: $"@{NameResolver.GetColumnName(property)}", value: property.GetValue(entity), direction: ParameterDirection.InputOutput);
            return result;
        }

        private static Type GetAttibuteType(bool update)
            => update ? typeof(IgnoreOnUpdateAttribute) : typeof(IgnoreOnInsertAttribute);

        private static IEnumerable<PropertyInfo> GetWritableProperties(TMDL entity, Type attributeType)
        {
            var type = entity.GetType();
            return type.GetProperties()
                    .Where(p => p.DeclaringType == type || p.Name == nameof(Entity<TPK>.Id))
                    .Where(p => p.GetSetMethod() != null && p.GetSetMethod().IsPublic && !p.GetCustomAttributes(attributeType).Any());
        }

        protected virtual DynamicParameters GetDynamicParameters(TMDL entity, bool update)
        {
            var result = new DynamicParameters();
            var attributeType = GetAttibuteType(update);
            var properties = GetWritableProperties(entity, attributeType);
            foreach (var property in properties)
            {
                result = AddParameter(entity, update, result, property);
            }
            return result;
        }

        public virtual async Task<bool> Delete(TMDL entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Id.Equals(default))
                throw new ArgumentException($"Invalid {entity.GetType().Name}.{nameof(entity.Id)}");

            var property = typeof(TMDL).GetProperty(nameof(entity.Id));
            var parameter = new DynamicParameters();
            parameter.Add($"@{NameResolver.GetColumnName(property)}", entity.Id);

            var result = await Context.Connection.ExecuteScalarAsync<int>(
                transaction: Context.Transaction,
                commandType: CommandType.StoredProcedure,
                sql: $"{entity.GetType().Name}_Delete",
                param: parameter) == 0;

            return result;
        }

        public virtual async Task<TMDL> Insert(TMDL entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var parameters = GetDynamicParameters(entity, false);

            await Context.Connection.ExecuteScalarAsync<int>(
                 transaction: Context.Transaction,
                 commandType: CommandType.StoredProcedure,
                 sql: $"{entity.GetType().Name}_Create",
                 param: parameters);

            var property = typeof(TMDL).GetProperty(nameof(entity.Id));
            entity.Id = parameters.Get<TPK>($"@{NameResolver.GetColumnName(property)}");
            return entity;
        }

        public virtual async Task<bool> Update(TMDL entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Id.Equals(default))
                throw new ArgumentException($"Invalid {entity.GetType().Name}.{nameof(entity.Id)}");

            var parameters = GetDynamicParameters(entity, true);

            var result = await Context.Connection.ExecuteScalarAsync<int>(
                transaction: Context.Transaction,
                commandType: CommandType.StoredProcedure,
                sql: $"{entity.GetType().Name}_Update",
                param: parameters);

            return result == 0;
        }
    }
}