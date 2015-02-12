using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryComposer.MvcHelper.Model
{
    public abstract class FieldDefinition
    {
        public abstract FieldDefinitionTypes FieldType { get; }

        public abstract string Render();
    }
}
