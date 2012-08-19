using System;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using WebApiContrib.Formatting.Html;
using WebApiContrib.Formatting.Html.ViewParsers;


namespace WebApiContrib.Formatting.Razor
{
    public class RazorViewParser : IViewParser
    {
        private readonly ITemplateService _templateService;

		public RazorViewParser(ITemplateService templateService)
		{
			if (templateService == null)
				throw new ArgumentNullException("templateService");

			_templateService = templateService;
		}

        public RazorViewParser()
        {
            var config = new TemplateServiceConfiguration { Resolver = new TemplateResolver() };
            _templateService = new TemplateService(config);
        }


        public RazorViewParser(ITemplateResolver resolver)
		{
			if (resolver == null)
				throw new ArgumentNullException("resolver");

			var config = new TemplateServiceConfiguration { Resolver = resolver };
			_templateService = new TemplateService(config);
		}
        
        
        public byte[] ParseView(IView view, string viewTemplate, System.Text.Encoding encoding)
        {
            _templateService.Compile(viewTemplate, view.ModelType, view.ModelType.Name);

            var parsedView = _templateService.Run(view.ModelType.Name, view.Model);

            return encoding.GetBytes(parsedView);
        }
    }
}
