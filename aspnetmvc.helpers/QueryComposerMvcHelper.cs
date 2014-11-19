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
        public static QueryComposer QueryComposer(this HtmlHelper helper, string name)
        {
            return new QueryComposer { Name = name };
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
            container.AddCssClass("query-component");
            container.Attributes.Add("data-querycomponent-name", component.Name);

            var mainDiv = new TagBuilder("div");
            mainDiv.Attributes.Add("data-bind", "template : { name: \'queryComponentTemplate\' }");

            StringBuilder jsBuilder = new StringBuilder();
            jsBuilder.AppendLine("<script type='text/javascript'>");
            jsBuilder.Append("var vm = new QueryComponent.QueriesViewModel([");
            foreach (var field in component.Fields)
            {
                jsBuilder.Append("{");

                jsBuilder.Append("name: '" + field.Name + "'");
                jsBuilder.Append(", text: '" + field.Text + "'");
                jsBuilder.Append(", type: " + (field.Type == FieldDefinition.Types.Text ? "0" : "1"));

                if (field.Type == FieldDefinition.Types.List)
                {
                    jsBuilder.Append(", values: [");

                    jsBuilder.Append(string.Join(",", field.Values.Select(v => "{value: " + v.Value + ", text:'" + v.Text + "'}")));

                    jsBuilder.Append("]");
                }

                if (component.Fields.Last() == field)
                {
                    jsBuilder.Append("}");
                }
                else
                {
                    jsBuilder.Append("},");
                }
            }

            if (component.Queries != null && component.Queries.Any())
            {
                jsBuilder.AppendLine("],");
                jsBuilder.Append("[");

                foreach (var query in component.Queries)
                {
                    jsBuilder.Append("{");

                    jsBuilder.Append("field: '" + query.Field + "'");
                    jsBuilder.Append(", value: '" + query.Value + "'");
                    jsBuilder.Append(", operator: '" + query.Operator + "'");

                    if (component.Queries.Last() == query)
                    {
                        jsBuilder.Append("}");
                    }
                    else
                    {
                        jsBuilder.Append("},");
                    }
                }
            }

            jsBuilder.AppendLine("]);");
            jsBuilder.AppendLine("ko.applyBindings(vm, $(\"[data-querycomponent-name='" + component.Name + "'\")[0]);");
            jsBuilder.Append("</script>");

            container.InnerHtml = mainDiv.ToString(TagRenderMode.Normal) + jsBuilder.ToString();

            return MvcHtmlString.Create(container.ToString(TagRenderMode.Normal));
        }
    }
}
