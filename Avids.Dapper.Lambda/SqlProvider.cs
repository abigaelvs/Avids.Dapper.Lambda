using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Dapper;

using Avids.Dapper.Lambda.Exception;
using Avids.Dapper.Lambda.Expressions;
using Avids.Dapper.Lambda.Extension;
using Avids.Dapper.Lambda.Helper;
using Avids.Dapper.Lambda.Model;

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
        /// Sql Parameters
        /// </summary>
        public DynamicParameters Params { get; set; }

        /// <summary>
        /// Sql String result
        /// </summary>
        public string SqlString { get; protected set; }

        /// <summary>
        /// Raw Sql String or Unprepared version of Sql String
        /// </summary>
        public string RawSqlString
        {
            get
            {
                string sql = $"{SqlString}";
                IOrderedEnumerable<string> parameters = Params.ParameterNames.OrderByDescending(p => p);
                foreach (string param in parameters)
                {
                    dynamic value = Params.Get<dynamic>(param);
                    string strValue = value is string || value is char || value is DateTime ?
                        $"{ProviderOption.OpenQuote}{value}{ProviderOption.CloseQuote}" : $"{value}";
                    sql = sql.Replace($"@{param}", strValue);
                }
                return sql;
            }
        }

        public SqlProvider()
        {
            Params = new DynamicParameters();
            SetContext = new SetContext();
        }

        public virtual SqlProvider FormatGet<T>()
        {
            string selectSql = ResolveExpression.ResolveSelect(SetContext);

            string fromTableSql = FormatTableName();

            JoinExpression joinParams = ResolveExpression.ResolveJoin(SetContext.JoinExpressions);
            string joinSql = joinParams.SqlCmd;

            string nolockSql = ResolveExpression.ResolveWithNoLock(SetContext.NoLock);

            WhereExpression whereParams = ResolveExpression.ResolveWhere(SetContext.WhereExpressions, withTableName: SetContext.HasJoin);

            string whereSql = whereParams.SqlCmd;

            Params = whereParams.Param;

            string groupBySql = ResolveExpression.ResolveGroupBy(SetContext);

            string orderbySql = ResolveExpression.ResolveOrderBy(SetContext.OrderbyExpressionList, SetContext.HasJoin);

            string limitSql = "LIMIT 1";

            if (!selectSql.Equals(string.Empty)) SqlString += $"{selectSql} ";
            if (!fromTableSql.Equals(string.Empty)) SqlString += $"{fromTableSql} ";
            if (!joinSql.Equals(string.Empty)) SqlString += $"{joinSql} ";
            if (!nolockSql.Equals(string.Empty)) SqlString += $"{nolockSql} ";
            if (!whereSql.Equals(string.Empty)) SqlString += $"{whereSql} ";
            if (!groupBySql.Equals(string.Empty)) SqlString += $"{groupBySql} ";
            if (!orderbySql.Equals(string.Empty)) SqlString += $"{orderbySql} ";
            if (!limitSql.Equals(string.Empty)) SqlString += $"{limitSql} ";

            return this;
        }

        public virtual SqlProvider FormatToList<T>()
        {
            string fromTableSql = FormatTableName();

            string nolockSql = ResolveExpression.ResolveWithNoLock(SetContext.NoLock);

            JoinExpression joinParams = ResolveExpression.ResolveJoin(SetContext.JoinExpressions);
            WhereExpression whereParams = ResolveExpression.ResolveWhere(SetContext.WhereExpressions, withTableName: SetContext.HasJoin);

            string selectSql = ResolveExpression.ResolveSelect(SetContext);

            string whereSql = whereParams.SqlCmd;

            string joinSql = joinParams.SqlCmd;

            Params = whereParams.Param;

            string groupBySql = ResolveExpression.ResolveGroupBy(SetContext);

            string orderbySql = ResolveExpression.ResolveOrderBy(SetContext.OrderbyExpressionList, SetContext.HasJoin);

            int? limitNum = SetContext.LimitNum;
            int? offsetNum = SetContext.OffsetNum;

            string limitSql = limitNum.HasValue ? $"LIMIT {limitNum}" : string.Empty;
            string offsetSql = offsetNum.HasValue ? $"OFFSET {offsetNum}" : string.Empty;

            if (!selectSql.Equals(string.Empty)) SqlString += $"{selectSql} ";
            if (!fromTableSql.Equals(string.Empty)) SqlString += $"{fromTableSql} ";
            if (!joinSql.Equals(string.Empty)) SqlString += $"{joinSql} ";
            if (!nolockSql.Equals(string.Empty)) SqlString += $"{nolockSql} ";
            if (!whereSql.Equals(string.Empty)) SqlString += $"{whereSql} ";
            if (!groupBySql.Equals(string.Empty)) SqlString += $"{groupBySql} ";
            if (!orderbySql.Equals(string.Empty)) SqlString += $"{orderbySql} ";
            if (!limitSql.Equals(string.Empty)) SqlString += $"{limitSql} ";
            if (!offsetSql.Equals(string.Empty)) SqlString += $"{offsetSql} ";

            return this;
        }

        public virtual SqlProvider FormatToPageList<T>(int pageIndex, int pageSize)
        {
            string orderbySql = ResolveExpression.ResolveOrderBy(SetContext.OrderbyExpressionList);

            string groupBySql = ResolveExpression.ResolveGroupBy(SetContext);

            string selectSql = ResolveExpression.ResolveSelect(SetContext);

            string fromTableSql = FormatTableName();

            string nolockSql = ResolveExpression.ResolveWithNoLock(SetContext.NoLock);

            JoinExpression joinParams = ResolveExpression.ResolveJoin(SetContext.JoinExpressions);
            WhereExpression whereParams = ResolveExpression.ResolveWhere(SetContext.WhereExpressions);

            string whereSql = whereParams.SqlCmd;

            string joinSql = joinParams.SqlCmd;

            Params = whereParams.Param;

            string limitSql = $"LIMIT {pageSize}";
            string offsetSql = $"OFFSET {(pageIndex - 1) * pageSize}";

            SqlString += "SELECT COUNT(1) ";
            if (!fromTableSql.Equals(string.Empty)) SqlString += $"{fromTableSql} ";
            if (!nolockSql.Equals(string.Empty)) SqlString += $"{nolockSql} ";
            if (!whereSql.Equals(string.Empty)) SqlString += $"{whereSql}";
            SqlString += ";";

            if (!selectSql.Equals(string.Empty)) SqlString += $"{selectSql} ";
            if (!fromTableSql.Equals(string.Empty)) SqlString += $"{fromTableSql} ";
            if (!joinSql.Equals(string.Empty)) SqlString += $"{joinSql} ";
            if (!nolockSql.Equals(string.Empty)) SqlString += $"{nolockSql} ";
            if (!whereSql.Equals(string.Empty)) SqlString += $"{whereSql} ";
            if (!groupBySql.Equals(string.Empty)) SqlString += $"{groupBySql} ";
            if (!orderbySql.Equals(string.Empty)) SqlString += $"{orderbySql} ";
            if (!limitSql.Equals(string.Empty)) SqlString += $"{limitSql} ";
            if (!offsetSql.Equals(string.Empty)) SqlString += $"{offsetSql} ";

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

            if (!selectSql.Equals(string.Empty)) SqlString += $"{selectSql} ";
            if (!fromTableSql.Equals(string.Empty)) SqlString += $"{fromTableSql} ";
            if (!nolockSql.Equals(string.Empty)) SqlString += $"{nolockSql} ";
            if (!whereSql.Equals(string.Empty)) SqlString += $"{whereSql} ";

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

            string limitSql = "LIMIT 1";

            if (!selectSql.Equals(string.Empty)) SqlString += $"{selectSql} ";
            if (!fromTableSql.Equals(string.Empty)) SqlString += $"{fromTableSql} ";
            if (!nolockSql.Equals(string.Empty)) SqlString += $"{nolockSql} ";
            if (!whereSql.Equals(string.Empty)) SqlString += $"{whereSql} ";
            if (!limitSql.Equals(string.Empty)) SqlString += $"{limitSql} ";

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
            return FormatInsert<T>(a => entity);
        }

        public virtual SqlProvider FormatInsert<T>(Expression<Func<T, T>> insertExpression)
        {
            InsertExpression insert = ResolveExpression.ResolveInsert<T>(insertExpression);
            Params = insert.Param;

            if (SetContext.IfNotExistsExpression == null)
            {
                SqlString = $"INSERT INTO {FormatTableName(false)} {insert.SqlCmd}";
            }
            else
            {
                Where where = new Where();
                where.WhereExpression = SetContext.IfNotExistsExpression;
                SetContext.WhereExpressions.Enqueue(where);
                WhereExpression ifnotexistsWhere = ResolveExpression.ResolveWhere(SetContext.WhereExpressions, "INT_");

                string fields = insert.FieldsStr;
                string values = insert.ValuesStr;
                string sqlTemplate = @"INSERT INTO {0} ({1}) SELECT {2} "
                    + @"WHERE NOT EXISTS(SELECT 1 FROM {0} {3})";
                SqlString = string.Format(sqlTemplate, FormatTableName(false),
                    fields, values, ifnotexistsWhere.SqlCmd);

                Params.AddDynamicParams(ifnotexistsWhere.Param);
            }

            return this;
        }

        public virtual SqlProvider FormatUpdate<T>(T entity)
        {
            return FormatUpdate<T>(a => entity);
        }

        public virtual SqlProvider FormatUpdate<T>(Expression<Func<T, T>> updateExpression)
        {
            UpdateExpression update = ResolveExpression.ResolveUpdate(updateExpression);

            SqlCmdExpression where = null;
            if (SetContext.WhereExpressions.Count > 0)
            {
                where = ResolveExpression.ResolveWhere(SetContext.WhereExpressions);
            }
            else
            {
                where = ResolveExpression.ResolveWhere(updateExpression);
            }

            string whereSql = where.SqlCmd;

            Params = where.Param;
            Params.AddDynamicParams(update.Param);

            SqlString += $"UPDATE {FormatTableName(false)} {update.SqlCmd} ";
            if (!whereSql.Equals(string.Empty)) SqlString += $"{whereSql} ";

            return this;
        }

        public virtual SqlProvider FormatSum<T, TResult>(Expression<Func<T, TResult>> sumExpression)
        {
            LambdaExpression expr = sumExpression as LambdaExpression;
            return FormatSum<T>(expr);
        }

        public virtual SqlProvider FormatSum<T>(LambdaExpression lambdaExpression)
        {
            string selectSql = ResolveExpression.ResolveSum(typeof(T).GetProperties(), lambdaExpression);

            string fromTableSql = FormatTableName();

            string nolockSql = ResolveExpression.ResolveWithNoLock(SetContext.NoLock);

            WhereExpression whereParams = ResolveExpression.ResolveWhere(SetContext.WhereExpressions);

            string whereSql = whereParams.SqlCmd;

            Params = whereParams.Param;

            if (!selectSql.Equals(string.Empty)) SqlString += $"{selectSql} ";
            if (!fromTableSql.Equals(string.Empty)) SqlString += $"{fromTableSql} ";
            if (!nolockSql.Equals(string.Empty)) SqlString += $"{nolockSql} ";
            if (!whereSql.Equals(string.Empty)) SqlString += $"{whereSql} ";

            return this;
        }

        public virtual SqlProvider FormatUpdateSelect<T>(Expression<Func<T, T>> updator)
        {
            string keyField = ProviderOption.CombineFieldName(typeof(T).GetKeyProperty().GetColumnAttributeName());

            UpdateExpression update = ResolveExpression.ResolveUpdate(updator);

            string selectSql = ResolveExpression.ResolveSelect(SetContext, false);

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

        public virtual SqlProvider FormatUpdateSelect<T>(T entity)
        {
            string keyField = ProviderOption.CombineFieldName(typeof(T).GetKeyProperty().GetColumnAttributeName());

            UpdateExpression update = ResolveExpression.ResolveUpdate<T>(a => entity);

            string selectSql = ResolveExpression.ResolveSelect(SetContext, false);

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

            string sql = ProviderOption.CombineFieldName(tableName);
            if (isNeedFrom)
                sql = "FROM " + sql;

            return sql;
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
