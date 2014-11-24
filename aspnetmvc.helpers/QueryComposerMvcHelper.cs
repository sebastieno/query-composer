using QueryComposer.MvcHelper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QueryComposer.MvcHelper
{
    /// <summary>
    /// Mvc Helper for the QueryComposer component
    /// </summary>
    public static class QueryComposerMvcHelper
    {
        /// <summary>
        /// Constructs a new query composer instance
        /// </summary>
        /// <param name="helper">HtmlHelper instance</param>
        /// <param name="name">Name of the component</param>
        /// <returns>The query composer instance</returns>
        public static QueryComposer QueryComposer(this HtmlHelper helper, string name, QueryComposerConfiguration configuration = null)
        {
            return new QueryComposer { Name = name, Fields = new List<FieldDefinition>(), Configuration = configuration ?? new QueryComposerConfiguration() };
        }

        /// <summary>
        /// Defines fields available in the query composer, for each query
        /// </summary>
        /// <param name="component">QueryComposer instance</param>
        /// <param name="fieldsBuilder">Builder used to define the fields</param>
        /// <returns>The query composer instance</returns>
        public static QueryComposer Fields(this QueryComposer component, Action<FieldDefinitionBuilder> fieldsBuilder)
        {
            fieldsBuilder(new FieldDefinitionBuilder(component));

            return component;
        }

        /// <summary>
        /// Defines current queries of the query composer
        /// </summary>
        /// <param name="component">QueryComposer instance</param>
        /// <param name="queries">List of queries</param>
        /// <returns>The query composer instance</returns>
        public static QueryComposer Data(this QueryComposer component, IEnumerable<Query> queries)
        {
            component.Queries = queries;
            return component;
        }

        /// <summary>
        /// Render the query composer into a html string
        /// </summary>
        /// <param name="component">QueryComposer instance</param>
        /// <returns>String containing the html of the query composer</returns>
        public static MvcHtmlString Render(this QueryComposer component)
        {
            var container = new TagBuilder("div");
            container.AddCssClass("query-composer");
            container.Attributes.Add("id", component.Name);
            container.Attributes.Add("data-bind", "template : { name: \'queryComposerTemplate\' }");

            StringBuilder jsBuilder = new StringBuilder();
            jsBuilder.AppendLine("<script type='text/javascript'>");

            jsBuilder.AppendLine("var fieldsDefinition = [");
            foreach (var field in component.Fields)
            {
                if (field.Type == FieldDefinition.Types.Text)
                {
                    jsBuilder.AppendLine("new QueryComposer.Model.TextFieldDefinition('" + field.Name + "', '" + field.Text + "'),");
                }
                else if (field.Type == FieldDefinition.Types.List)
                {
                    var data = string.Join(", ", field.Values.Select(v => "{ text: '" + v.Text + "', value: '" + v.Value + "'}"));

                    jsBuilder.AppendLine("new QueryComposer.Model.ListFieldDefinition('" + field.Name + "', '" + field.Text + "', [" + data + "]),");
                }
            }

            jsBuilder.AppendLine("];");

            //if (component.Queries != null && component.Queries.Any())
            //{
            //    jsBuilder.AppendLine("],");
            //    jsBuilder.Append("[");

            //    foreach (var query in component.Queries)
            //    {
            //        jsBuilder.Append("{");

            //        jsBuilder.Append("field: '" + query.Field + "'");
            //        jsBuilder.Append(", value: '" + query.Value + "'");
            //        jsBuilder.Append(", operator: '" + query.Operator + "'");

            //        if (component.Queries.Last() == query)
            //        {
            //            jsBuilder.Append("}");
            //        }
            //        else
            //        {
            //            jsBuilder.Append("},");
            //        }
            //    }
            //}

            jsBuilder.Append("var vm = new QueryComposer.QueriesViewModel(fieldsDefinition, ");
            jsBuilder.AppendLine("{ showNewEmptyLine: " + component.Configuration.ShowNewEmptyLine.ToString().ToLowerInvariant() + "}");
            jsBuilder.AppendLine(");");
            jsBuilder.AppendLine("ko.applyBindings(vm, document.getElementById('" + component.Name + "')[0]);");
            jsBuilder.Append("</script>");

            return MvcHtmlString.Create(container.ToString(TagRenderMode.Normal) + jsBuilder.ToString());
        }
    }
}
