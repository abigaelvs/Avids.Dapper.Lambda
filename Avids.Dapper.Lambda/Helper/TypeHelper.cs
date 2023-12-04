using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Avids.Dapper.Lambda.Helper
{
    internal static class TypeHelper
    {
        /// <summary>
        /// Find IEnumerable property in Type
        /// </summary>
        /// <param name="seqType"></param>
        /// <returns></returns>
        public static Type FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
                return null;
            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
            if (seqType.IsGenericType)
            {
                foreach (Type arg in seqType.GetGenericArguments())
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        return ienum;
                    }
                }
            }
            Type[] ifaces = seqType.GetInterfaces();
            if (ifaces.Length > 0)
            {
                foreach (Type iface in ifaces)
                {
                    Type ienum = FindIEnumerable(iface);
                    if (ienum != null) return ienum;
                }
            }
            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
            {
                return FindIEnumerable(seqType.BaseType);
            }
            return null;
        }

        /// <summary>
        /// Check if Type is Nullable
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullableType(Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Check if Type is Null Assignable
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullAssignable(Type type)
        {
            return !type.IsValueType || IsNullableType(type);
        }

        /// <summary>
        /// Get Non Nullable Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetNonNullableType(Type type)
        {
            if (IsNullableType(type))
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }

        /// <summary>
        /// Get Column Attribute Name
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static string GetColumnAttributeName(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute<ColumnAttribute>()?.Name ?? propertyInfo.Name;
        }

        /// <summary>
        /// Get Table Attribute Name
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTableAttributeName(this Type type)
        {
            return type.GetCustomAttribute<TableAttribute>()?.Name ?? type.Name;
        }
    }

}
