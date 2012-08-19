using System;
using System.Collections.Generic;
using WebApiContrib.Formatting.Html.Locators;
using WebApiContrib.Formatting.Html.ViewParsers;

namespace WebApiContrib.Formatting.Html.Configuration
{
    public static class GlobalViews
    {
        static GlobalViews()
        {
            Views = new Dictionary<Type, string>();
        }

        public static IDictionary<Type, string> Views { get; private set; }

        public static IViewLocator DefaultViewLocator { get; set; }

        public static IViewParser DefaultViewParser { get; set; }
    }
}
