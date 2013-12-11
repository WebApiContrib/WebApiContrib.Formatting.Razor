using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using RazorEngine.Text;

namespace WebApiContrib.Formatting.Razor
{
    /// <summary>
    /// A <see cref="TemplateBase"/> class for use when rendering HTML with Razor in Web API applications.
    /// </summary>
    /// <typeparam name="T">The Model type.</typeparam>
    [RequireNamespaces("System.Web.Mvc.Html")]
    public abstract class HtmlTemplateBase<T> : TemplateBase<T>, IViewDataContainer
    {
        private HtmlHelper<T> _htmlHelper;
        private System.Web.Http.Routing.UrlHelper _urlHelper;
        private ViewDataDictionary _viewData;

        /// <summary>
        /// Returns a <see cref="System.Web.Mvc.HtmlHelper{T}"/> based on the current Model type.
        /// </summary>
        public HtmlHelper<T> Html
        {
            get
            {
                if (_htmlHelper == null)
                {
                    var writer = this.CurrentWriter;
                    var viewContext = new ViewContext { Writer = writer, ViewData = ViewData };
                    _htmlHelper = new HtmlHelper<T>(viewContext, this);
                }

                return _htmlHelper;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.Web.Http.Routing.UrlHelper"/>.
        /// </summary>
        /// <remarks>
        /// This is not currently tied to the current <see cref="System.Net.Http.HttpRequestMessage"/> but should be.
        /// </remarks>
        public System.Web.Http.Routing.UrlHelper Url
        {
            get
            {
                if (_urlHelper == null)
                    _urlHelper = new System.Web.Http.Routing.UrlHelper();

                return _urlHelper;
            }
        }

        /// <summary>
        /// Returns a <see cref="ViewDataDictionary"/> typed to the Model's type.
        /// </summary>
        public ViewDataDictionary ViewData
        {
            get
            {
                if (_viewData == null)
                {
                    _viewData = new ViewDataDictionary<T>();
                    _viewData.TemplateInfo = new TemplateInfo { HtmlFieldPrefix = "" };
                    if (Model != null)
                        _viewData.Model = Model;
                }

                return _viewData;
            }
            set
            {
                _viewData = value;
            }
        }

        /// <summary>
        /// Writes markup to the specified <paramref name="writer"/>, encoding by default
        /// except in cases where the <paramref name="value"/> is already encoded.
        /// </summary>
        /// <param name="writer">The current <see cref="TextWriter"/>.</param>
        /// <param name="value">The value to write to the <paramref name="writer"/>.</param>
        public override void WriteTo(TextWriter writer, object value)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (value == null)
                return;

            var encodedString = value as IEncodedString;
            if (encodedString != null)
            {
                writer.Write(encodedString);
            }
            else
            {
                var htmlString = value as IHtmlString;
                if (htmlString != null)
                {
                    writer.Write(htmlString.ToHtmlString());
                }
                else
                {
                    //This was the base template's implementation:
                    encodedString = TemplateService.EncodedStringFactory.CreateEncodedString(value);
                    writer.Write(encodedString);
                }
            }
        }
    }
}
