using QueryComposer.MvcHelper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
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
        /// Defines fields available in the query composer, for each query
        /// </summary>
        /// <param name="component">QueryComposer instance</param>
        /// <param name="fields">List of query fields</param>
        /// <returns>The query composer instance</returns>
        public static QueryComposer Fields(this QueryComposer component, List<FieldDefinition> fields)
        {
            component.Fields = fields;

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

            jsBuilder.Append("var fieldsDefinition = [");
            jsBuilder.Append(string.Join(", ", component.Fields.Select(f => f.Render())));
            jsBuilder.AppendLine("];");

            if (component.Queries != null && component.Queries.Any())
            {
                jsBuilder.AppendLine("var data = [");

                foreach (var query in component.Queries)
                {
                    jsBuilder.Append("{");

                    jsBuilder.Append("field: '" + query.Field + "'");
                    jsBuilder.Append(", value: '" + query.Value + "'");
                    jsBuilder.Append(", operator: '" + query.Operator + "'},");
                }

                jsBuilder.AppendLine("];");
            }
            else
            {
                jsBuilder.AppendLine("var data = [];");
            }

            jsBuilder.Append("var vm = new QueryComposer.QueriesViewModel(fieldsDefinition, ");
            jsBuilder.AppendLine("{ showNewEmptyLine: " + component.Configuration.ShowNewEmptyLine.ToString().ToLowerInvariant() + "}");
            jsBuilder.AppendLine(", data);");
            jsBuilder.AppendLine("ko.applyBindings(vm, document.getElementById('" + component.Name + "'));");
            jsBuilder.Append("</script>");

            return MvcHtmlString.Create(container.ToString(TagRenderMode.Normal) + jsBuilder.ToString());
        }
    }
}