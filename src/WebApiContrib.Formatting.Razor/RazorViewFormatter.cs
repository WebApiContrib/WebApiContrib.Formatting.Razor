using WebApiContrib.Formatting.Html;
using WebApiContrib.Formatting.Html.Formatting;

namespace WebApiContrib.Formatting.Razor
{
    /// <summary>
    /// <see cref="HtmlMediaTypeFormatter"/> using the Razor syntax.
    /// </summary>
    public sealed class RazorViewFormatter : HtmlMediaTypeViewFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RazorViewFormatter"/>.
        /// </summary>
        /// <param name="siteRootPath">The root path containing view files. This defaults to "~/Views".</param>
        /// <param name="viewLocator">The <see cref="IViewLocator"/> instance used to locate the correct view. This defaults to <see cref="RazorViewLocator"/>.</param>
        /// <param name="viewParser">The <see cref="IViewParser"/> instance used to parse the view. This defaults to <see cref="RazorViewParser"/>.</param>
        public RazorViewFormatter(string siteRootPath = null, IViewLocator viewLocator = null, IViewParser viewParser = null)
            : base(siteRootPath, viewLocator ?? new RazorViewLocator(), viewParser ?? new RazorViewParser())
        {
        }
    }
}
