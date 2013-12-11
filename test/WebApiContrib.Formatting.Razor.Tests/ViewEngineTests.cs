using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using NUnit.Framework;
using RazorEngine.Configuration;
using WebApiContrib.Formatting.Html;
using WebApiContrib.Formatting.Html.Formatting;

namespace WebApiContrib.Formatting.Razor.Tests
{
    [TestFixture]
    public class ViewEngineTests
    {
        private HttpRequestMessage _request;

        [SetUp]
        public void Before()
        {
            var formatter = new RazorViewFormatter();
            var config = new HttpConfiguration();
            config.Formatters.Add(formatter);

            _request = new HttpRequestMessage();
            _request.SetConfiguration(config);
            _request.RegisterForDispose(config);
        }

        [TearDown]
        public void After()
        {
            _request.Dispose();
            _request = null;
        }

        [Test]
        public async Task render_simple_template()
        {
            var cts = new CancellationTokenSource();
            var view = new ViewResult(_request, "Test1", new {Name = "foo"});

            var response = await view.ExecuteAsync(cts.Token);
            var output = await response.Content.ReadAsStringAsync();

            Assert.AreEqual("Hello foo! Welcome to Razor!", output);
        }

        [Test]
        public async Task render_template_with_embedded_layout()
        {
            var cts = new CancellationTokenSource();
            var view = new ViewResult(_request, "Test2", new { Name = "foo" });

            var response = await view.ExecuteAsync(cts.Token);
            var output = await response.Content.ReadAsStringAsync();

            Assert.AreEqual("<html>Hello foo! Welcome to Razor!</html>", output);
        }

        [Test]
        public async Task render_template_with_specified_resolver()
        {
            var cts = new CancellationTokenSource();
            var resolver = new EmbeddedResolver(this.GetType());
            var formatter = new HtmlMediaTypeViewFormatter(null, new RazorViewLocator(), new RazorViewParser(resolver));

            // Replace the HTML formatter.
            var config = _request.GetConfiguration();
            var oldFormatter = config.Formatters.GetHtmlFormatter();
            config.Formatters.Remove(oldFormatter);
            config.Formatters.Add(formatter);

            var view = new ViewResult(_request, "Test2", new { Name = "foo" });

            var response = await view.ExecuteAsync(cts.Token);
            var output = await response.Content.ReadAsStringAsync();

            Assert.AreEqual("<html>Hello foo! Welcome to Razor!</html>", output);
        }
    }
}
