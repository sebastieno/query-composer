using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryComposer.MvcHelper.Model
{
    public enum FieldDefinitionTypes
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
        /// <remarks>With this type of field, the user must select a value from a list</remarks>
        Multiple
    }
}
