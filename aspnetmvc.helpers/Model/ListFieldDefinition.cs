using System.Web;
using System.Web.Mvc;
using System.Linq;

namespace QueryComposer.MvcHelper.Model
{
    public class ListFieldDefinition : FieldDefinition
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

        /// <summary>
        /// List of values
        /// </summary>
        /// <remarks>Only if the field type is list</remarks>
        public SelectList Values { get; set; }

        public override FieldDefinitionTypes FieldType
        {
            get { return FieldDefinitionTypes.List; }
        }

        public override string Render()
        {
            var data = string.Join(", ", this.Values.Select(v => "{ text: '" + HttpUtility.JavaScriptStringEncode(v.Text) + "', value: '" + v.Value + "'}"));

            return "new QueryComposer.Model.ListFieldDefinition('" + this.Name + "', '" + HttpUtility.JavaScriptStringEncode(this.Text) + "', [" + data + "])";
        }
    }
}
