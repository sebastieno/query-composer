using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QueryComposer.MvcHelper.Model
{
    public class TextFieldDefinition : FieldDefinition
    {
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

        public override FieldDefinitionTypes FieldType
        {
            get { return FieldDefinitionTypes.Text; }
        }

        public override string Render()
        {
            return "new QueryComposer.Model.TextFieldDefinition('" + this.Name + "', '" + HttpUtility.JavaScriptStringEncode(this.Text) + "')";
        }
    }
}
