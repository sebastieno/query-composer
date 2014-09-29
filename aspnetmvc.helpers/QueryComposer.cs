using QueryComposer.MvcHelper.Model;
using System.Collections.Generic;

namespace QueryComposer.MvcHelper
{
    /// <summary>
    /// Model representing a query composer component
    /// </summary>
    public class QueryComposer
    {
        /// <summary>
        /// Name of the component instance
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Fields of the query composer component
        /// </summary>
        public List<FieldDefinition> Fields { get; internal set; }

        /// <summary>
        /// Current queries of the query composer component
        /// </summary>
        public IEnumerable<Query> Queries { get; internal set; }
    }
}
