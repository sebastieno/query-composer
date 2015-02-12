using System.Collections.Generic;
using System.Linq;
namespace QueryComposer.MvcHelper.Model
{
    public class MultipleFieldDefinition : FieldDefinition, IFieldsContainer
    {
        /// <summary>
        /// Represents the main field
        /// </summary>
        public FieldDefinition MainField { get; set; }

        /// <summary>
        /// List of children fields
        /// </summary>
        public List<FieldDefinition> Fields { get; set; }

        public override FieldDefinitionTypes FieldType
        {
            get { return FieldDefinitionTypes.Multiple; }
        }

        public override string Render()
        {
            var mainFieldRendered = this.MainField.Render();
            var childrenFieldsRendered = string.Join(", ", this.Fields.Select(c => c.Render()));

            return "new QueryComposer.Model.MultipleFieldsDefinition(" + mainFieldRendered + ", [" + childrenFieldsRendered + "])";
        }
    }
}
