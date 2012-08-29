using System.Web.Http;
using WebApiContrib.Formatting.Html;

namespace MvcWebApiSiteTest.Controllers
{
    public class HomeController : ApiController
    {
        public View Get()
        {
            return new View("Index", null);
        }
    }
}
