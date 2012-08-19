using System;
using System.IO;
using System.Reflection;
using WebApiContrib.Formatting.Html;
using WebApiContrib.Formatting.Html.Locators;

namespace WebApiContrib.Formatting.Razor
{
    public class RazorViewLocator : IViewLocator
    {
        private readonly string[] _viewLocationFormats = new[]
            {
                "~\\Views\\{0}.cshtml",
                "~\\Views\\{0}.vbhtml",
                "~\\Views\\Shared\\{0}.cshtml",
                "~\\Views\\Shared\\{0}.vbhtml"
            };


        public string GetView(string siteRootPath, IView view)
        {
            if (view == null)
                throw new ArgumentNullException("view");

            var path = GetPhysicalSiteRootPath(siteRootPath);

            foreach(var viewLocationFormat in _viewLocationFormats)
            {
                var potentialViewPathFormat = viewLocationFormat.Replace("~", GetPhysicalSiteRootPath(siteRootPath));

                var viewPath = string.Format(potentialViewPathFormat, view.ViewName);

                if (File.Exists(viewPath))
                    return File.ReadAllText(viewPath);
            }

            throw new FileNotFoundException(string.Format("Can't find a view with the name '{0}.cshtml' or '{0}.vbhtml in the '\\Views' folder under  path '{1}'", view.ViewName, path));
        }


        internal static string GetPhysicalSiteRootPath(string siteRootPath)
        {
            if (string.IsNullOrWhiteSpace(siteRootPath))
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)
                           .Replace("file:\\", string.Empty)
                           .Replace("\\bin", string.Empty);

            return siteRootPath;
        }
    }
}