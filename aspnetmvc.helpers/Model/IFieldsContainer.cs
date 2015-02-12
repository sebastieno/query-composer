using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryComposer.MvcHelper.Model
{
    public interface IFieldsContainer
    {
        List<FieldDefinition> Fields { get; }
    }
}
