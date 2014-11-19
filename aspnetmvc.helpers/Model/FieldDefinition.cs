using System.Collections;
using System.Web.Mvc;

namespace QueryComposer.MvcHelper.Model
{
    /// <summary>
    /// Model representing a field definition
    /// </summary>
    public class FieldDefinition
    {
        /// <summary>
        /// Type of the field
        /// </summary>
        public enum Types
        {
            /// <summary>
            /// Text field
            /// </summary>
            /// <remarks>With this type of field, the user must enter a text</remarks>
            Text,

            /// <summary>
            /// List field
            /// </summary>
            /// <remarks>With this type of field, the user must select a value from a list</remarks>
            List,

            /// <summary>
            /// Multiple field
            /// </summary>
            /// <remarks>This type represents an aggregation of many fields.</remarks>
            Multiple
        }

        /// <summary>
        /// Text of the field
        /// </summary>
        /// <remarks>The text is used for the display</remarks>
        public string Text { get; set; }

        /// <summary>
        /// Name of the field
        /// </summary>
        /// <remarks>The name is used to identify the field</remarks>
        public string Name { get; set; }

        /// <summary>
        /// Type of the field
        /// </summary>
        public Types Type { get; set; }

        /// <summary>
        /// List of values
        /// </summary>
        /// <remarks>Only if the field type is list</remarks>
        public SelectList Values { get; set; }
    }
}
