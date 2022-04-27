using NASRx.Infra.Abstractions;
using NASRx.Utilities;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace NASRx.Infra.Concretes
{
    public class Context : IContext
    {        
        public Context() 
            => Connection = Create();

        private static DbConnection Create()
            => new SqlConnection(NASRxSettings.Instance.ConnectionString);

        public DbConnection Connection { get; }

        public DbTransaction Transaction { get; private set; }

        public void BeginTransaction()
        {
            if (Transaction == null)
            {
                // Open the connection if it is not already
                if (Connection.State == ConnectionState.Closed)
                    Connection.Open();

                // Generate the transaction
                Transaction = Connection.BeginTransaction();
            }
        }

        public void CommitTransaction()
        {
            Transaction?.Commit();
            Transaction?.Dispose();
            Transaction = null;
        }

        public void Dispose()
        {
            if (Transaction != null)
                RollbackTransaction();

            Connection?.Close();
            Connection?.Dispose();
            GC.SuppressFinalize(this);
        }

        public void RollbackTransaction()
        {
            Transaction?.Rollback();
            Transaction?.Dispose();
            Transaction = null;
        }
    }
}