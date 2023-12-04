using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using Dapper;

using Avids.Dapper.Lambda.Exception;
using Avids.Dapper.Lambda.Expressions;
using Avids.Dapper.Lambda.Extension;
using Avids.Dapper.Lambda.Helper;
using Avids.Dapper.Lambda.Model;
using System.Collections.Generic;
using System;

namespace Avids.Dapper.Lambda
{
    /// <summary>
    /// Provider for Sql Database
    /// </summary>
    public class SqlProvider
    {
        /// <summary>
        /// Context for database
        /// </summary>
        internal SetContext SetContext { get; set; }

        /// <summary>
        /// Expression Resolver
        /// </summary>
        internal ResolveExpression ResolveExpression { get; set; }

        /// <summary>
        /// Option for each Database Provider
        /// </summary>
        protected ProviderOption ProviderOption { get; set; }

        /// <summary>
        /// Sql String result
        /// </summary>
        public string SqlString { get; set; }

        /// <summary>
        /// Sql Parameters
        /// </summary>
        public DynamicParameters Params { get; set; }

        public SqlProvider()
        {
            Params = new DynamicParameters();
            SetContext = new SetContext();
        }

        public virtual SqlProvider FormatGet<T>()
        {
            string selectSql = ResolveExpression.ResolveSelect(SetContext.SelectExpressions);

            string fromSql = FormatTableName();

            JoinExpression joinParams = ResolveExpression.ResolveJoin(SetContext.JoinExpressions);
            string joinSql = joinParams.SqlCmd;

            string noLockSql = ResolveExpression.ResolveWithNoLock(SetContext.NoLock);

            WhereExpression whereParams = ResolveExpression.ResolveWhere(SetContext.WhereExpressions);

            string whereSql = whereParams.SqlCmd;

            Params = whereParams.Param;

            string orderBySql = ResolveExpression.ResolveOrderBy(SetContext.OrderbyExpressionList);

            SqlString = $"{selectSql} {fromSql} {joinSql} {noLockSql} {whereSql} {orderBySql} LIMIT 1";

            return this;
        }

        public virtual SqlProvider FormatToList<T>()
        {
            string fromTableSql = FormatTableName();

            string nolockSql = ResolveExpression.ResolveWithNoLock(SetContext.NoLock);

            bool withTableName = SetContext.JoinExpressions.Count > 0;
            JoinExpression joinParams = ResolveExpression.ResolveJoin(SetContext.JoinExpressions);
            WhereExpression whereParams = ResolveExpression.ResolveWhere(SetContext.WhereExpressions, withTableName: withTableName);

            string selectSql = ResolveExpression.ResolveSelect(SetContext.SelectExpressions, withTableName: withTableName);

            string whereSql = whereParams.SqlCmd;

            string joinSql = joinParams.SqlCmd;

            Params = whereParams.Param;

            string orderbySql = ResolveExpression.ResolveOrderBy(SetContext.OrderbyExpressionList, withTableName);

            int? limitNum = SetContext.LimitNum;
            int? offsetNum = SetContext.OffsetNum;

            string limitSql = limitNum.HasValue ? $"LIMIT {limitNum}" : "";
            string offsetSql = offsetNum.HasValue ? $"OFFSET {offsetNum}" : "";

            SqlString = $"{selectSql} {fromTableSql} {joinSql} {nolockSql} {whereSql} {orderbySql} {limitSql} {offsetSql}";

            return this;
        }

        public virtual SqlProvider FormatToPageList<T>(int pageIndex, int pageSize)
        {
            string orderbySql = ResolveExpression.ResolveOrderBy(SetContext.OrderbyExpressionList);
            if (string.IsNullOrEmpty(orderbySql))
                throw new DapperExtensionException("order by takes precedence over pagelist");

            string selectSql = ResolveExpression.ResolveSelect(SetContext.SelectExpressions);

            string fromTableSql = FormatTableName();

            string nolockSql = ResolveExpression.ResolveWithNoLock(SetContext.NoLock);

            WhereExpression whereParams = ResolveExpression.ResolveWhere(SetContext.WhereExpressions);

            string whereSql = whereParams.SqlCmd;

            Params = whereParams.Param;

            SqlString = $"SELECT COUNT(1) {fromTableSql} {nolockSql} {whereSql};";
            SqlString += $"{selectSql} {fromTableSql} {nolockSql} {whereSql} {orderbySql} LIMIT {pageSize} OFFSET  {(pageIndex - 1) * pageSize}";

            return this;
        }

