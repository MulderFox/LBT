using LBT.Cache;
using LBT.Extensions;
using LBT.Filters;
using LBT.Models;
using LBT.ModelViews;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace LBT.Controllers
{
    public partial class BaseController
    {
        protected void PopulatePageSize(int pageSize)
        {
            var pageSizes = new[]
                                {
                                    new KeyValuePair<int, string>(PageSize, PageSize.ToString(CultureInfo.InvariantCulture)),
                                    new KeyValuePair<int, string>(25, "25"),
                                    new KeyValuePair<int, string>(50, "50"),
                                    new KeyValuePair<int, string>(100, "100")
                                };
            ViewBag.PageSize = new SelectList(pageSizes, "Key", "Value", pageSize);
            ViewBag.PageSizeCount = pageSize;
        }

        protected void PopulateFilteredUserId(object selectedFilteredUserId = null)
        {
            IEnumerable<FilteredUser> filteredUserIds = FilteredUser.GetFilteredUsers(Db, UserId).DistinctBy(gfu => gfu.UserId);
            ViewBag.FilteredUserId = new SelectList(filteredUserIds, "UserId", "Title", selectedFilteredUserId);
            ViewBag.SelectedFilteredUserId = selectedFilteredUserId;
        }

        protected void PopulateLanguage(object selectedLcid = null)
        {
            ViewBag.LCIDDropDownList = new SelectList(SetLanguageAttribute.AvailableLCIDList, "Key", "Value", selectedLcid);
        }

        protected void PopulateSearchStringAccording(string searchString, string searchStringAccording)
        {
            if (String.IsNullOrEmpty(searchStringAccording))
            {
                searchStringAccording = BaseCache.LastNameField;
            }

            IEnumerable<SearchAccording> searchAccordings = SearchAccording.GetUserProfileFilter();
            ViewBag.SearchStringAccording = new SelectList(searchAccordings, BaseCache.SearchStringAccordingField, BaseCache.TitleField, searchStringAccording);
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentFilterAccording = searchStringAccording;
        }

        protected void PopulateSignUnsignSearchStringAccording(string searchString, string searchStringAccording)
        {
            if (String.IsNullOrEmpty(searchStringAccording))
            {
                searchStringAccording = "LastName";
            }

            IEnumerable<SearchAccording> searchAccordings = SearchAccording.GetMeetingBusinessInfoSignUsignFilter();
            ViewBag.SearchStringAccording = new SelectList(searchAccordings, "SearchStringAccording", "Title", searchStringAccording);
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentFilterAccording = searchStringAccording;
        }

        protected void PopulateDistrictId(object selectedDistrict = null)
        {
            IOrderedQueryable<District> districts = Db.Districts.OrderBy(d => d.Title);
            ViewBag.DistrictId = new SelectList(districts, "DistrictId", "Title", selectedDistrict);
        }

        protected void PopulateVideoId(int userId, bool isAdmin, object selectedVideo = null)
        {
            IOrderedQueryable<Video> videos = Db.Videos.Where(v => isAdmin || v.AllUsers || v.VideoUsers.Any(vu => vu.UserProfileId == userId)).OrderBy(v => v.Title);
            ViewBag.VideoId = new SelectList(videos, "VideoId", "Title", selectedVideo);
        }

        protected void PopulatePhoneNumberPrefix1Id(object selectedPhoneNumberPrefix = null)
        {
            IOrderedQueryable<PhoneNumberPrefix> phoneNumberPrefixes = Db.PhoneNumberPrefixes.OrderBy(pnp => pnp.Title);
            ViewBag.PhoneNumberPrefix1Id = new SelectList(phoneNumberPrefixes, "PhoneNumberPrefixId", "Title", selectedPhoneNumberPrefix);
        }

        protected void PopulatePhoneNumberPrefix2Id(object selectedPhoneNumberPrefix = null)
        {
            IOrderedQueryable<PhoneNumberPrefix> phoneNumberPrefixes = Db.PhoneNumberPrefixes.OrderBy(pnp => pnp.Title);
            ViewBag.PhoneNumberPrefix2Id = new SelectList(phoneNumberPrefixes, "PhoneNumberPrefixId", "Title", selectedPhoneNumberPrefix);
        }

        protected void PopulateBankAccountType(object selectedBankAccountType = null)
        {
            Dictionary<BankAccountType, string> translationDictionary = BankAccount.GetTranslationDictionaryForBankAccountType();
            ViewBag.BankAccountType = TEnumExtensions.ToSelectList(translationDictionary, selectedBankAccountType);
        }

        protected void PopulateCurrencyType(object selectedCurrencyType = null)
        {
            ViewBag.CurrencyType = TEnumExtensions.ToSelectList<CurrencyType>(selectedCurrencyType);
        }

        protected void PopulateAutomaticLogoutInterval(object selectedAutomaticLogoutInterval = null)
        {
            Dictionary<AutomaticLogoutIntervalType, string> translationDictionary = UserProfile.GetTranslationDictionaryForAutomaticLogoutIntervalType();
            ViewBag.AutomaticLogoutInterval = TEnumExtensions.ToSelectList(translationDictionary, selectedAutomaticLogoutInterval ?? AutomaticLogoutIntervalType.TenMinutes);
        }

        protected void PopulateCurrencyTypeExceptCZK(object selectedCurrencyType = null)
        {
            CurrencyType[] existingCurrencyTypes = Db.Currencies.Select(c => c.CurrencyType).Union(new[] { CurrencyType.CZK }).ToArray();
            ViewBag.CurrencyType = TEnumExtensions.ToSelectList(selectedCurrencyType, existingCurrencyTypes);
        }

        protected void PopulateStatus(object selectedStatus = null)
        {
            ViewBag.Status = TEnumExtensions.ToSelectList<TopTenStatus>(selectedStatus ?? TopTenStatus.A);
        }

        protected void PopulateRole(object selectedRole = null)
        {
            Dictionary<RoleType, string> translationDictionary = UserProfile.GetTranslationDictionaryForRoleType();
            ViewBag.Role = TEnumExtensions.ToSelectList(translationDictionary, selectedRole ?? RoleType.Leader, new[] { RoleType.Unknown });
        }

        protected void PopulateSecondaryOrganizerId(int mainLeaderUserId, int? secondaryLeaderUserId, object selectedUserProfile = null)
        {
            IEnumerable<PopulatedUser> users = secondaryLeaderUserId.HasValue
                                                   ? PopulatedUser.GetDownlineUsers(Db, mainLeaderUserId, secondaryLeaderUserId.GetValueOrDefault())
                                                   : PopulatedUser.GetDownlineUsers(Db, mainLeaderUserId);

            ViewBag.SecondaryOrganizerId = new SelectList(users, "UserId", "Title", selectedUserProfile);
        }

        protected void PopulateMainLeaderId(int userId, object selectedMainLeaderId = null)
        {
            IEnumerable<PopulatedUser> mainLeaders = PopulatedUser.GetUplineUsers(Db, userId);
            ViewBag.MainLeaderId = new SelectList(mainLeaders, "UserId", "Title", selectedMainLeaderId);
        }

        protected void PopulateSecondaryLeaderId(object selectedSecondaryLeaderId = null)
        {
            IEnumerable<PopulatedUser> secondaryLeaders = PopulatedUser.GetUsers(Db);
            ViewBag.SecondaryLeaderId = new SelectList(secondaryLeaders, "UserId", "Title", selectedSecondaryLeaderId);
        }

        protected void PopulateToUplineUserId(object selectedToUplineUserId = null)
        {
            IEnumerable<PopulatedUser> populateUplineUsers = PopulatedUser.GetUplineUsersWithoutSharedContacts(Db, UserId);
            ViewBag.ToUplineUserId = new SelectList(populateUplineUsers, "UserId", "Title", selectedToUplineUserId);
        }

        protected void PopulateToDownlineUserId(object selectedToDownlineUserId = null)
        {
            IEnumerable<PopulatedUser> populatedDownlineUsers = PopulatedUser.GetDownlineUsersWithoutSharedContacts(Db, UserId);
            ViewBag.ToDownlineUserId = new SelectList(populatedDownlineUsers, "UserId", "Title", selectedToDownlineUserId);
        }

        protected void PopulateToAnyUserId(object selectedToAnyUserId = null)
        {
            IEnumerable<PopulatedUser> populatedAnyUsers = PopulatedUser.GetAnyUsersWithoutSharedContacts(Db, UserId);
            ViewBag.ToAnyUserId = new SelectList(populatedAnyUsers, "UserId", "Title", selectedToAnyUserId);
        }

        protected void PopulateToUserId(object selectedToUserId = null)
        {
            IEnumerable<PopulatedUser> populatedDownlineUsers = PopulatedUser.GetDownlineUsersWithoutTopTenContacts(Db, UserId);
            ViewBag.ToUserId = new SelectList(populatedDownlineUsers, "UserId", "Title", selectedToUserId);
        }

        protected void PopulateClaAccessChangedCurrency(decimal fromAmount, CurrencyType fromCurrencyType, object selectedClaAccessChangedCurrency = null)
        {
            PopulatedCurrency[] populatedCurrencies = PopulatedCurrency.GetClaAccessChangedCurrency(Db, fromAmount, fromCurrencyType);
            if (populatedCurrencies.Length < 2)
            {
                ViewBag.ClaAccessChangedCurrencyEnabled = false;
                return;
            }

            ViewBag.ClaAccessChangedCurrencyEnabled = true;
            ViewBag.ClaAccessChangedCurrency = new SelectList(populatedCurrencies, "CurrencyType", "Title", selectedClaAccessChangedCurrency);
            ViewBag.ClaAccessChangedInfoList = populatedCurrencies;
        }

        protected void PopulateMeetingType(object selectedMeetingType = null)
        {
            Dictionary<MeetingType, string> translationDictionary = Meeting.GetTranslationDictionaryForBankAccountType();
            ViewBag.MeetingType = TEnumExtensions.ToSelectList(translationDictionary, selectedMeetingType);
        }

        protected void PopulateMeetingTitleTypeId(MeetingType meetingType, object selectedMeetingTitleTypeId = null)
        {
            IOrderedQueryable<MeetingTitleType> meetingTitleTypesQuery = Db.MeetingTitleTypes.Where(mtt => mtt.MeetingType == meetingType).OrderBy(mtt => mtt.Title);
            ViewBag.MeetingTitleTypeId = new SelectList(meetingTitleTypesQuery, "MeetingTitleTypeId", "Title", selectedMeetingTitleTypeId);
        }

        protected void PopulateRegistrarId(object selectedRegistrarId = null)
        {
            UserProfile[] userProfiles = UserProfileCache.GetIndex(Db);
            ViewBag.RegistrarId = new SelectList(userProfiles, "UserId", "UserName", selectedRegistrarId);
        }

        protected void PopulateBankAccountId(BankAccountType bankAccountType, int userId, object selectedBankAccountId = null)
        {
            BankAccount[] bankAccounts = BankAccountCache.GetIndex(Db, bankAccountType, userId);
            ViewBag.BankAccountId = new SelectList(bankAccounts, "BankAccountId", "Title", selectedBankAccountId);
        }

        protected void PopulateSecondBankAccountId(BankAccountType bankAccountType, int userId, object selectedSecondBankAccountId = null)
        {
            BankAccount[] bankAccounts = BankAccountCache.GetIndex(Db, bankAccountType, userId);
            ViewBag.SecondBankAccountId = new SelectList(bankAccounts, "BankAccountId", "Title", selectedSecondBankAccountId);
        }

        protected void PopulateMemberIds(object selectedMemberIds = null)
        {
            IEnumerable<PopulatedUser> members = PopulatedUser.GetUsers(Db);
            ViewBag.MemberIds = new SelectList(members, "UserId", "Title", selectedMemberIds);
        }

        protected void PopulateUserIds(object selectedUserIds = null)
        {
            IEnumerable<PopulatedUser> users = PopulatedUser.GetUsers(Db);
            ViewBag.UserIds = new SelectList(users, "UserId", "Title", selectedUserIds);
        }

        protected void PopulateMeetingSearchStringAccording(MeetingType meetingType, string searchString, string searchStringAccording)
        {
            if (String.IsNullOrEmpty(searchStringAccording) && meetingType != MeetingType.Webinar)
            {
                searchStringAccording = BaseCache.CityField;
            }
            else if (String.IsNullOrEmpty(searchStringAccording))
            {
                searchStringAccording = BaseCache.OrganizerField;
            }

            IEnumerable<SearchAccording> searchAccordings = meetingType != MeetingType.Webinar
                                                                ? SearchAccording.GetMeetingBusinessInfoMspEveningFilter()
                                                                : SearchAccording.GetMeetingWebinarFilter();
            ViewBag.SearchStringAccording = new SelectList(searchAccordings, BaseCache.SearchStringAccordingField, BaseCache.TitleField, searchStringAccording);
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentFilterAccording = searchStringAccording;
        }
    }
}
