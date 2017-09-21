using LBT.Cache;
using LBT.DAL;
using LBT.Models;
using LBT.Resources;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace LBT.ModelViews
{
    public abstract partial class BaseModelView
    {
        protected const int PageSize = 15;

        protected static void PopulatePageSize(dynamic viewBag, int pageSize)
        {
            var pageSizes = new[]
                                {
                                    new KeyValuePair<int, string>(PageSize, PageSize.ToString(CultureInfo.InvariantCulture)),
                                    new KeyValuePair<int, string>(25, "25"),
                                    new KeyValuePair<int, string>(50, "50"),
                                    new KeyValuePair<int, string>(100, "100")
                                };
            viewBag.PageSize = new SelectList(pageSizes, "Key", "Value", pageSize);
            viewBag.PageSizeCount = pageSize;
        }

        protected static void PopulateManualTypeId(dynamic viewBag, DefaultContext db, object selectedManualTypeId = null)
        {
            IOrderedQueryable<ManualType> manualTypes = db.ManualTypes.OrderBy(mt => mt.Order);
            viewBag.ManualTypeId = new SelectList(manualTypes, "ManualTypeId", "Title", selectedManualTypeId);
        }
    }

    /// <summary>
    /// Class SearchAccording
    /// </summary>
    public class SearchAccording
    {
        /// <summary>
        /// Gets or sets the search string according.
        /// </summary>
        /// <value>The search string according.</value>
        public string SearchStringAccording { get; set; }
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="SearchAccording" /> class from being created.
        /// </summary>
        private SearchAccording()
        {

        }

        public static IEnumerable<SearchAccording> GetUserProfileFilter()
        {
            var searchAccordings = new List<SearchAccording>
                                       {
                                           new SearchAccording
                                               {Title = FieldResource.Global_FirstName_Name, SearchStringAccording = BaseCache.FirstNameField},
                                           new SearchAccording
                                               {Title = FieldResource.Global_LastName_Name, SearchStringAccording = BaseCache.LastNameField},
                                           new SearchAccording
                                               {Title = FieldResource.Global_City_Name, SearchStringAccording = BaseCache.CityField},
                                           new SearchAccording
                                               {Title = FieldResource.Global_LyonessId_Name, SearchStringAccording = BaseCache.LyonessIdField},
                                       };
            return searchAccordings;
        }

        public static IEnumerable<SearchAccording> GetMeetingBusinessInfoSignUsignFilter()
        {
            var searchAccordings = new List<SearchAccording>
                                       {
                                           new SearchAccording
                                               {Title = FieldResource.Global_LastName_Name, SearchStringAccording = BaseCache.LastNameField},
                                           new SearchAccording
                                               {Title = FieldResource.Global_City_Name, SearchStringAccording = BaseCache.CityField}
                                       };
            return searchAccordings;
        }

        public static IEnumerable<SearchAccording> GetMeetingBusinessInfoMspEveningFilter()
        {
            var searchAccordings = new List<SearchAccording>
                                       {
                                           new SearchAccording
                                               {Title = FieldResource.Global_City_Name, SearchStringAccording = BaseCache.CityField},
                                           new SearchAccording
                                               {Title = FieldResource.Meeting_Organizer_Name, SearchStringAccording = BaseCache.OrganizerField}
                                       };
            return searchAccordings;
        }

        public static IEnumerable<SearchAccording> GetMeetingWebinarFilter()
        {
            var searchAccordings = new List<SearchAccording>
                                       {
                                           new SearchAccording
                                               {Title = FieldResource.Meeting_Organizer_Name, SearchStringAccording = BaseCache.OrganizerField}
                                       };
            return searchAccordings;
        }
    }
}