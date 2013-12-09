WebApiContrib.Formatting.Razor
=============

A `MediaTypeFormatter` for generating HTML markup for [ASP.NET Web API](http://asp.net/web-api) applications.

Get it from [NuGet](http://nuget.org/packages/WebApiContrib.Formatting.Razor):

    Install-Package WebApiContrib.Formatting.Razor

WebApiContrib.Formatting.Razor lets you return HTML markup using three different mechanisms:

1. conventionally [name the view](https://github.com/WebApiContrib/WebApiContrib.Formatting.Razor/tree/master/samples/MvcWebApiSiteTest/Views) the same as the [return type](https://github.com/WebApiContrib/WebApiContrib.Formatting.Razor/blob/master/samples/MvcWebApiSiteTest/Controllers/CustomerController.cs#L14)
2. [return a `ViewResult` object](https://github.com/WebApiContrib/WebApiContrib.Formatting.Razor/blob/master/samples/MvcWebApiSiteTest/Controllers/HomeController.cs#L10)
3. [attribute your return type with a `ViewAttribute`](https://github.com/WebApiContrib/WebApiContrib.Formatting.Razor/blob/master/samples/MvcWebApiSiteTest/Controllers/CustomerController.cs#L21)
4. define the view mapping in a [`ViewConfig`](https://github.com/WebApiContrib/WebApiContrib.Formatting.Razor/blob/master/samples/MvcWebApiSiteTest/App_Start/ViewConfig.cs)

The simplest way to register Razor as a formatter is to use the `RazorViewFormatter`:

``` csharp
config.Formatters.Add(new RazorViewFormatter());
```

The `RazorViewFormatter takes three, optional parameters:

1. `siteRootPath` specifies the root folder containing the view files. This looks in `~/Views` by default.
2. `viewLocator` specifies an instance of `IViewLocator`, which is set to `RazorViewLocator` by default.
3. `viewParser` specifies an instance of `IViewParser`, which is set to `RazorViewParser` by default.

You can pass these values into either `RazorViewFormatter` or `HtmlMediaTypeViewFormatter`. You may want to do this to use embedded view resources, for example.

``` csharp
config.Formatters.Add(new RazorViewFormatter(null, new RazorViewLocator(), new RazorViewParser()));
//config.Formatters.Add(new HtmlMediaTypeViewFormatter(null, new RazorViewLocator(), new RazorViewParser()));
```

You may also want to register the `HtmlMediaTypeFormatter` using the default constructor and set the `GlobalViews.DefaultViewParser` and `GlobalViews.DefaultViewLocator`:

``` csharp
GlobalConfiguration.Configuration.Formatters.Add(new HtmlMediaTypeViewFormatter());

GlobalViews.DefaultViewParser = new RazorViewParser();
GlobalViews.DefaultViewLocator = new RazorViewLocator();
// If using ViewConfig:
//ViewConfig.Register(GlobalViews.Views);
```

The [`GlobalViews`](https://github.com/WebApiContrib/WebApiContrib.Formatting.Html/blob/master/src/WebApiContrib.Formatting.Html/Configuration/GlobalViews.cs) configuration comes from the [WebApiContrib.Formatting.Html](https://github.com/WebApiContrib/WebApiContrib.Formatting.Html) project.
