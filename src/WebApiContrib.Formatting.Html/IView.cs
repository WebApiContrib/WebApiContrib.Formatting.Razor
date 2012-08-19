using System;

namespace WebApiContrib.Formatting.Html
{
    public interface IView
    {
        object Model { get; }

        Type ModelType { get; }

        string ViewName { get; }
    }
}
