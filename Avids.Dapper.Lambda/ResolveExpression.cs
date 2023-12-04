using System;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

using Avids.Dapper.Lambda.Exception;
using Avids.Dapper.Lambda.Expressions;
using Avids.Dapper.Lambda.Extension;
using Avids.Dapper.Lambda.Helper;
using Avids.Dapper.Lambda.Model;

namespace Avids.Dapper.Lambda
{
    /// <summary>
    /// Base Class for Expression Resolver
    /// </summary>
    internal class ResolveExpression
    {
        public ProviderOption ProviderOption { get; set; }
        public ResolveExpression(ProviderOption providerOption)
        {
            ProviderOption = providerOption;
        }

        /// <summary>
        /// Resolve OrderBy
        /// </summary>
        /// <param name="orderbyExpressionDic"></param>
        /// <param name="withTableName"></param>
        /// <returns></returns>
        public string ResolveOrderBy(Dictionary<EOrderBy, LambdaExpression> orderbyExpressionDic, bool withTableName = false)
        {
            List<string> orderByList = orderbyExpressionDic.Select(a =>
            {
                MemberInfo member = ((MemberExpression)a.Value.Body).Member;
                string tableName = withTableName ? $"{ProviderOption.CombineFieldName(member.DeclaringType.GetTableAttributeName())}." : "";
                string columnName = ProviderOption.CombineFieldName(member.GetColumnAttributeName());
                return $"{tableName}{columnName} {(a.Key == EOrderBy.Asc ? "ASC" : "DESC")}";
            }).ToList();

            if (!orderByList.Any()) return "";

            string orderBy = string.Join(", ", orderByList);
            return $"ORDER BY {orderBy}";
        }

        /// <summary>
        /// Resolve Where
        /// </summary>
        /// <param name="whereExpressions"></param>
        /// <param name="prefix"></param>
        /// <param name="withTableName"></param>
        /// <returns></returns>
        public WhereExpression ResolveWhere(Queue<Where> whereExpressions, string prefix = null, 
            bool withTableName = false)
        {
            WhereExpression where = new WhereExpression(whereExpressions, prefix, ProviderOption, withTableName);

            return where;
        }

        /// <summary>
        /// Resolve Join
        /// </summary>
        /// <param name="joinExpressions"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public JoinExpression ResolveJoin(Queue<Join> joinExpressions, string prefix = null)
        {
            JoinExpression join = new JoinExpression(joinExpressions, prefix, ProviderOption, true);
            return join;
        }

        //public static UpdateEntityWhereExpression ResolveWhere(LambdaExpression expression)
        //{
        //    var where = new UpdateEntityWhereExpression(expression, ProviderOption);
        //    where.Resolve();
        //    return where;
        //}

        /// <summary>
        /// Helper for Select
        /// </summary>
        /// <param name="a"></param>
        /// <param name="withTableName"></param>
        /// <returns></returns>
        public string Helper(MemberBinding a, bool withTableName)
        {
            string tableName = a.Member.DeclaringType.GetTableAttributeName();
            string table = withTableName ? $"{ProviderOption.CombineFieldName(tableName)}." : "";
            string field = ProviderOption.CombineFieldName(a.Member.GetColumnAttributeName());
            return $"{table}{field}";
        }

