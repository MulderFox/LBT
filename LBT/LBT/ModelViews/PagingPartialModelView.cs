using System.Web.Routing;

namespace LBT.ModelViews
{
    public class PagingPartialModelView
    {
        public string SectionId { get; set; }
        public PagedList.IPagedList List { get; set; }
        public string ActionName { get; set; }
        public int Page { get { return List.PageCount < List.PageNumber ? 0 : List.PageNumber; } }
        public int PageCount { get { return List.PageCount; } }
        public int TotalItemCount { get { return List.TotalItemCount; } }
        public object RouteValues { get; set; }

        public RouteValueDictionary RouteValueDictionary
        {
            get
            {
                return new RouteValueDictionary(RouteValues);
            }
        }

        public PagingPartialModelView()
        {
            SectionId = "Paging";
            ActionName = "Index";
        }

        public RouteValueDictionary GetRouteValueDictionary(int page, string sortOrder, string currentFilter, string currentFilterAccording, int? filteredUserId, int pageSize)
        {
            var routeValues = new RouteValueDictionary(RouteValues)
                                  {
                                      {"page", page},
                                      {"sortOrder", sortOrder},
                                      {"currentFilter", currentFilter},
                                      {"currentFilterAccording", currentFilterAccording},
                                      {"filteredUserId", filteredUserId},
                                      {"pageSize", pageSize}
                                  };
            return routeValues;
        }
    }
}