using QueryComposer.MvcHelper.Model;
using System;
using System.Collections.Generic;
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
            var param = Expression.Parameter(typeof(T), "p");
            Expression body = null;
            foreach (var queryModel in queries)
            {

                var property = Expression.Property(param, queryModel.Field);
                var value = Expression.Constant(queryModel.Value);

                var subBody = Expression.Equal(property, value);

                //Grouper par ||
                switch (queryModel.Operator)
                {
                    case "&&":
                        body = Expression.AndAlso(body, subBody);
                        break;
                    case "||":
                        body = Expression.OrElse(body, subBody);
                        break;
                }
            }

            if (body != null)
            {
                var subQuery = Expression.Lambda<Func<T, bool>>(body, param);

                query = query.Where(subQuery);
            }

            return query;
        }
    }
}