        /// <summary>
        /// Resolve Select
        /// </summary>
        /// <param name="selectExpressions"></param>
        /// <param name="isNeedSelect"></param>
        /// <param name="withTableName"></param>
        /// <returns></returns>
        public string ResolveSelect(Queue<Select> selectExpressions = null, 
            bool isNeedSelect = true, bool withTableName = false)
        {
            string selectFormat = isNeedSelect ? "SELECT {0}" : "{0}";
            string selectSql = "";

            if (selectExpressions.Count < 1)
            {
                StringBuilder propertyBuilder = new StringBuilder();
                propertyBuilder.Append("*");
                //foreach (var propertyInfo in propertyInfos)
                //{
                //    if (propertyBuilder.Length > 0)
                //        propertyBuilder.Append(",");
                //    propertyBuilder.AppendFormat($"{ProviderOption.CombineFieldName(propertyInfo.GetColumnAttributeName())} AS {ProviderOption.CombineFieldName(propertyInfo.Name)}");
                //}
                //selectSql = string.Format(selectFormat, propertyBuilder);
                selectSql = string.Format(selectFormat, propertyBuilder);
            }
            else
            {
                List<string> selects = new List<string>();
                
                while (selectExpressions.Count > 0)
                {
                    Select select = selectExpressions.Dequeue();
                    LambdaExpression expr = select.SelectExpression;
                    ExpressionType nodeType = expr.Body.NodeType;
                    if (nodeType == ExpressionType.MemberAccess)
                    {
                        MemberInfo memberAccess = ((MemberExpression)expr.Body).Member;
                        string tableName = memberAccess.DeclaringType.GetTableAttributeName();
                        string table = withTableName ? $"{ProviderOption.CombineFieldName(tableName)}." : "";
                        string columnName = ProviderOption.CombineFieldName(memberAccess.GetColumnAttributeName());
                        //selectSql = string.Format(selectFormat, ProviderOption.CombineFieldName(columnName));
                        selects.Add($"{table}{columnName}");
                    }
                    else if (nodeType == ExpressionType.MemberInit)
                    {
                        MemberInitExpression memberInitExpression = (MemberInitExpression)expr.Body;
                        //selectSql = string.Format(selectFormat, string.Join(",", memberInitExpression.Bindings.Select(a => ProviderOption.CombineFieldName(a.Member.GetColumnAttributeName()))));
                        selects.Add(string.Join(",", memberInitExpression.Bindings.Select(a => Helper(a, withTableName))));
                    }
                    else if (nodeType == ExpressionType.Parameter)
                    {
                        ParameterExpression memberExpression = (ParameterExpression)expr.Body;

                        string table = $"{ProviderOption.CombineFieldName(memberExpression.Type.GetTableAttributeName())}.";
                        string field = $"{table}*";
                        selects.Add(field);
                        //selectSql = string.Format(selectFormat, field);
                    }
                    selectSql = string.Format(selectFormat, string.Join(", ", selects));
                }
            }

            return selectSql;
        }

        /// <summary>
        /// Resolve Select of Update
        /// </summary>
        /// <param name="propertyInfos"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public string ResolveSelectOfUpdate(PropertyInfo[] propertyInfos, LambdaExpression selector)
        {
            string selectSql = "";

            if (selector == null)
            {
                StringBuilder propertyBuilder = new StringBuilder();
                foreach (var propertyInfo in propertyInfos)
                {
                    if (propertyBuilder.Length > 0)
                        propertyBuilder.Append(",");
                    propertyBuilder.AppendFormat($"INSERTED.{ProviderOption.CombineFieldName(propertyInfo.GetColumnAttributeName())} {ProviderOption.CombineFieldName(propertyInfo.Name)}");
                }
                selectSql = propertyBuilder.ToString();
            }
            else
            {
                ExpressionType nodeType = selector.Body.NodeType;
                if (nodeType == ExpressionType.MemberAccess)
                {
                    string columnName = ((MemberExpression)selector.Body).Member.GetColumnAttributeName();
                    selectSql = "INSERTED." + ProviderOption.CombineFieldName(columnName);
                }
                else if (nodeType == ExpressionType.MemberInit)
                {
                    MemberInitExpression memberInitExpression = (MemberInitExpression)selector.Body;
                    selectSql = string.Join(",", memberInitExpression.Bindings.Select(a => "INSERTED." + ProviderOption.CombineFieldName(a.Member.GetColumnAttributeName())));
                }
            }

            return "OUTPUT " + selectSql;
        }

        /// <summary>
        /// Resolve Sum
        /// </summary>
        /// <param name="propertyInfos"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DapperExtensionException"></exception>
        public string ResolveSum(PropertyInfo[] propertyInfos, LambdaExpression selector)
        {
            if (selector == null)
                throw new ArgumentException("selector");
            string selectSql = "";

            ExpressionType nodeType = selector.Body.NodeType;
            switch (nodeType)
            {
                case ExpressionType.MemberAccess:
                    string columnName = ((MemberExpression)selector.Body).Member.GetColumnAttributeName();
                    selectSql = $" SELECT {ProviderOption.FunctionIsNull}(SUM({ProviderOption.CombineFieldName(columnName)}),0)  ";
                    break;
                case ExpressionType.MemberInit:
                    throw new DapperExtensionException("The Expression Type is not supported");
            }

            return selectSql;
        }

        /// <summary>
        /// Resolve Update
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateExpression"></param>
        /// <returns></returns>
        public UpdateExpression ResolveUpdate<T>(Expression<Func<T, T>> updateExpression)
        {
            return new UpdateExpression(updateExpression, ProviderOption);
        }

        /// <summary>
        /// Resolve NO LOCK Expression
        /// </summary>
        /// <param name="nolock">NO LOCK true / false</param>
        /// <returns>Sql string of NOLOCK</returns>
        public virtual string ResolveWithNoLock(bool nolock)
        {
            return nolock ? "NOLOCK" : "";
        }
    }
}
