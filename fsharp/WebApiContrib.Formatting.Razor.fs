namespace WebApiContrib.Formatting.Razor

#if INTERACTIVE
#I "../packages"
#r "System.Net.Http"
#r "Microsoft.AspNet.WebApi.Client.5.0.0/lib/net45/System.Net.Http.Formatting.dll"
#r "Microsoft.AspNet.WebApi.Core.5.0.0/lib/net45/System.Web.Http.dll"
#r "Microsoft.AspNet.Razor.2.0.30506.0/lib/net40/System.Web.Razor.dll"
#r "RazorEngine.3.3.0/lib/net40/RazorEngine.dll"
#r "WebApiContrib.Formatting.Html.2.0.0/lib/net45/WebApiContrib.Formatting.Html.dll"
#endif

open System
open System.IO
open System.Reflection
open RazorEngine.Configuration
open RazorEngine.Templating
open WebApiContrib.Formatting.Html
open WebApiContrib.Formatting.Html.Formatting

type RazorViewLocator() =
    let viewLocationFormats =
        [| "~\\Views\\{0}.cshtml"
           "~\\Views\\{0}.vbhtml"
           "~\\Views\\Shared\\{0}.cshtml"
           "~\\Views\\Shared\\{0}.vbhtml" |]

    static member internal GetPhysicalSiteRootPath(siteRootPath: string) =
        if String.IsNullOrWhiteSpace(siteRootPath) then
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)
                .Replace("file:\\", String.Empty)
                .Replace("\\bin", String.Empty)
                .Replace("\\Debug", String.Empty)
                .Replace("\\Release", String.Empty)
        else siteRootPath

    interface IViewLocator with
        member this.GetView(siteRootPath, view) =
            if view = Unchecked.defaultof<_> then
                raise (new ArgumentNullException("view"))

            let path = RazorViewLocator.GetPhysicalSiteRootPath(siteRootPath)

            let result = seq {
                for viewLocationFormat in viewLocationFormats do
                    let potentialViewPathFormat = viewLocationFormat.Replace("~", RazorViewLocator.GetPhysicalSiteRootPath(siteRootPath));

                    let viewPath = String.Format(potentialViewPathFormat, view.ViewName);

                    if File.Exists(viewPath) then
                        yield File.ReadAllText(viewPath) }

            if Seq.isEmpty result then
                raise (new FileNotFoundException(String.Format("Can't find a view with the name '{0}.cshtml' or '{0}.vbhtml in the '\\Views' folder under  path '{1}'", view.ViewName, path)))
            else Seq.head result

type TemplateResolver() =
    interface ITemplateResolver with
        member this.Resolve(name) =
            //Replace the "~/" to the root path of the web.
            let name = name.Replace("~", RazorViewLocator.GetPhysicalSiteRootPath(null)).Replace("/", "\\")

            if not (File.Exists(name)) then
                raise (new FileNotFoundException(name))

            File.ReadAllText(name)

// Type passed should be located at the root of the folder structure where the embedded templates are located
type EmbeddedResolver(rootLocatorType: Type) =
    interface ITemplateResolver with
        member this.Resolve(name) =
            // To locate embedded files, 
            //    - they must be marked as "Embedded Resource"
            //    - you must use a case senstive path and filename
            //    - the namespaces and project folder names must match.
            //
            name = name.Replace("~/", "").Replace("/", ".")  //Convert "web path" to "resource path"
            let viewStream = rootLocatorType.Assembly.GetManifestResourceStream(rootLocatorType, name)
            use reader = new StreamReader(viewStream)
            reader.ReadToEnd()

/// <summary>
/// An <see cref="IViewParser"/> for Razor templates.
/// </summary>
/// <param name="templateService">The <see cref="ITemplateService"/>.</param>
type RazorViewParser(templateService: ITemplateService) =

    do if templateService = Unchecked.defaultof<_> then
        raise (new ArgumentNullException("templateService"))

    /// <summary>
    /// Initializes a new <see cref="RazorViewParser"/> with the specified <paramref name="ITemplateServiceConfiguration"/>.
    /// </summary>
    /// <param name="config">The <see cref="ITemplateServiceConfiguration"/>.</param>
    new (config: ITemplateServiceConfiguration) =
        new RazorViewParser(new TemplateService(config) :> ITemplateService)

    /// <summary>
    /// Initializes a new <see cref="RazorViewParser"/> with optional arguments.
    /// </summary>
    /// <param name="resolver">The <see cref="ITemplateResolver"/>. If not provided, <see cref="TemplateResolver"/> is used.</param>
    /// <param name="baseTemplateType">The <see cref="Type"/> to use as the TemplateBase.</param>
    new (?resolver: ITemplateResolver, ?baseTemplateType: Type) =
        let resolver = defaultArg resolver (new TemplateResolver() :> ITemplateResolver)
        let baseTemplateType = defaultArg baseTemplateType Unchecked.defaultof<_>
        let config = new TemplateServiceConfiguration(BaseTemplateType = baseTemplateType, Resolver = resolver)
        new RazorViewParser(new TemplateService(config) :> ITemplateService)

    interface IViewParser with
        /// <summary>
        /// Parses the <paramref name="viewTemplate"/> with the provided <paramref name="view"/>.
        /// </summary>
        /// <param name="view">The <see cref="IView"/>.</param>
        /// <param name="viewTemplate">The view template to parse.</param>
        /// <param name="encoding">The <see cref="System.Text.Encoding"/> to use in parsing.</param>
        /// <returns>The <see cref="byte[]"/> representing the parsed view.</returns>
        member this.ParseView(view, viewTemplate, encoding) =
            templateService.Compile(viewTemplate, view.ModelType, view.ViewName)
            let parsedView = templateService.Run(view.ViewName, view.Model, null)
            encoding.GetBytes(parsedView);

/// <summary>
/// <see cref="HtmlMediaTypeFormatter"/> using the Razor syntax.
/// </summary>
/// <param name="siteRootPath">The root path containing view files. This defaults to "~/Views".</param>
/// <param name="viewLocator">The <see cref="IViewLocator"/> instance used to locate the correct view. This defaults to <see cref="RazorViewLocator"/>.</param>
/// <param name="viewParser">The <see cref="IViewParser"/> instance used to parse the view. This defaults to <see cref="RazorViewParser"/>.</param>
[<Sealed>]
type RazorViewFormatter(?siteRootPath: string, ?viewLocator: IViewLocator, ?viewParser: IViewParser) =
    inherit HtmlMediaTypeViewFormatter(
        defaultArg siteRootPath null,
        defaultArg viewLocator (new RazorViewLocator() :> IViewLocator),
        defaultArg viewParser (new RazorViewParser() :> IViewParser))