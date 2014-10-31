using QueryComposer.MvcHelper.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace QueryComposer.MvcHelper
{
    /// <summary>
    /// IQueryable Helper for the QueryComposer component
    /// </summary>
    public static class QueryableHelper
    {
        /// <summary>
        /// Filter an IQueryable by queries
        /// </summary>
        /// <typeparam name="T">Type of the IQueryable</typeparam>
        /// <param name="query">IQueryable instance</param>
        /// <param name="queries">Queries used to filter the IQueryable</param>
        /// <returns></returns>
        public static IQueryable<T> FilterByQueries<T>(this IQueryable<T> query, IEnumerable<Query> queries)
        {
            if(queries == null)
            {
                return query;
            }

            var param = Expression.Parameter(typeof(T), "p");
            Expression body = null;

            var groupedQueries = GroupByOr(queries.Where(q => !string.IsNullOrEmpty(q.Field) && !string.IsNullOrEmpty(q.Operator)));

            foreach (var group in groupedQueries)
            {
                Expression groupedBody = null;

                foreach (var queryModel in group)
                {
                    MemberExpression property = null;
                    var splittedFields = queryModel.Field.Split('.');
                    foreach(var splittedField in splittedFields)
                    {
                        if(property == null)
                        {
                            property = Expression.Property(param, splittedField);
                        }
                        else
                        {
                            property = Expression.Property(property, splittedField);
                        }
                    }

                    if (!property.Type.IsPrimitive && !property.Type.Equals(typeof(string)))
                    {
                        throw new ArgumentException("The type of the " + queryModel.Field + " must be a simple type.");
                    }

                    ConstantExpression value = null;

                    if (property.Type == typeof(string))
                    {
                        value = Expression.Constant(queryModel.Value);
                    }
                    else
                    {
                        var convertedValue = Convert.ChangeType(queryModel.Value, property.Type);
                        value = Expression.Constant(convertedValue);
                    }

                    var subBody = Expression.Equal(property, value);

                    if (groupedBody != null)
                    {
                        groupedBody = Expression.AndAlso(groupedBody, subBody);
                    }
                    else
                    {
                        groupedBody = subBody;
                    }
                }

                if (body != null)
                {
                    body = Expression.OrElse(body, groupedBody);
                }
                else
                {
                    body = groupedBody;
                }
            }

            if (body != null)
            {
                var subQuery = Expression.Lambda<Func<T, bool>>(body, param);

                query = query.Where(subQuery);
            }

            return query;
        }

        /// <summary>
        /// Group a list of query by OR operator
        /// </summary>
        /// <param name="queries">Queries used to filter the IQueryable</param>
        /// <returns>List of list of queries.</returns>
        private static IEnumerable<IEnumerable<Query>> GroupByOr(IEnumerable<Query> queries)
        {
            var groups = new List<List<Query>>();
            if (!queries.Any())
            {
                return groups;
            }

            var groupQueries = new List<Query>();
            foreach (var query in queries)
            {

                if (query.Operator == "||" && queries.First() != query)
                {
                    groups.Add(groupQueries);
                    groupQueries = new List<Query>();
                }

                groupQueries.Add(query);
            }

            groups.Add(groupQueries);

            return groups;
        }
    }
}
