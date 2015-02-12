using QueryComposer.MvcHelper.Model;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace QueryComposer.MvcHelper
{
    /// <summary>
    /// Builder for query composer fields 
    /// </summary>
    public class FieldDefinitionBuilder
    {
        private readonly IFieldsContainer fieldsContainer;

        /// <summary>
        /// Constructs an instance of FieldDefinitionBuilder
        /// </summary>
        /// <param name="query">Container on which fields will be added</param>
        public FieldDefinitionBuilder(IFieldsContainer fieldsContainer)
        {
            this.fieldsContainer = fieldsContainer;
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
            this.fieldsContainer.Fields.Add(new TextFieldDefinition { Name = name, Text = text });
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
            this.fieldsContainer.Fields.Add(new ListFieldDefinition { Name = name, Text = text, Values = values });
        }

        /// <summary>
        /// Adds a multiple field, with a text main field
        /// </summary>
        /// <param name="name">Name of the field</param>
        /// <param name="text">Text of the field</param>
        /// <param name="fieldsBuilder">Builder for children fields</param>
        public void AddMultipleField(string name, string text, Action<FieldDefinitionBuilder> fieldsBuilder)
        {
            var field = new MultipleFieldDefinition
            {
                MainField = new TextFieldDefinition { Name = name, Text = text },
                Fields = new List<FieldDefinition>()
            };

            this.fieldsContainer.Fields.Add(field);

            fieldsBuilder(new FieldDefinitionBuilder(field));
        }

        /// <summary>
        /// Adds a multiple field, with a text main field
        /// </summary>
        /// <param name="name">Name of the field</param>
        /// <param name="text">Text of the field</param>
        /// <param name="values">List of values available for the main field</param>
        /// <param name="fieldsBuilder">Builder for children fields</param>
        public void AddMultipleField(string name, string text, SelectList values, Action<FieldDefinitionBuilder> fieldsBuilder)
        {
            var field = new MultipleFieldDefinition
            {
                MainField = new ListFieldDefinition { Name = name, Text = text, Values = values },
                Fields = new List<FieldDefinition>()
            };

            this.fieldsContainer.Fields.Add(field);

            fieldsBuilder(new FieldDefinitionBuilder(field));
        }
    }
}
