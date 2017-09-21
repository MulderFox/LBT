using System.Web.Routing;

namespace LBT.ModelViews
{
    public class SignUnsignFilteringPartialModelView
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

        public SignUnsignFilteringPartialModelView()
        {
            SectionClass = "Filter";
        }
    }
}