using System.Text;

namespace WebApiContrib.Formatting.Html.ViewParsers
{
    public interface IViewParser
    {
        byte[] ParseView(IView view, string viewTemplate, Encoding encoding);
    }
}
