namespace WebApiContrib.Formatting.Razor

#if INTERACTIVE
#I "../packages"
#r "System.Net.Http"
#r "Microsoft.AspNet.Mvc.5.0.0/lib/net45/System.Web.Mvc.dll"
#r "Microsoft.AspNet.Razor.3.0.0/lib/net45/System.Web.Razor.dll"
#r "Microsoft.AspNet.WebApi.Client.5.0.0/lib/net45/System.Net.Http.Formatting.dll"
#r "Microsoft.AspNet.WebApi.Core.5.0.0/lib/net45/System.Web.Http.dll"
#r "RazorEngine.3.3.0/lib/net40/RazorEngine.dll"
#r "WebApiContrib.Formatting.Html.2.0.0/lib/net45/WebApiContrib.Formatting.Html.dll"
#endif

open System
open System.Collections.Generic
open System.IO
open System.Web
open System.Web.Mvc
open System.Web.Routing
open RazorEngine.Configuration
open RazorEngine.Templating
open RazorEngine.Text

/// <summary>
/// A <see cref="TemplateBase"/> class for use when rendering HTML with Razor in Web API applications.
/// </summary>
/// <typeparam name="T">The Model type.</typeparam>
[<AbstractClass>]
[<RequireNamespaces("System.Web.Mvc.Html")>]
type HtmlTemplateBase<'T>() =
    inherit TemplateBase<'T>()

    let urlHelper = lazy (new System.Web.Http.Routing.UrlHelper())
    let mutable viewData : ViewDataDictionary = null

    /// <summary>
    /// Writes markup to the specified <paramref name="writer"/>, encoding by default
    /// except in cases where the <paramref name="value"/> is already encoded.
    /// </summary>
    /// <param name="writer">The current <see cref="TextWriter"/>.</param>
    /// <param name="value">The value to write to the <paramref name="writer"/>.</param>
    /// <see href="http://stackoverflow.com/questions/19431365/razorengine-html-helpers-work-but-escape-the-html"/>
    override this.WriteTo(writer: TextWriter, value: obj) =
        if writer = null then
            raise (new ArgumentNullException("writer"))

        if value = null then () else
        let valueType = value.GetType()
        if typeof<IEncodedString>.IsAssignableFrom(valueType) then
            let encodedString = unbox<IEncodedString> value
            writer.Write(encodedString)
        elif typeof<IHtmlString>.IsAssignableFrom(valueType) then
            let htmlString = unbox<IHtmlString> value
            writer.Write(htmlString.ToHtmlString())
        else
            // This was the base template's implementation:
            writer.Write(this.TemplateService.EncodedStringFactory.CreateEncodedString(value))

    /// <summary>
    /// Returns a <see cref="System.Web.Mvc.HtmlHelper{T}"/> based on the current Model type.
    /// </summary>
    member this.Html =
        // NOTE: Removed caching, as that would block updates the ViewData property, which has a setter.
        let writer = this.CurrentWriter
        let viewContext = new ViewContext(Writer = writer, ViewData = this.ViewData)
        new HtmlHelper<'T>(viewContext, this)

    /// <summary>
    /// Returns a <see cref="System.Web.Http.Routing.UrlHelper"/>.
    /// </summary>
    /// <remarks>
    /// This is not currently tied to the current <see cref="System.Net.Http.HttpRequestMessage"/> but should be.
    /// </remarks>
    member this.Url = urlHelper.Force()

    /// <summary>
    /// Returns a <see cref="ViewDataDictionary"/> typed to the Model's type.
    /// </summary>
    member this.ViewData
        with get() : ViewDataDictionary =
            if viewData = null then
                viewData <- new ViewDataDictionary<'T>(TemplateInfo = new TemplateInfo(HtmlFieldPrefix = ""), Model = this.Model)
            viewData
        and set(value) =
            viewData <- value

    interface IViewDataContainer with
        member this.ViewData
            with get() = this.ViewData
            and set(value) = this.ViewData <- value
