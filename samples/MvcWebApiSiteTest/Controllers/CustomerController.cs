using System.Web.Http;
using WebApiContrib.Formatting.Html;

namespace MvcWebApiSiteTest.Controllers
{
    public class CustomerController : ApiController
    {
        // GET api/customer
        public ViewResult Get()
        {
            return new ViewResult(this.Request, "CustomerViaViewResult", new Customer { Name = "John Doe", Country = "Sweden" });
        }

        // GET api/customer/1
        public Customer Get(int id)
        {
            return new Customer { Name = "John Doe", Country = "Sweden" };
        }
    }


    //[View("CustomerViaAttrib")]
    public class Customer
    {
        public string Name { get; set; }

        public string Country { get; set; }
    }
}