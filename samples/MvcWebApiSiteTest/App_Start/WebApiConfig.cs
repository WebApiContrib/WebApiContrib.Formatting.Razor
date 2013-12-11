using System.Web.Http;
using RazorEngine.Configuration;
using WebApiContrib.Formatting.Html;
using WebApiContrib.Formatting.Html.Formatting;
using WebApiContrib.Formatting.Razor;

namespace MvcWebApiSiteTest
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Formatters.Add(new RazorViewFormatter());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { controller = "Home", id = RouteParameter.Optional }
            );
        }
    }
}
