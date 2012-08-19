namespace WebApiContrib.Formatting.Html.Locators
{
    public interface IViewLocator
    {
        string GetView(string siteRootPath, IView view);
    }
}
