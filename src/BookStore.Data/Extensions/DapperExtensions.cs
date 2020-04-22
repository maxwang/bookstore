using Dapper;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Data.Extensions
{
    public static class DapperExtensions
    {
        public static async Task<int> UpdateItem<T>(
            this IDbConnection conn,
            T data,
            string tableName,
            string primaryKeyColumnName,
            IDbTransaction transaction = null
            )
        {
            var sql = PrepareUpdateSql(data, tableName, primaryKeyColumnName);
            return await conn.ExecuteAsync(sql, data, transaction);
        }

        public static async Task<int> UpsertOneToOneSubItem<T>(
           this IDbConnection conn,
           T data,
           string tableName,
           string primaryKeyColumnName,
           IDbTransaction transaction = null
           )
        {
            if (data == null)
            {
                return 0;
            }

            var sql = PrepareUpdateSql(data, tableName, primaryKeyColumnName);
            return await conn.ExecuteAsync(sql, data, transaction);
        }

        public static async Task<int> InsertItem<T>(
            this IDbConnection conn,
            T data,
            string tableName,
            string primaryKeyColumnName,
            IDbTransaction transaction = null,
            Action<int> callback = null,
            bool setIdentityInsert = false) where T : class
        {
            var sql = PrepareInsertSql(data, tableName, primaryKeyColumnName, setIdentityInsert);
            int result = await conn.ExecuteAsync(sql, data, transaction);
            callback?.Invoke(result);

            if (!setIdentityInsert && !(data is IEnumerable))
            {
                var propertyInfo = data.GetType().GetProperty(primaryKeyColumnName);
                propertyInfo.SetValue(data, result, null);
            }

            return result;
        }

        public static Task<T> GetAsync<T>(this IDbConnection conn, object id, string tableName, string primaryKeyColumnName) =>
                conn.QueryFirstOrDefaultAsync<T>($"SELECT * FROM {tableName} WHERE {primaryKeyColumnName} = @id;", new { id });

        private static string PrepareUpdateSql(object data, string tableName, string primaryKeyColumnName)
        {
            IList<string> names = CleanSqlColumns(GetPropertyNames(data), primaryKeyColumnName, false);
            var sql = new StringBuilder();
            sql.Append($"UPDATE {tableName} SET ");
            sql.Append(string.Join(",", names.Select(x => $"{x} = @{x}")));
            sql.Append($" WHERE {primaryKeyColumnName} = @{primaryKeyColumnName};");
            return sql.ToString();
        }

        private static string PrepareInsertSql(object data, string tableName, string primaryKeyColumnName, bool setIdentityInsert)
        {
            IList<string> names = CleanSqlColumns(GetPropertyNames(data), primaryKeyColumnName, setIdentityInsert);
            string cols = string.Join(",", names);
            string colsParams = string.Join(",", names.Select(p => "@" + p));
            var sql = new StringBuilder();
            if (setIdentityInsert)
            {
                sql.Append($"SET IDENTITY_INSERT {tableName} ON;");
            }
            sql.Append("SET NOCOUNT ON;");
            sql.Append($"INSERT INTO {tableName} ({cols}) VALUES ({colsParams});");
            if (setIdentityInsert)
            {
                sql.Append($"SET IDENTITY_INSERT {tableName} OFF;");
            }
            sql.Append($"SELECT CAST(SCOPE_IDENTITY() AS INT);");
            return sql.ToString();
        }

        private static IList<string> CleanSqlColumns(IList<string> names, string primaryKeyColumnName, bool needIdColumn)
        {
            return needIdColumn ? names : names.Where(n => string.Compare(n, primaryKeyColumnName) != 0).ToList();
        }

        private static readonly ConcurrentDictionary<Type, List<string>> paramNameCache = new ConcurrentDictionary<Type, List<string>>();
        private static IList<string> GetPropertyNames(object data)
        {
            if (data is DynamicParameters parameters)
            {
                return parameters.ParameterNames.ToList();
            }

            var dataType = data.GetType();
            if (typeof(IEnumerable).IsAssignableFrom(dataType))
            {
                dataType = dataType.GetGenericArguments()[0];
            }

            if (!paramNameCache.TryGetValue(dataType, out List<string> results))
            {
                results = dataType
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => IsSimleType(p.PropertyType) && p.GetGetMethod(false) != null)
                    .Select(x => x.Name)
                    .ToList();

                paramNameCache[data.GetType()] = results;
            }
            return results;
        }

        private static bool IsSimleType(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                return IsSimleType(typeInfo.GetGenericArguments()[0]);
            }

            return typeInfo.IsPrimitive
                   || typeInfo.IsEnum
                   || type == typeof(string)
                   || type == typeof(decimal)
                   || type == typeof(DateTime);
        }
    }
}
