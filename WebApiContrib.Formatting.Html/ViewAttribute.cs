using System;

namespace WebApiContrib.Formatting.Html
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited= true)] 
    public class ViewAttribute : Attribute
    {
        public ViewAttribute(string viewName)
        {
            if (string.IsNullOrWhiteSpace(viewName))
                throw new ArgumentException("Argument viewName can't be null or empty", "viewName");

            ViewName = viewName;
        }


        public string ViewName { get; private set; }
    }
}
