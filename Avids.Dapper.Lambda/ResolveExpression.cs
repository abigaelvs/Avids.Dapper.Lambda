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
        /// Resolve Group By
        /// </summary>
        /// <param name="orderbyExpressionDic"></param>
        /// <param name="withTableName"></param>
        /// <returns></returns>
        public string ResolveGroupBy(SetContext context)
        {
            Console.WriteLine("=== group by");
            List<string> groupByList = context.GroupByExpressionList.Select(a =>
            {
                MemberInfo member = ((MemberExpression)a.Body).Member;
                string tableName = context.HasJoin ? $"{ProviderOption.CombineFieldName(member.DeclaringType.GetTableAttributeName())}." : "";
                string columnName = ProviderOption.CombineFieldName(member.GetColumnAttributeName());
                return $"{tableName}{columnName}";
            }).ToList();

            if (!groupByList.Any()) return "";

            string orderBy = string.Join(", ", groupByList);
            return $"GROUP BY {orderBy}";
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

        public UpdateEntityWhereExpression ResolveWhere<T>(Expression<Func<T, T>> updateExpression)
        {
            UpdateEntityWhereExpression where = new UpdateEntityWhereExpression(updateExpression, ProviderOption);

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

        public string GetSelectAsName(SetContext context, MemberBinding a)
        {
            string expr = a.GetType().GetProperty("Expression").GetValue(a).ToString();
            string propName = expr.Split('.').ToList().ElementAt(1);
            return context.TableType.GetProperty(propName).GetColumnAttributeName();
        }

        /// <summary>
        /// Helper for Select
        /// </summary>
        /// <param name="a"></param>
        /// <param name="withTableName"></param>
        /// <returns></returns>
        public string ResolveSelectHelper(SetContext context, MemberBinding a)
        {
            string selectAs = GetSelectAsName(context, a);

            string tableName = a.Member.DeclaringType.GetTableAttributeName();
            string table = context.HasJoin ? $"{ProviderOption.CombineFieldName(tableName)}." : "";
            string column = a.Member.GetColumnAttributeName();
            string field = ProviderOption.CombineFieldName(column);
            string fieldAs = context.HasJoin || column != selectAs ? 
                $" AS {ProviderOption.CombineFieldName(selectAs)}" : "";

            string result = $"{table}{field}{fieldAs}";
            return result;
        }

        /// <summary>
        /// Resolve Select
        /// </summary>
        /// <param name="selectExpressions"></param>
        /// <param name="isNeedSelect"></param>
        /// <param name="withTableName"></param>
        /// <returns></returns>
        public string ResolveSelect(SetContext context, bool isNeedSelect = true)
        {
            string statement = "";
            if (isNeedSelect) statement += "SELECT ";
            if (context.Distinct) statement += "DISTINCT";
            string selectFormat = isNeedSelect ? $"{statement}" + " {0}" : "{0}";
            string selectSql = "";

            if (context.SelectExpressions.Count < 1)
            {
                StringBuilder propertyBuilder = new StringBuilder();
                propertyBuilder.Append("*");
                selectSql = string.Format(selectFormat, propertyBuilder);
                return selectSql;
            }
         
            List<string> selects = new List<string>();
                
            while (context.SelectExpressions.Count > 0)
            {
                Select select = context.SelectExpressions.Dequeue();
                LambdaExpression expr = select.SelectExpression;
                ExpressionType nodeType = expr.Body.NodeType;
                if (nodeType == ExpressionType.MemberAccess)
                {
                    MemberInfo memberAccess = ((MemberExpression)expr.Body).Member;
                    string tableName = memberAccess.DeclaringType.GetTableAttributeName();
                    string table = context.HasJoin ? $"{ProviderOption.CombineFieldName(tableName)}." : "";
                    string columnName = ProviderOption.CombineFieldName(memberAccess.GetColumnAttributeName());
                    selects.Add($"{table}{columnName}");
                }
                else if (nodeType == ExpressionType.MemberInit)
                {
                    MemberInitExpression memberInitExpression = (MemberInitExpression)expr.Body;
                    selects.Add(string.Join(", ", memberInitExpression.Bindings.Select(a =>
                        ResolveSelectHelper(context, a))));
                }
                else if (nodeType == ExpressionType.Parameter)
                {
                    ParameterExpression memberExpression = (ParameterExpression)expr.Body;

                    string table = $"{ProviderOption.CombineFieldName(memberExpression.Type.GetTableAttributeName())}.";
                    string field = $"{table}*";
                    selects.Add(field);
                }
                selectSql = string.Format(selectFormat, string.Join(", ", selects));
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

        public InsertExpression ResolveInsert<T>(Expression<Func<T, T>> insertExpression)
        {
            return new InsertExpression(insertExpression, ProviderOption);
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
