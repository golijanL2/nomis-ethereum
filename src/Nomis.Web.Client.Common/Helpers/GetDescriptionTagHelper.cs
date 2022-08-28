using System.ComponentModel;

using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Nomis.Web.Client.Common.Helpers
{
    /// <summary>
    /// Get Description tag helper.
    /// </summary>
    [HtmlTargetElement("td", Attributes = "desc-for")]
    public class GetDescriptionTagHelper : TagHelper
    {
        /// <summary>
        /// For property.
        /// </summary>
        [HtmlAttributeName("desc-for")]
        public ModelExpression? For { get; set; }

        /// <inheritdoc/>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            base.Process(context, output);

            if (For?.Metadata is not DefaultModelMetadata metadata)
            {
                return;
            }

            var attribute = metadata.Attributes.Attributes.OfType<DescriptionAttribute>().FirstOrDefault();
            if (attribute == null)
            {
                return;
            }

            output.Content.SetHtmlContent(attribute.Description);
        }
    }
}