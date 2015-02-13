namespace QueryComposer.MvcHelper.Model
{
    /// <summary>
    /// Model representing a query
    /// </summary>
    public class Query
    {
        /// <summary>
        /// Type of the query
        /// </summary>
        public FieldDefinitionTypes Type { get; set; }

        /// <summary>
        /// Field used for the query
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Value used for the query
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Operator between this query and the previous one
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// Boolean which indicates if this query is dependant on an other query
        /// </summary>
        public bool IsDependant { get; set; }
    }
}
