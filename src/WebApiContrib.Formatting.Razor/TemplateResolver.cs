using System.IO;
using RazorEngine.Templating;

namespace WebApiContrib.Formatting.Razor
{
    public class TemplateResolver : ITemplateResolver
    {
        public string Resolve(string name)
        {
            //Replace the "~/" to the root path of the web.
            name = name.Replace("~", RazorViewLocator.GetPhysicalSiteRootPath(null)).Replace("/", "\\");

            if (!File.Exists(name))
                throw new FileNotFoundException(name);

            return File.ReadAllText(name);
        }
    }
}