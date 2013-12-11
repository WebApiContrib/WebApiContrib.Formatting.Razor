using System;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using WebApiContrib.Formatting.Html;

namespace WebApiContrib.Formatting.Razor
{
    /// <summary>
    /// An <see cref="IViewParser"/> for Razor templates.
    /// </summary>
    public class RazorViewParser : IViewParser
    {
        private readonly ITemplateService _templateService;

        /// <summary>
        /// Initializes a new <see cref="RazorViewParser"/> with the specified <paramref name="ITemplateService"/>.
        /// </summary>
        /// <param name="templateService">The <see cref="ITemplateService"/>.</param>
        public RazorViewParser(ITemplateService templateService)
        {
            if (templateService == null)
                throw new ArgumentNullException("templateService");

            _templateService = templateService;
        }

        /// <summary>
        /// Initializes a new <see cref="RazorViewParser"/> with the specified <paramref name="ITemplateServiceConfiguration"/>.
        /// </summary>
        /// <param name="config">The <see cref="ITemplateServiceConfiguration"/>.</param>
        public RazorViewParser(ITemplateServiceConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            _templateService = new TemplateService(config);
        }

        /// <summary>
        /// Initializes a new <see cref="RazorViewParser"/> with a default <see cref="ITemplateService"/>
        /// using the <see cref="WebApiContrib.Formatting.Razor.TemplateResolver"/>.
        /// </summary>
        public RazorViewParser()
            : this(new TemplateServiceConfiguration { Resolver = new TemplateResolver() })
        {
        }

        [Obsolete("Use the constructor overload taking an optional ITemplateResolver or the ITemplateServiceConfiguration.")]
        public RazorViewParser(ITemplateResolver resolver)
        {
            if (resolver == null)
                throw new ArgumentNullException("resolver");

            var config = new TemplateServiceConfiguration { Resolver = resolver };
            _templateService = new TemplateService(config);
        }
        
        /// <summary>
        /// Parses the <paramref name="viewTemplate"/> with the provided <paramref name="view"/>.
        /// </summary>
        /// <param name="view">The <see cref="IView"/>.</param>
        /// <param name="viewTemplate">The view template to parse.</param>
        /// <param name="encoding">The <see cref="System.Text.Encoding"/> to use in parsing.</param>
        /// <returns>The <see cref="byte[]"/> representing the parsed view.</returns>
        public byte[] ParseView(IView view, string viewTemplate, System.Text.Encoding encoding)
        {
            var parsedView = GetParsedView(view, viewTemplate);
            return encoding.GetBytes(parsedView);
        }

        private string GetParsedView(IView view, string viewTemplate)
        {
            _templateService.Compile(viewTemplate, view.ModelType, view.ViewName);
            return _templateService.Run(view.ViewName, view.Model, null);
        }
    }
}
