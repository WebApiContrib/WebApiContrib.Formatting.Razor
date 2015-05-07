using System;
using System.IO;
using System.Reflection;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using WebApiContrib.Formatting.Html;
using WebApiContrib.Formatting.Html.Formatting;

namespace WebApiContrib.Formatting.Razor
{
    public class RazorViewLocator : IViewLocator
    {
        private readonly string[] _viewLocationFormats = new[]
            {
                "~\\Views\\{0}.cshtml",
                "~\\Views\\{0}.vbhtml",
                "~\\Views\\Shared\\{0}.cshtml",
                "~\\Views\\Shared\\{0}.vbhtml"
            };

        public string GetView(string siteRootPath, IView view)
        {
            if (view == null)
                throw new ArgumentNullException("view");

            var path = GetPhysicalSiteRootPath(siteRootPath);

            foreach(var viewLocationFormat in _viewLocationFormats)
            {
                var potentialViewPathFormat = viewLocationFormat.Replace("~", GetPhysicalSiteRootPath(siteRootPath));

                var viewPath = string.Format(potentialViewPathFormat, view.ViewName);

                if (File.Exists(viewPath))
                    return File.ReadAllText(viewPath);
            }

            throw new FileNotFoundException(string.Format("Can't find a view with the name '{0}.cshtml' or '{0}.vbhtml in the '\\Views' folder under  path '{1}'", view.ViewName, path));
        }

        internal static string GetPhysicalSiteRootPath(string siteRootPath)
        {
            if (string.IsNullOrWhiteSpace(siteRootPath))
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)
                           .Replace("file:\\", string.Empty)
                           .Replace("\\bin", string.Empty)
                           .Replace("\\Debug", string.Empty)
                           .Replace("\\Release", string.Empty);

            return siteRootPath;
        }
    }
    public class TemplateResolver : ITemplateResolver
    {
        public string Resolve(string name)
        {
            //Replace the "~/" to the root path of the web.
            name = name.Replace("~", RazorViewLocator.GetPhysicalSiteRootPath(null)).Replace("/", "\\");

            if (!File.Exists(name))
                throw new FileNotFoundException(name);

            return File.ReadAllText(name);
        }
    }

    public class EmbeddedResolver : ITemplateResolver
    {
        private readonly Type _rootLocatorType;

        // Type passed should be located at the root of the folder structure where the embedded templates are located
        public EmbeddedResolver(Type rootLocatorType)
        {
            _rootLocatorType = rootLocatorType;
        }

        public string Resolve(string name)
        {
            // To locate embedded files, 
            //    - they must be marked as "Embedded Resource"
            //    - you must use a case senstive path and filename
            //    - the namespaces and project folder names must match.
            //
            name = name.Replace("~/", "").Replace("/", ".");  //Convert "web path" to "resource path"
            var viewStream = _rootLocatorType.Assembly.GetManifestResourceStream(_rootLocatorType, name);
            using (var reader = new StreamReader(viewStream))
                return reader.ReadToEnd();
        }
    }

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
        /// Initializes a new <see cref="RazorViewParser"/> with optional arguments.
        /// </summary>
        /// <param name="resolver">The <see cref="ITemplateResolver"/>. If not provided, <see cref="TemplateResolver"/> is used.</param>
        /// <param name="baseTemplateType">The <see cref="Type"/> to use as the TemplateBase.</param>
        public RazorViewParser(ITemplateResolver resolver = null, Type baseTemplateType = null)
            : this(new TemplateServiceConfiguration { BaseTemplateType = baseTemplateType, Resolver = resolver ?? new TemplateResolver() })
        {
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