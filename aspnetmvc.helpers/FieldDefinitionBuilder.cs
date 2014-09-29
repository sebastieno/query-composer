using QueryComposer.MvcHelper.Model;
using System.Web.Mvc;

namespace QueryComposer.MvcHelper
{
    /// <summary>
    /// Builder for query composer fields 
    /// </summary>
    public class FieldDefinitionBuilder
    {
        private readonly QueryComposer query;

        /// <summary>
        /// Constructs an instance of FieldDefinitionBuilder
        /// </summary>
        /// <param name="query">QueryComposer on which the builder will construct fields</param>
        public FieldDefinitionBuilder(QueryComposer query)
        {
            this.query = query;
        }

        /// <summary>
        /// Adds a text field
        /// </summary>
        /// <param name="name">Name of the field</param>
        public void AddTextField(string name)
        {
            this.AddTextField(name, name);
        }

        /// <summary>
        /// Adds a text field
        /// </summary>
        /// <param name="name">Name of the field</param>
        /// <param name="text">Text of the field</param>
        public void AddTextField(string name, string text)
        {
            this.query.Fields.Add(new FieldDefinition { Name = name, Text = text, Type = FieldDefinition.Types.Text });
        }

        /// <summary>
        /// Adds a list field
        /// </summary>
        /// <param name="name">Name of the field</param>
        /// <param name="values">List of values available for this field</param>
        public void AddListField(string name, SelectList values)
        {
            this.AddListField(name, name, values);
        }

        /// <summary>
        /// Adds a list field
        /// </summary>
        /// <param name="name">Name of the field</param>
        /// <param name="text">Text of the field</param>
        /// <param name="values">List of values available for this field</param>
        public void AddListField(string name, string text, SelectList values)
        {
            this.query.Fields.Add(new FieldDefinition { Name = name, Text = text, Type = FieldDefinition.Types.List, Values = values });
        }
    }
}
