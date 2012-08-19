using System;
using System.Web.Http;

namespace MvcApplication1.Controllers
{
    public class CustomerController : ApiController
    {
        //// GET api/customer
        ////public View Get()
        ////{
        ////    return new View("CustomerViaView", new Customer { Name = "John Doe", Country = "Sweden" });
        ////}

        // GET api/customer
        public Customer Get()
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