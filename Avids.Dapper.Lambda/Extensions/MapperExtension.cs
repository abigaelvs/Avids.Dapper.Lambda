using System.Data;
using System.Reflection;

using Avids.Dapper.Lambda.Helper;

namespace Avids.Dapper.Lambda.Extension
{
    internal static class MapperExtension
    {
        /// <summary>
        /// List to DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> list)
        {
            Type type = typeof(T);
            string tableName = type.GetTableAttributeName();
            List<PropertyInfo> properties = type.GetProperties().ToList();

            DataTable newDt = new DataTable(tableName);

            properties.ForEach(propertie =>
            {
                Type columnType;
                if (propertie.PropertyType.IsGenericType && propertie.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    columnType = propertie.PropertyType.GetGenericArguments()[0];
                }
                else
                {
                    columnType = propertie.PropertyType;
                }

                string columnName = propertie.GetColumnAttributeName();
                newDt.Columns.Add(columnName, columnType);
            });

            foreach (var item in list)
            {
                DataRow newRow = newDt.NewRow();

                properties.ForEach(propertie =>
                {
                    newRow[propertie.GetColumnAttributeName()] = propertie.GetValue(item, null) ?? DBNull.Value;
                });

                newDt.Rows.Add(newRow);
            }

            return newDt;
        }
    }
}
