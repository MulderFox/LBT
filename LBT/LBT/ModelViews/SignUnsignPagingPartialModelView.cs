using System.Web.Routing;

namespace LBT.ModelViews
{
    public class SignUnsignPagingPartialModelView
    {
        public string SectionClass { get; set; }
        public PagedList.IPagedList List { get; set; }
        public string ActionName { get; set; }
        public string PagingRouteValueName { get; set; }
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

        public SignUnsignPagingPartialModelView()
        {
            SectionClass = "Paging";
            ActionName = "Index";
            PagingRouteValueName = "page";
        }

        public RouteValueDictionary GetRouteValueDictionary(int pagerPage, int currentPage, string sortOrder, int pageSize, string currentFilter, string currentFilterAccording, string signedSortOrder, int currentSignedPage, int filteredUserId)
        {
            var routeValues = new RouteValueDictionary(RouteValues)
                                  {
                                      {"filteredUserId", filteredUserId},
                                      {"pageSize", pageSize},
                                      {"signedPage", currentSignedPage},
                                      {"signedSortOrder", signedSortOrder},
                                      {"currentFilterAccording", currentFilterAccording},
                                      {"currentFilter", currentFilter},
                                      {"page", currentPage},
                                      {"sortOrder", sortOrder}
                                  };
            routeValues[PagingRouteValueName] = pagerPage;

            return routeValues;
        }

        public RouteValueDictionary GetRouteValueDictionary(int pagerPage, int currentPage, string sortOrder, int pageSize, string currentFilter, string currentFilterAccording, string signedSortOrder, int currentSignedPage)
        {
            var routeValues = new RouteValueDictionary(RouteValues)
                                  {
                                      {"pageSize", pageSize},
                                      {"signedPage", currentSignedPage},
                                      {"signedSortOrder", signedSortOrder},
                                      {"currentFilterAccording", currentFilterAccording},
                                      {"currentFilter", currentFilter},
                                      {"page", currentPage},
                                      {"sortOrder", sortOrder}
                                  };
            routeValues[PagingRouteValueName] = pagerPage;

            return routeValues;
        }
    }
}