        public virtual SqlProvider FormatCount()
        {
            string selectSql = "SELECT COUNT(*)";

            string fromTableSql = FormatTableName();

            string nolockSql = ResolveExpression.ResolveWithNoLock(SetContext.NoLock);

            WhereExpression whereParams = ResolveExpression.ResolveWhere(SetContext.WhereExpressions);

            string whereSql = whereParams.SqlCmd;

            Params = whereParams.Param;

            SqlString = $"{selectSql} {fromTableSql} {nolockSql} {whereSql} ";

            return this;
        }

        public virtual SqlProvider FormatExists()
        {
            string selectSql = "SELECT 1";

            string fromTableSql = FormatTableName();

            string nolockSql = ResolveExpression.ResolveWithNoLock(SetContext.NoLock);

            WhereExpression whereParams = ResolveExpression.ResolveWhere(SetContext.WhereExpressions);

            string whereSql = whereParams.SqlCmd;

            Params = whereParams.Param;

            SqlString = $"{selectSql} {fromTableSql} {nolockSql} {whereSql} LIMIT 1";

            return this;
        }

        public virtual SqlProvider FormatDelete()
        {
            string fromTableSql = FormatTableName();

            WhereExpression whereParams = ResolveExpression.ResolveWhere(SetContext.WhereExpressions);

            string whereSql = whereParams.SqlCmd;

            Params = whereParams.Param;

            SqlString = $"DELETE {fromTableSql} {whereSql}";

            return this;
        }

