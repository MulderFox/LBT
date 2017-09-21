using System.Web.Routing;

namespace LBT.ModelViews
{
    public class FilteringPartialModelView
    {
        public string SectionId { get; set; }
        public object RouteValues { get; set; }
        public bool ShowFiltering { get; set; }
        public bool ShowSwitchToIndexTree { get; set; }
        public string IndexTreeActionName { get { return "IndexTree"; } }
        public bool ShowSwitchToIndex { get; set; }
        public string IndexActionName { get { return "Index"; } }

        public RouteValueDictionary RouteValueDictionary
        {
            get
            {
                return new RouteValueDictionary(RouteValues);
            }
        }

        public FilteringPartialModelView()
        {
            SectionId = "Filter1";
            ShowFiltering = true;
        }
    }
}