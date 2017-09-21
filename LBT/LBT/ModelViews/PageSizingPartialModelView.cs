using System.Web.Routing;

namespace LBT.ModelViews
{
    public class PageSizingPartialModelView
    {
        public string SectionClass { get; set; }
        public object RouteValues { get; set; }

        public RouteValueDictionary RouteValueDictionary
        {
            get
            {
                return new RouteValueDictionary(RouteValues);
            }
        }

        public PageSizingPartialModelView()
        {
            SectionClass = "Paging";
        }
    }
}