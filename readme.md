WebApiContrib.Formatting.Razor
=============

A `MediaTypeFormatter` for generating HTML markup for [ASP.NET Web API](http://asp.net/web-api) applications.

Get it from [NuGet](http://nuget.org/packages/WebApiContrib.Formatting.Razor):

    Install-Package WebApiContrib.Formatting.Razor

WebApiContrib.Formatting.Razor lets you return HTML markup using three different mechanisms:

1. conventionally [name the view](https://github.com/WebApiContrib/WebApiContrib.Formatting.Razor/tree/master/samples/MvcWebApiSiteTest/Views) the same as the [return type](https://github.com/WebApiContrib/WebApiContrib.Formatting.Razor/blob/master/samples/MvcWebApiSiteTest/Controllers/CustomerController.cs#L14)
2. [return a `View` object](https://github.com/WebApiContrib/WebApiContrib.Formatting.Razor/blob/master/samples/MvcWebApiSiteTest/Controllers/HomeController.cs#L10)
3. [attribute your return type with a `ViewAttribute`](https://github.com/WebApiContrib/WebApiContrib.Formatting.Razor/blob/master/samples/MvcWebApiSiteTest/Controllers/CustomerController.cs#L21)
4. define the view mapping in a [`ViewConfig`](https://github.com/WebApiContrib/WebApiContrib.Formatting.Razor/blob/master/samples/MvcWebApiSiteTest/App_Start/ViewConfig.cs)

You also need to register the formatter and set the `GlobalViews.DefaultViewParser` and `GlobalViews.DefaultViewLocator`:

    GlobalConfiguration.Configuration.Formatters.Add(new HtmlMediaTypeViewFormatter());

    GlobalViews.DefaultViewParser = new RazorViewParser();
    GlobalViews.DefaultViewLocator = new RazorViewLocator();
