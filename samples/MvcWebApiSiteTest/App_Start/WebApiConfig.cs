using System.Web.Http;

namespace MvcWebApiSiteTest
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { controller = "Home", id = RouteParameter.Optional }
            );
        }
    }
}
