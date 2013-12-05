using System.Web.Http;
using WebApiContrib.Formatting.Html;

namespace MvcWebApiSiteTest.Controllers
{
    public class HomeController : ApiController
    {
        public IHttpActionResult Get()
        {
            return new ViewResult(Request, "Index", null);
        }
    }
}
