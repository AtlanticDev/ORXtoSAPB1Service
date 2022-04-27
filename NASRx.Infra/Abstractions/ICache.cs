
using System;

namespace NASRx.Infra.Abstractions
{
    public interface ICache
    {
        void ClearAll();

        void Delete(string key);

        void Insert(string key, object value, TimeSpan? expiration = null);

        void Insert(string key, object value, string parentKey, TimeSpan? expiration = null);

        T Read<T>(string key);
    }
}
