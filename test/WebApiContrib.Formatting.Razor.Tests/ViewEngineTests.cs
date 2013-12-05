using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using WebApiContrib.Formatting.Html;
using WebApiContrib.Formatting.Html.Formatting;

namespace WebApiContrib.Formatting.Razor.Tests
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
        public async Task render_simple_template()
        {
            var cts = new CancellationTokenSource();
            var view = new ViewResult(new HttpRequestMessage(), "Test1", new {Name = "foo"});

            var response = await view.ExecuteAsync(cts.Token);
            var output = await response.Content.ReadAsStringAsync();

            Assert.AreEqual("Hello foo! Welcome to Razor!", output);
        }

        [Test]
        public async Task render_template_with_embedded_layout()
        {
            var cts = new CancellationTokenSource();
            var view = new ViewResult(new HttpRequestMessage(), "Test2", new { Name = "foo" });

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
            var view = new ViewResult(new HttpRequestMessage(), "Test2", new { Name = "foo" });

            var response = await view.ExecuteAsync(cts.Token);
            var output = await response.Content.ReadAsStringAsync();

            Assert.AreEqual("<html>Hello foo! Welcome to Razor!</html>", output);
        }
    }
}
