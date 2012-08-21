using System.IO;
using System.Net.Http;
using System.Reflection;
using NUnit.Framework;
using WebApiContrib.Formatting.Html;
using WebApiContrib.Formatting.Html.Configuration;
using WebApiContrib.Formatting.Html.Formatters;
using WebApiContrib.Formatting.Razor;

namespace WebApiContrib.Formatting.RazorViewEngine.Tests
{
    [TestFixture]
    public class ViewEngineTests
    {
	    private HtmlMediaTypeViewFormatter _formatter;

		[SetUp]
		public void Before()
		{
			_formatter = new HtmlMediaTypeViewFormatter();
            GlobalViews.DefaultViewParser = new RazorViewParser();
            GlobalViews.DefaultViewLocator = new RazorViewLocator();
		}

        [Test]
        public void render_simple_template()
        {
            var view = new View("Test1", new {Name = "foo"});
	        var content = new ObjectContent<View>(view, _formatter);

	        var output = content.ReadAsStringAsync().Result;

            Assert.AreEqual("Hello foo! Welcome to Razor!", output);
        }

        [Test]
        public void render_template_with_embedded_layout()
        {
            var view = new View("Test2", new { Name = "foo" });
	        var content = new ObjectContent<View>(view, _formatter);

	        var output = content.ReadAsStringAsync().Result;

            Assert.AreEqual("<html>Hello foo! Welcome to Razor!</html>", output);
        }

        [Test]
        public void render_template_with_specified_resolver()
        {
	        var resolver = new EmbeddedResolver(this.GetType());
            var formatter = new HtmlMediaTypeViewFormatter(null, new RazorViewLocator(), new RazorViewParser(resolver));
            var view = new View("Test2", new { Name = "foo" });
	        var content = new ObjectContent<View>(view, formatter);

	        var output = content.ReadAsStringAsync().Result;

            Assert.AreEqual("<html>Hello foo! Welcome to Razor!</html>", output);
        }
    }
}
