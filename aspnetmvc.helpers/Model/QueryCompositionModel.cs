using System.Collections.Generic;

namespace QueryComposer.MvcHelper.Model
{
    /// <summary>
    /// Model representing a query composition
    /// </summary>
    public class QueryCompositionModel
    {
        /// <summary>
        /// List of queries
        /// </summary>
        public IEnumerable<Query> Queries { get; set; }
    }
}
