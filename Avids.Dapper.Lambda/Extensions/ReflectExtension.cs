using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Avids.Dapper.Lambda.Exception;

namespace Avids.Dapper.Lambda.Extension
{
    internal static class ReflectExtension
    {
        /// <summary>
        /// Get List of PropertyInfo of object that contains no DatabaseGeneratedAttribute
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetProperties(this object obj)
        {
            return obj.GetType().GetProperties().Where(f => !Attribute.IsDefined(f, typeof(DatabaseGeneratedAttribute))).ToArray();
        }

        /// <summary>
        /// Get PropertyInfo of object that has KeyAttribute
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <exception cref="DapperExtensionException"></exception>
        public static PropertyInfo GetKeyProperty(this object obj)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties().Where(a => a.GetCustomAttribute<KeyAttribute>() != null).ToArray();

            if (!properties.Any())
                throw new DapperExtensionException($"the {nameof(obj)} entity with no KeyAttribute Propertity");

            if (properties.Length > 1)
                throw new DapperExtensionException($"the {nameof(obj)} entity with greater than one KeyAttribute Propertity");

            return properties.First();
        }

        /// <summary>
        /// Get PropertyInfo that contains KeyAttribute
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        /// <exception cref="DapperExtensionException"></exception>
        public static PropertyInfo GetKeyProperty(this Type typeInfo)
        {
            PropertyInfo[] properties = typeInfo.GetProperties().Where(a => a.GetCustomAttribute<KeyAttribute>() != null).ToArray();

            if (!properties.Any())
                throw new DapperExtensionException($"the type {nameof(typeInfo.FullName)} entity with no KeyAttribute Propertity");

            if (properties.Length > 1)
                throw new DapperExtensionException($"the type {nameof(typeInfo.FullName)} entity with greater than one KeyAttribute Propertity");

            return properties.First();
        }
    }
}
