using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http.Headers;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace WebApiContrib.Formatting.RazorViewEngine
{
    public class RazorViewEngine  : IViewEngine
    {
		private readonly ITemplateService _templateService;

		public RazorViewEngine(ITemplateService templateService)
		{
			if (templateService == null)
				throw new ArgumentNullException("templateService");

			_templateService = templateService;
		}

        public RazorViewEngine()
        {
	        var config = new TemplateServiceConfiguration();
	        _templateService = new TemplateService(config);
        }

        public RazorViewEngine(Type rootLocatorType) 
        {
            var config = new TemplateServiceConfiguration { Resolver = new EmbeddedResolver(rootLocatorType) };
	        _templateService = new TemplateService(config);
        }

		public RazorViewEngine(ITemplateResolver resolver)
		{
			if (resolver == null)
				throw new ArgumentNullException("resolver");

			var config = new TemplateServiceConfiguration { Resolver = resolver };
			_templateService = new TemplateService(config);
		}

        public void RenderTo<T>(T model, Stream templateStream, Stream outputStream)
        {
            string template = new StreamReader(templateStream).ReadToEnd();
            string result = _templateService.Parse(template, model);

            var sw = new StreamWriter(outputStream);
            sw.Write(result);
            sw.Flush();
        }

        public Collection<MediaTypeHeaderValue> SupportedMediaTypes
        {
            get
            {
                return new Collection<MediaTypeHeaderValue>
                {
                    new MediaTypeHeaderValue("text/html"),
                    new MediaTypeHeaderValue("application/xhtml")
                };
            }
        }
    }
}
