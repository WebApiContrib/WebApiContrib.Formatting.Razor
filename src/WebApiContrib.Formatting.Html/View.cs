using System;

namespace WebApiContrib.Formatting.Html
{
    public class View : IView
    {
        public View(string viewName, object model) : this(viewName, model, null)
        {
        }

        internal View(string viewName, object model, Type modelType)
        {
            Model = model;
            ViewName = viewName;

            if (modelType != null)
                ModelType = model.GetType();
        }

        public object Model { get; protected set; }
        public string ViewName { get; protected set; }
        public Type ModelType { get; protected set; }
    }
}