        public virtual SqlProvider FormatInsert<T>(T entity)
        {
            string[] paramsAndValuesSql = FormatInsertParamsAndValues(entity);

            if (SetContext.IfNotExistsExpression == null)
                SqlString = $"INSERT INTO {FormatTableName(false)} ({paramsAndValuesSql[0]}) VALUES ({paramsAndValuesSql[1]})";
            else
            {
                Where where = new Where();
                where.WhereExpression = SetContext.IfNotExistsExpression;
                SetContext.WhereExpressions.Enqueue(where);
                WhereExpression ifnotexistsWhere = ResolveExpression.ResolveWhere(SetContext.WhereExpressions, "INT_");

                SqlString = string.Format(@"INSERT INTO {0}({1})  
                SELECT {2}
                WHERE NOT EXISTS(
                    SELECT 1
                    FROM {0}  
                {3}
                    ); ", FormatTableName(false), paramsAndValuesSql[0], paramsAndValuesSql[1], ifnotexistsWhere.SqlCmd);

                Params.AddDynamicParams(ifnotexistsWhere.Param);
            }

            return this;
        }
        public virtual SqlProvider FormatInsert(Dictionary<string, object> entity)
        {
            string[] paramsAndValuesSql = FormatInsertParamsAndValues(entity);

            if (SetContext.IfNotExistsExpression == null)
                SqlString = $"INSERT INTO {FormatTableName(false)} ({paramsAndValuesSql[0]}) VALUES ({paramsAndValuesSql[1]})";
            else
            {
                Where where = new Where();
                where.WhereExpression = SetContext.IfNotExistsExpression;
                SetContext.WhereExpressions.Enqueue(where);
                WhereExpression ifnotexistsWhere = ResolveExpression.ResolveWhere(SetContext.WhereExpressions, "INT_");

                SqlString = string.Format(@"INSERT INTO {0}({1})  
                SELECT {2}
                WHERE NOT EXISTS(
                    SELECT 1
                    FROM {0}  
                {3}
                    ); ", FormatTableName(false), paramsAndValuesSql[0], paramsAndValuesSql[1], ifnotexistsWhere.SqlCmd);

                Params.AddDynamicParams(ifnotexistsWhere.Param);
            }

            return this;
        }

        public virtual SqlProvider FormatUpdate<T>(Expression<Func<T, T>> updateExpression)
        {
            UpdateExpression update = ResolveExpression.ResolveUpdate(updateExpression);

            WhereExpression where = ResolveExpression.ResolveWhere(SetContext.WhereExpressions);

            string whereSql = where.SqlCmd;

            Params = where.Param;
            Params.AddDynamicParams(update.Param);

            SqlString = $"UPDATE {FormatTableName(false)} {update.SqlCmd} {whereSql}";

            return this;
        }

        public virtual SqlProvider FormatUpdate<T>(T entity)
        {
            UpdateExpression update = ResolveExpression.ResolveUpdate<T>(a => entity);

            WhereExpression where = ResolveExpression.ResolveWhere(SetContext.WhereExpressions);

            string whereSql = where.SqlCmd;

            Params = where.Param;
            Params.AddDynamicParams(update.Param);

            SqlString = $"UPDATE {FormatTableName(false)} {update.SqlCmd} {whereSql}";

            return this;
        }

        public virtual SqlProvider FormatSum<T>(LambdaExpression lambdaExpression)
        {
            string selectSql = ResolveExpression.ResolveSum(typeof(T).GetProperties(), lambdaExpression);

            string fromTableSql = FormatTableName();

            string nolockSql = ResolveExpression.ResolveWithNoLock(SetContext.NoLock);

            WhereExpression whereParams = ResolveExpression.ResolveWhere(SetContext.WhereExpressions);

            string whereSql = whereParams.SqlCmd;

            Params = whereParams.Param;

            SqlString = $"{selectSql} {fromTableSql} {nolockSql} {whereSql} ";

            return this;
        }

        public virtual SqlProvider FormatUpdateSelect<T>(Expression<Func<T, T>> updator)
        {
            string keyField = ProviderOption.CombineFieldName(typeof(T).GetKeyProperty().GetColumnAttributeName());

            UpdateExpression update = ResolveExpression.ResolveUpdate(updator);

            string selectSql = ResolveExpression.ResolveSelect(SetContext.SelectExpressions, false);

            WhereExpression where = ResolveExpression.ResolveWhere(SetContext.WhereExpressions);

            string whereSql = where.SqlCmd;

            Params = where.Param;
            Params.AddDynamicParams(update.Param);

            int? topNum = SetContext.LimitNum;

            string limitSql = topNum.HasValue ? " LIMIT " + topNum.Value : "";
            string tableName = FormatTableName(false);

            SqlString = $"UPDATE {tableName} {update.SqlCmd} WHERE {keyField} IN (SELECT {keyField} FROM {tableName} {whereSql} {limitSql} FOR UPDATE SKIP LOCKED) RETURNING {selectSql}";

            return this;
        }

        //public abstract SqlProvider ExcuteBulkCopy<T>(IDbConnection conn, IEnumerable<T> list);

        protected string FormatTableName(bool isNeedFrom = true)
        {
            Type typeOfTableClass = SetContext.TableType;

            string className = typeOfTableClass.GetTableAttributeName();
            string tableName = className;

            SqlString = ProviderOption.CombineFieldName(tableName);
            if (isNeedFrom)
                SqlString = "FROM " + SqlString;

            return SqlString;
        }

        protected string[] FormatInsertParamsAndValues<T>(T entity)
        {
            StringBuilder paramSqlBuilder = new StringBuilder(64);
            StringBuilder valueSqlBuilder = new StringBuilder(64);

            PropertyInfo[] properties = entity.GetProperties();

            bool isAppend = false;
            foreach (PropertyInfo propertiy in properties)
            {
                if (isAppend)
                {
                    paramSqlBuilder.Append(",");
                    valueSqlBuilder.Append(",");
                }

                string columnName = propertiy.GetColumnAttributeName();
                string paramterName = ProviderOption.ParameterPrefix + columnName;
                paramSqlBuilder.Append(ProviderOption.CombineFieldName(columnName));
                valueSqlBuilder.Append(paramterName);

                Params.Add(paramterName, propertiy.GetValue(entity));

                isAppend = true;
            }

            return new[] { paramSqlBuilder.ToString(), valueSqlBuilder.ToString() };
        }

        protected string[] FormatInsertParamsAndValues(Dictionary<string, object> entity)
        {
            StringBuilder paramSqlBuilder = new StringBuilder(64);
            StringBuilder valueSqlBuilder = new StringBuilder(64);

            PropertyInfo[] properties = entity.GetProperties();

            bool isAppend = false;
            foreach (PropertyInfo propertiy in properties)
            {
                if (isAppend)
                {
                    paramSqlBuilder.Append(",");
                    valueSqlBuilder.Append(",");
                }

                string columnName = propertiy.GetColumnAttributeName();
                string paramterName = ProviderOption.ParameterPrefix + columnName;
                paramSqlBuilder.Append(ProviderOption.CombineFieldName(columnName));
                valueSqlBuilder.Append(paramterName);

                Params.Add(paramterName, propertiy.GetValue(entity));

                isAppend = true;
            }

            return new[] { paramSqlBuilder.ToString(), valueSqlBuilder.ToString() };
        }

        public void FormatProperty<T>()
        {
            SqlMapper.ITypeMap typeMap = SqlMapper.GetTypeMap(typeof(T));
            if (typeMap is DefaultTypeMap)
            {
                CustomPropertyTypeMap map = new CustomPropertyTypeMap(
                    typeof(T), (type, columnName) => SnakeCaseMapper(type, columnName));
                SqlMapper.SetTypeMap(typeof(T), map);
            }
        }

        protected Func<Type, string, PropertyInfo> SnakeCaseMapper = 
            new Func<Type, string, PropertyInfo>((type, columnName) =>
            {
            return type.GetProperty(CustomPropertyHelper
                .ConvertSnakeCaseToPascalCase(columnName));
            });
    }
}
