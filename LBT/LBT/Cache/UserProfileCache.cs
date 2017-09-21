using LBT.DAL;
using LBT.Helpers;
using LBT.Models;
using LBT.ModelViews;
using LBT.Resources;
using LBT.Services;
using LBT.Services.GoogleApis;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace LBT.Cache
{
    public class UserProfileCache : BaseCache
    {
        private const string RegistrarIsNullErrorMessage = "Cannot change main leader to upline for the most top user.";

        public static UserProfile[] GetIndex(DefaultContext db)
        {
            IQueryable<UserProfile> userProfiles = db.UserProfiles;
            return userProfiles.ToArray();
        }

        public static UserProfile[] GetAdmins(DefaultContext db)
        {
            IEnumerable<UserProfile> userProfiles = db.Database.SqlQuery<UserProfile>(GetAdminsProcedureTemplate);
            return userProfiles.ToArray();
        }

        /// <summary>
        /// Gets the index of the user profile.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="searchString">The search string.</param>
        /// <param name="searchStringAccording">The search string according.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns>IEnumerable{UserProfileIndex}.</returns>
        public static UserProfileIndex[] GetUserProfileIndex(DefaultContext db, int userId, string searchString, string searchStringAccording, string sortOrder)
        {
            var parameter = new SqlParameter(UserIdSqlParameter, userId);
            UserProfileIndex[] userProfiles = db.Database.SqlQuery<UserProfileIndex>(GetUserProfileIndexProcedureTemplate, parameter).ToArray();
            foreach (UserProfileIndex userProfileIndex in userProfiles)
            {
                userProfileIndex.IsEditable = !userProfileIndex.IsPoliciesAccepted;
            }

            userProfiles = GetUserProfileIndex(userProfiles, searchString, searchStringAccording, sortOrder);
            return userProfiles;
        }

        /// <summary>
        /// Gets the user profile index for admin.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="searchString">The search string.</param>
        /// <param name="searchStringAccording">The search string according.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns>IEnumerable{UserProfileIndex}.</returns>
        public static UserProfileIndex[] GetUserProfileIndexForAdmin(DefaultContext db, int userId, string searchString, string searchStringAccording, string sortOrder)
        {
            var parameter = new SqlParameter(UserIdSqlParameter, userId);
            UserProfileIndex[] userProfiles = db.Database.SqlQuery<UserProfileIndex>(GetUserProfileIndexForAdminProcedureTemplate, parameter).ToArray();

            userProfiles = GetUserProfileIndex(userProfiles, searchString, searchStringAccording, sortOrder);
            return userProfiles;
        }

        /// <summary>
        /// Gets the user profile index tree.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>IEnumerable{UserProfileIndexTree}.</returns>
        public static UserProfileIndexTree[] GetUserProfileIndexTree(DefaultContext db, int userId)
        {
            var parameter = new SqlParameter(UserIdSqlParameter, userId);
            UserProfileIndexTree[] userProfiles = db.Database.SqlQuery<UserProfileIndexTree>(GetUserProfileIndexTreeProcedureTemplate, parameter).ToArray();
            foreach (UserProfileIndexTree userProfileIndexTree in userProfiles)
            {
                userProfileIndexTree.IsEditable = !userProfileIndexTree.IsPoliciesAccepted;
            }

            return userProfiles;
        }

        /// <summary>
        /// Gets the user profile index tree for admin.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>IEnumerable{UserProfileIndexTree}.</returns>
        public static UserProfileIndexTree[] GetUserProfileIndexTreeForAdmin(DefaultContext db, int userId)
        {
            var parameter = new SqlParameter(UserIdSqlParameter, userId);
            UserProfileIndexTree[] userProfiles = db.Database.SqlQuery<UserProfileIndexTree>(GetUserProfileIndexTreeForAdminProcedureTemplate, parameter).ToArray();
            return userProfiles;
        }

        /// <summary>
        /// Gets the leaded users.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="user">The user.</param>
        /// <returns>IQueryable{UserProfile}.</returns>
        public static IQueryable<UserProfile> GetLeadedUsers(DefaultContext db, int userId, IPrincipal user)
        {
            IQueryable<UserProfile> userProfiles = db.UserProfiles.Include(p => p.PhoneNumberPrefix1);

            if (!user.IsInRole(UserProfile.GetRoleTypeDbName(RoleType.Leader)))
            {
                return userProfiles.Where(u => u.UserId != userId);
            }

            int[] leadedUserIds = GetLeadedUserIds(db, userId).ToArray();
            return userProfiles.Where(u => leadedUserIds.Contains(u.UserId) && u.UserId != userId);
        }

        /// <summary>
        /// Gets the leaded user ids.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>IEnumerable{System.Int32}.</returns>
        public static int[] GetLeadedUserIds(DefaultContext db, int userId)
        {
            var parameter = new SqlParameter(UserIdSqlParameter, userId);
            int[] leadedUserIds = db.Database.SqlQuery<int>(GetLeadedUserIdsProcedureTemplate, parameter).ToArray();
            return leadedUserIds;
        }

        public static IQueryable<UserProfile> GetSharedFromUsers(DefaultContext db, int userId)
        {
            IQueryable<UserProfile> userProfiles = from s in db.SharedContacts
                                                   join u in db.UserProfiles on s.FromUserId equals u.UserId
                                                   where s.ToUserId == userId
                                                   select u;
            return userProfiles;
        }

        public class UserProfileForPopulated
        {
            public int UserId { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string LyonessId { get; set; }
        }

        public static IEnumerable<UserProfileForPopulated> GetUplineUsers(DefaultContext db, int userId)
        {
            IEnumerable<UserProfileForPopulated> uplineUsers = db.Database.SqlQuery<UserProfileForPopulated>(String.Format(GetUplineUsersProcedureTemplate, userId));
            return uplineUsers;
        }

        public static IEnumerable<UserProfileForPopulated> GetDownlineUsers(DefaultContext db, int userId)
        {
            IEnumerable<UserProfileForPopulated> downlineUsers = db.Database.SqlQuery<UserProfileForPopulated>(String.Format(GetDownlineUsersAllProcedureTemplate, userId));
            return downlineUsers;
        }

        public static IEnumerable<UserProfileForPopulated> GetUsers(DefaultContext db)
        {
            IEnumerable<UserProfileForPopulated> userProfileForPopulatedList = db.UserProfiles.Select(u => new UserProfileForPopulated
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                LyonessId = u.LyonessId,
                UserId = u.UserId
            });

            return userProfileForPopulatedList;
        }

        public static int[] GetDownlineUserIdsWithoutAdmins(DefaultContext db, params int?[] userIds)
        {
            var downlineUserIdsWithoutAdmins = new List<int>();
            foreach (int userId in userIds.Where(u => u.HasValue))
            {
                var sqlParameter = new SqlParameter(UserIdSqlParameter, userId);
                downlineUserIdsWithoutAdmins.AddRange(db.Database.SqlQuery<int>(GetDownlineUsersProcedureTemplate, sqlParameter));
            }

            return downlineUserIdsWithoutAdmins.Distinct().ToArray();
        }

        public static int[] GetDownlineUserIds(DefaultContext db, params int?[] userIds)
        {
            var downlineUserIds = new List<int>();
            foreach (int userId in userIds.Where(u => u.HasValue))
            {
                var sqlParameter = new SqlParameter(UserIdSqlParameter, userId);
                downlineUserIds.AddRange(db.Database.SqlQuery<int>(GetDonwlinesProcedureTemplate, sqlParameter));
            }

            return downlineUserIds.Distinct().ToArray();
        }

        public static int[] GetDownlineAndUplineUsersIds(DefaultContext db, int userId)
        {
            var sqlParameter = new SqlParameter(UserIdSqlParameter, userId);
            int[] downlineAndUplineUserIds = db.Database.SqlQuery<int>(GetDownlinesAndUplinesProcedureTemplate, sqlParameter).ToArray();
            return downlineAndUplineUserIds;
        }

        public static int[] GetAnyUsers(DefaultContext db)
        {
            int[] anyUsersIds = db.Database.SqlQuery<int>(GetAnyUsersProcedureTemplate).ToArray();
            return anyUsersIds;
        }

        public static UserProfile GetDetail(DefaultContext db, int id)
        {
            UserProfile userProfile = db.UserProfiles.Find(id);
            if (userProfile != null)
            {
                userProfile.Role = UserProfile.GetRoleForUser(userProfile.UserName);
            }

            return userProfile;
        }

        public static UserProfile GetDetail(DefaultContext db, string userName = null, string lyonessId = null)
        {
            UserProfile userProfile;
            if (!String.IsNullOrEmpty(userName))
            {
                userProfile = GetDetailByUserName(db, userName);
                return userProfile;
            }

            userProfile = GetDetailByLyonessId(db, lyonessId);
            return userProfile;
        }

        public static void Login(DefaultContext db, Login login, out bool getGoogleToken)
        {
            UserProfile userProfile = GetDetail(db, login.UserId);
            userProfile.LastAccessed = DateTime.Now;
            db.SaveChanges();

            getGoogleToken = !String.IsNullOrEmpty(userProfile.GoogleCredentialsJson) && userProfile.UseGoogleCalendar;
        }

        public static bool IsPhone1Unique(DefaultContext db, int phoneNumberPrefix1Id, string phoneNumber1)
        {
            bool isPhone1Unique = !db.UserProfiles.Any(u => u.PhoneNumberPrefix1Id == phoneNumberPrefix1Id && u.PhoneNumber1.Equals(phoneNumber1, StringComparison.InvariantCultureIgnoreCase));
            return isPhone1Unique;
        }

        public static bool IsLyonessIdUnique(DefaultContext db, string lyonessId)
        {
            bool isLyonessIdUnique = !db.UserProfiles.Any(u => u.LyonessId.Equals(lyonessId));
            return isLyonessIdUnique;
        }

        public static bool IsLyonessIdUnique(DefaultContext db, int userId, string lyonessId)
        {
            bool isLyonessIdUnique = !db.UserProfiles.Any(u => u.UserId != userId && u.LyonessId.Equals(lyonessId));
            return isLyonessIdUnique;
        }

        public enum RegisterResult
        {
            Ok,
            CannotSendRegisterEmail,
            Failed
        }

        public static ModelStateDictionary Register(DefaultContext db, Register model, out RegisterResult registerResult)
        {
            var modelStateDictionary = new ModelStateDictionary();

            // Attempt to register the user
            try
            {
                var propertyValues = new
                {
                    model.FirstName,
                    model.LastName,
                    model.Title,
                    model.City,
                    model.DistrictId,
                    model.PhoneNumberPrefix1Id,
                    model.PhoneNumber1,
                    model.PhoneNumberPrefix2Id,
                    model.PhoneNumber2,
                    model.Email1,
                    model.Email2,
                    model.GoogleCredentialsJson,
                    model.GoogleCalendarId,
                    model.PremiumMembershipGranted,
                    model.AccessGranted,
                    model.Ca,
                    model.Presenting,
                    model.MspCoach,
                    model.RegistrarId,
                    Active = true,
                    model.LyonessId,
                    model.Note,
                    model.Tasks,
                    model.Skype,
                    model.SmsEmail,
                    UseMail = true,
                    UseSms = true,
                    UseGoogleCalendar = true,
                    model.ClaAccessExpired,
                    model.ClaAccessYearlyAccessCZK,
                    model.ClaAccessYearlyAccessEUR,
                    model.ClaAccessYearlyAccessUSD,
                    ClaAccessCurrency = model.CurrencyType,
                    model.ClaAccessTrial,
                    model.ClaAccessFixCurrencyChange,
                    model.LCID
                };
                string password = Cryptography.GetRandomPassword();
                WebSecurity.CreateUserAndAccount(model.UserName, password, propertyValues);
                Roles.AddUserToRole(model.UserName, UserProfile.GetRoleTypeDbName(RoleType.AuthGuest));

                string textBody = String.Format(MailResource.AccountController_Register_TextBody, model.UserName, password, Url.GetWebRootUrl());
                if (!Mail.SendEmail(model.Email1, MailResource.AccountController_Register_Subject, textBody, true, true))
                {
                    registerResult = RegisterResult.CannotSendRegisterEmail;
                    return modelStateDictionary;
                }
            }
            catch (MembershipCreateUserException e)
            {
                string message = String.Format("Register New User '{0}' - {1}", model.UserName, e.Message);
                Logger.SetLog(message);
                registerResult = RegisterResult.Failed;
                modelStateDictionary.AddModelError(EmptyField, DecodeMembershipCreateStatus(e.StatusCode));
                return modelStateDictionary;
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
                registerResult = RegisterResult.Failed;
                modelStateDictionary.AddModelError(EmptyField, ValidationResource.Global_UnexpectedException_ErrorMessage);
                return modelStateDictionary;
            }

            registerResult = RegisterResult.Ok;
            return modelStateDictionary;
        }

        public static bool Update(DefaultContext db, UserProfileEdit userProfileEdit, bool isAdmin, out UserProfile userProfile)
        {
            userProfile = null;

            UserProfile dbUserProfile = GetDetail(db, userProfileEdit.UserId);
            if (dbUserProfile == null)
                return false;

            dbUserProfile.CopyFrom(userProfileEdit, isAdmin);
            db.SaveChanges();

            userProfile = dbUserProfile;

            if (!isAdmin)
                return true;

            ChangeUserRole(dbUserProfile.UserName, userProfileEdit.Role);

            return true;
        }

        public static bool Update(DefaultContext db, SettingsProfile settingsProfile, out UserProfile userProfile)
        {
            userProfile = null;

            UserProfile dbUserProfile = GetDetail(db, settingsProfile.UserId);
            if (dbUserProfile == null)
                return false;

            dbUserProfile.CopyFrom(settingsProfile);
            db.SaveChanges();

            userProfile = dbUserProfile;

            return true;
        }

        public static bool Update(DefaultContext db, CheckBillingInformation checkBillingInformation, out UserProfile userProfile)
        {
            userProfile = null;

            UserProfile dbUserProfile = GetDetail(db, checkBillingInformation.UserId);
            if (dbUserProfile == null)
                return false;

            dbUserProfile.CopyFrom(checkBillingInformation);
            db.SaveChanges();

            userProfile = dbUserProfile;

            return true;
        }

        public static DeleteResult Delete(DefaultContext db, UrlHelper urlHelper, int id, int userId, bool isAdmin, out UserProfile userProfile)
        {
            userProfile = null;

            if (id == 1)
                return DeleteResult.AdminAccountProtection;

            if (id == userId)
                return DeleteResult.AuthorizationFailed;

            DeleteResult deleteResult = Delete(db, urlHelper, id, true, out userProfile, userId, isAdmin);
            return deleteResult;
        }

        public static DeleteResult Delete(DefaultContext db, UrlHelper urlHelper, int id)
        {
            if (id == 1)
                return DeleteResult.AdminAccountProtection;

            UserProfile userProfile;
            DeleteResult deleteResult = Delete(db, urlHelper, id, false, out userProfile);
            return deleteResult;
        }

        private static DeleteResult Delete(DefaultContext db, UrlHelper urlHelper, int id, bool checkLeadedUser, out UserProfile userProfile, int userId = 0, bool isAdmin = false)
        {
            userProfile = GetDetail(db, id);
            if (userProfile == null)
                return DeleteResult.AuthorizationFailed;

            if (checkLeadedUser)
            {
                bool isUserLeader = GetLeadedUserIds(db, userId).Contains(userProfile.UserId);
                if (!isAdmin && !isUserLeader)
                {
                    return DeleteResult.AuthorizationFailed;
                }
            }

            string[] roles = Roles.GetRolesForUser(userProfile.UserName);
            try
            {
                UserProfile registrar = GetDetail(db, userProfile.RegistrarId.GetValueOrDefault());
                ModifyMeetingAttendees(db, urlHelper, userProfile, registrar);
                ModifyMeetings(db, urlHelper, userProfile, registrar);

                var parameter = new SqlParameter(UserIdSqlParameter, userProfile.UserId);
                db.Database.ExecuteSqlCommand(CascadeRemoveUserProcedureTemplate, parameter);
            }
            catch (Exception e)
            {
                if (!Roles.GetRolesForUser(userProfile.UserName).Any())
                    Roles.AddUserToRoles(userProfile.UserName, roles);

                Logger.SetLog(e);

                return DeleteResult.DbFailed;
            }

            try
            {
                var parameter = new SqlParameter(UserIdSqlParameter, userProfile.UserId);
                db.Database.ExecuteSqlCommand(CascadeRemoveUserProfileProcedureTemplate, parameter);
                return DeleteResult.Ok;
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
                return DeleteResult.DbFailed;
            }
        }

        private static void ModifyMeetingAttendees(DefaultContext db, UrlHelper urlHelper, UserProfile userProfile, UserProfile registrar)
        {
            DateTime currentDateTime = DateTime.Now;
            bool modified = false;

            MeetingAttendee[] meetingAttendeesByRegistrar = db.MeetingAttendees.Where(ma => ma.RegistrarId == userProfile.UserId).ToArray();
            foreach (MeetingAttendee meetingAttendee in meetingAttendeesByRegistrar)
            {
                if (registrar == null)
                    throw new Exception(RegistrarIsNullErrorMessage);

                meetingAttendee.RegistrarId = registrar.UserId;
                meetingAttendee.Registrar = registrar;
                modified = true;
            }

            var mailMessages = new List<MailMessage>();
            MeetingAttendee[] meetingAttendeesByUserAttendee = db.MeetingAttendees.Where(ma => ma.UserAttendeeId == userProfile.UserId && ma.Meeting.Finished > currentDateTime && userProfile.UseMail).Include(ma => ma.Meeting).ToArray();
            // ReSharper disable LoopCanBeConvertedToQuery
            foreach (MeetingAttendee meetingAttendee in meetingAttendeesByUserAttendee)
            // ReSharper restore LoopCanBeConvertedToQuery
            {
                string meetingUrl = Url.GetActionAbsoluteUrl(urlHelper, MeetingCache.GetMeetingAction(meetingAttendee.Meeting), "Meeting", new { id = meetingAttendee.Meeting.MeetingId });
                string textBody = String.Format(MailResource.UserProfileController_UserAttendeeWasRemoved_TextBody, meetingAttendee.Meeting.Title, meetingUrl);
                var mailMessage = new MailMessage { Address = userProfile.Email1, Subject = MailResource.UserProfileController_UserAttendeeWasRemoved_Subject, TextBody = textBody };
                mailMessages.Add(mailMessage);
            }

            int userId = userProfile.UserId;
            IQueryable<int> peopleContactIds = db.PeopleContacts.Where(pc => pc.RegistrarId == userId).Select(pc => pc.PeopleContactId);
            MeetingAttendee[] meetingAttendeesByAttendee = db.MeetingAttendees.Where(ma => ma.AttendeeId != null && peopleContactIds.Contains(ma.AttendeeId.Value) && ma.Meeting.Finished > currentDateTime && userProfile.UseMail).Include(ma => ma.Meeting).ToArray();
            // ReSharper disable LoopCanBeConvertedToQuery
            foreach (MeetingAttendee meetingAttendee in meetingAttendeesByAttendee)
            // ReSharper restore LoopCanBeConvertedToQuery
            {
                string meetingUrl = Url.GetActionAbsoluteUrl(urlHelper, MeetingCache.GetMeetingAction(meetingAttendee.Meeting), "Meeting", new { id = meetingAttendee.Meeting.MeetingId });
                string textBody = String.Format(MailResource.UserProfileController_AttendeeWasRemoved_TextBody, meetingAttendee.Attendee.FullName, meetingAttendee.Meeting.Title, meetingUrl);
                var mailMessage = new MailMessage { Address = userProfile.Email1, Subject = MailResource.UserProfileController_AttendeeWasRemoved_Subject, TextBody = textBody };
                mailMessages.Add(mailMessage);
            }

            if (modified)
            {
                db.SaveChanges();
            }

            foreach (MailMessage mailMessage in mailMessages)
            {
                Mail.SendEmail(mailMessage.Address, mailMessage.Subject, mailMessage.TextBody, true, true);
            }
        }

        private static void ModifyMeetings(DefaultContext db, UrlHelper urlHelper, UserProfile userProfile, UserProfile registrar)
        {
            var meetings =
                db.Meetings.Where(
                    m =>
                    m.OrganizerId == userProfile.UserId || m.MainLeaderId == userProfile.UserId ||
                    m.SecondaryOrganizerId == userProfile.UserId || m.SecondaryLeaderId == userProfile.UserId).Include(
                        m => m.MainLeader).ToArray();

            DateTime currentDateTime = DateTime.Now;
            var mailMessages = new List<MailMessage>();
            bool modified = false;
            foreach (Meeting meeting in meetings)
            {
                string meetingUrl = Url.GetActionAbsoluteUrl(urlHelper, MeetingCache.GetMeetingAction(meeting), "Meeting", new { id = meeting.MeetingId });

                if (meeting.MainLeaderId == userProfile.UserId)
                {
                    if (registrar == null)
                        throw new Exception(RegistrarIsNullErrorMessage);

                    meeting.MainLeaderId = registrar.UserId;
                    meeting.MainLeader = registrar;
                    modified = true;

                    if (meeting.Finished > currentDateTime && registrar.UseMail)
                    {
                        string textBody = String.Format(MailResource.UserProfileController_MainLeaderChanged_TextBody, meeting.Title, meetingUrl);
                        var mailMessage = new MailMessage { Address = registrar.Email1, Subject = MailResource.UserProfileController_MainLeaderChanged_Subject, TextBody = textBody };
                        mailMessages.Add(mailMessage);
                    }
                }

                if (meeting.OrganizerId == userProfile.UserId)
                {
                    meeting.OrganizerId = meeting.MainLeaderId;
                    modified = true;

                    if (meeting.Finished > currentDateTime && meeting.MainLeader.UseMail)
                    {
                        string textBody = String.Format(MailResource.UserProfileController_OrganizerChanged_TextBody, meeting.Title, meetingUrl);
                        var mailMessage = new MailMessage { Address = meeting.MainLeader.Email1, Subject = MailResource.UserProfileController_OrganizerChanged_Subject, TextBody = textBody };
                        mailMessages.Add(mailMessage);
                    }
                }

                if (meeting.SecondaryOrganizerId == userProfile.UserId)
                {
                    meeting.SecondaryOrganizerId = null;
                    meeting.SecondaryOrganizer = null;
                    modified = true;
                }

                if (meeting.SecondaryLeaderId == userProfile.UserId)
                {
                    if (registrar == null)
                        throw new Exception(RegistrarIsNullErrorMessage);

                    meeting.SecondaryLeaderId = registrar.UserId;
                    meeting.SecondaryLeader = registrar;
                    modified = true;

                    if (meeting.Finished > currentDateTime && registrar.UseMail)
                    {
                        string textBody = String.Format(MailResource.UserProfileController_SecondaryLeaderChanged_TextBody, meeting.Title, meetingUrl);
                        var mailMessage = new MailMessage { Address = registrar.Email1, Subject = MailResource.UserProfileController_SecondaryLeaderChanged_Subject, TextBody = textBody };
                        mailMessages.Add(mailMessage);
                    }
                }
            }

            if (modified)
            {
                db.SaveChanges();
            }

            foreach (MailMessage mailMessage in mailMessages)
            {
                Mail.SendEmail(mailMessage.Address, mailMessage.Subject, mailMessage.TextBody, true, true);
            }
        }

        private class MailMessage
        {
            public string Address;
            public string Subject;
            public string TextBody;
        }

        private static UserProfileIndex[] GetUserProfileIndex(UserProfileIndex[] userProfiles, string searchString, string searchStringAccording, string sortOrder)
        {
            IEnumerable<UserProfileIndex> linqUserProfiles = userProfiles;
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (searchStringAccording)
                {
                    case FirstNameField:
                        linqUserProfiles = userProfiles.Where(up => up.FirstName.ToUpper().Contains(searchString.ToUpper()));
                        break;

                    case LastNameField:
                        linqUserProfiles = userProfiles.Where(up => up.LastName.ToUpper().Contains(searchString.ToUpper()));
                        break;

                    case CityField:
                        linqUserProfiles = userProfiles.Where(up => up.City.ToUpper().Contains(searchString.ToUpper()));
                        break;

                    case LyonessIdField:
                        linqUserProfiles = userProfiles.Where(up => up.LyonessId.ToUpper().Contains(searchString.ToUpper()));
                        break;
                }
            }

            switch (sortOrder)
            {
                case LastNameDescSortOrder:
                    linqUserProfiles = linqUserProfiles.OrderByDescending(up => up.LastName);
                    break;

                case FirstNameAscSortOrder:
                    linqUserProfiles = linqUserProfiles.OrderBy(up => up.FirstName);
                    break;

                case FirstNameDescSortOrder:
                    linqUserProfiles = linqUserProfiles.OrderByDescending(up => up.FirstName);
                    break;

                case CityAscSortOrder:
                    linqUserProfiles = linqUserProfiles.OrderBy(up => up.City);
                    break;

                case CityDescSortOrder:
                    linqUserProfiles = linqUserProfiles.OrderByDescending(up => up.City);
                    break;

                case LyonessIdAscSortOrder:
                    linqUserProfiles = linqUserProfiles.OrderBy(up => up.LyonessId);
                    break;

                case LyonessIdDescSortOrder:
                    linqUserProfiles = linqUserProfiles.OrderByDescending(up => up.LyonessId);
                    break;

                default:
                    linqUserProfiles = linqUserProfiles.OrderBy(up => up.LastName);
                    break;
            }

            return linqUserProfiles.ToArray();
        }

        public static IQueryable<UserProfilePrintIndex> GetLeadedUsers(DefaultContext db, int userId, IPrincipal user, string currentFilter, string currentFilterAccording, string sortOrder)
        {
            IQueryable<UserProfile> userProfiles = GetLeadedUsers(db, userId, user);

            if (!String.IsNullOrEmpty(currentFilter))
            {
                switch (currentFilterAccording)
                {
                    case FirstNameField:
                        userProfiles = userProfiles.Where(up => up.FirstName.ToUpper().Contains(currentFilter.ToUpper()));
                        break;

                    case LastNameField:
                        userProfiles = userProfiles.Where(up => up.LastName.ToUpper().Contains(currentFilter.ToUpper()));
                        break;

                    case CityField:
                        userProfiles = userProfiles.Where(up => up.City.ToUpper().Contains(currentFilter.ToUpper()));
                        break;

                    case LyonessIdField:
                        userProfiles = userProfiles.Where(up => up.LyonessId.ToUpper().Contains(currentFilter.ToUpper()));
                        break;
                }
            }

            switch (sortOrder)
            {
                case LastNameDescSortOrder:
                    userProfiles = userProfiles.OrderByDescending(up => up.LastName);
                    break;

                case FirstNameAscSortOrder:
                    userProfiles = userProfiles.OrderBy(up => up.FirstName);
                    break;

                case FirstNameDescSortOrder:
                    userProfiles = userProfiles.OrderByDescending(up => up.FirstName);
                    break;

                case CityAscSortOrder:
                    userProfiles = userProfiles.OrderBy(up => up.City);
                    break;

                case CityDescSortOrder:
                    userProfiles = userProfiles.OrderByDescending(up => up.City);
                    break;

                case LyonessIdAscSortOrder:
                    userProfiles = userProfiles.OrderBy(up => up.LyonessId);
                    break;

                case LyonessIdDescSortOrder:
                    userProfiles = userProfiles.OrderByDescending(up => up.LyonessId);
                    break;

                case PremiumMembershipGrantedAscSortOrder:
                    userProfiles = userProfiles.OrderBy(up => up.PremiumMembershipGranted.HasValue);
                    break;

                case PremiumMembershipGrantedDescSortOrder:
                    userProfiles = userProfiles.OrderByDescending(up => up.PremiumMembershipGranted.HasValue);
                    break;

                default:
                    userProfiles = userProfiles.OrderBy(up => up.LastName);
                    break;
            }

            IQueryable<UserProfilePrintIndex> userProfilePrintIndex = userProfiles.Select(userProfile => new UserProfilePrintIndex
            {
                City = userProfile.City,
                Email1 = userProfile.Email1,
                FirstName = userProfile.FirstName,
                LastName = userProfile.LastName,
                // Kvůli LINQ Entites nelze použít String.Format
                PhoneNumber1 = userProfile.PhoneNumberPrefix1.Title + " " + userProfile.PhoneNumber1,
                PremiumMembershipGranted = userProfile.PremiumMembershipGranted,
                LyonessId = userProfile.LyonessId
            });
            return userProfilePrintIndex;
        }

        private static void ChangeUserRole(string userName, RoleType toRole)
        {
            string[] rolesForUser = Roles.GetRolesForUser(userName);
            string roleTypeDbName = UserProfile.GetRoleTypeDbName(toRole);
            if (rolesForUser.Contains(roleTypeDbName))
                return;

            if (rolesForUser.Any())
            {
                Roles.RemoveUserFromRoles(userName, rolesForUser);
            }

            Roles.AddUserToRole(userName, roleTypeDbName);
        }

        private static string DecodeMembershipCreateStatus(MembershipCreateStatus membershipCreateStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (membershipCreateStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return ValidationResource.MembershipCreateStatus_DuplicateUserName_ErrorMessage;

                case MembershipCreateStatus.DuplicateEmail:
                    return ValidationResource.MembershipCreateStatus_DuplicateEmail_ErrorMessage;

                case MembershipCreateStatus.InvalidPassword:
                    return ValidationResource.MembershipCreateStatus_InvalidPassword_ErrorMessage;

                case MembershipCreateStatus.InvalidEmail:
                    return ValidationResource.MembershipCreateStatus_InvalidEmail_ErrorMessage;

                case MembershipCreateStatus.InvalidAnswer:
                    return ValidationResource.MembershipCreateStatus_InvalidAnswer_ErrorMessage;

                case MembershipCreateStatus.InvalidQuestion:
                    return ValidationResource.MembershipCreateStatus_InvalidQuestion_ErrorMessage;

                case MembershipCreateStatus.InvalidUserName:
                    return ValidationResource.MembershipCreateStatus_InvalidUserName_ErrorMessage;

                case MembershipCreateStatus.ProviderError:
                    return ValidationResource.MembershipCreateStatus_ProviderError_ErrorMessage;

                case MembershipCreateStatus.UserRejected:
                    return ValidationResource.MembershipCreateStatus_UserRejected_ErrorMessage;

                default:
                    return ValidationResource.MembershipCreateStatus_Others_ErrorMessage;
            }
        }

        private static UserProfile GetDetailByUserName(DefaultContext db, string userName)
        {
            UserProfile userProfile = db.UserProfiles.SingleOrDefault(u => u.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase));
            SetRole(ref userProfile);

            return userProfile;
        }

        private static UserProfile GetDetailByLyonessId(DefaultContext db, string lyonessId)
        {
            UserProfile userProfile = db.UserProfiles.FirstOrDefault(up => up.LyonessId.Equals(lyonessId, StringComparison.InvariantCultureIgnoreCase));
            SetRole(ref userProfile);

            return userProfile;
        }

        private static void SetRole(ref UserProfile userProfile)
        {
            if (userProfile != null)
            {
                userProfile.Role = UserProfile.GetRoleForUser(userProfile.UserName);
            }
        }

        public static void SetAcceptedPrivacy(DefaultContext db, int userId)
        {
            UserProfile userProfile = GetDetail(db, userId);
            if (userProfile == null)
            {
                Logger.SetLog("Cannot get UserProfile for setting accepted privacy.");
                return;
            }

            userProfile.IsPoliciesAccepted = true;

            db.SaveChanges();
        }

        public static async Task<Calendar> GetCalendar(DefaultContext db, int registrarId, Controller controller, CancellationToken cancellationToken)
        {
            UserProfile userProfile = GetDetail(db, registrarId);
            var calendar = new Calendar
            {
                GoogleCredentialsJson = userProfile.GoogleCredentialsJson,
                GoogleCalendarId = userProfile.GoogleCalendarId,
                UseGoogleCalendarByUser = userProfile.UseGoogleCalendar,
                UseMail = userProfile.UseMail,
                EmailTo = userProfile.Email1,
                ReminderTime = userProfile.ReminderTime,
                IsEventsPrivate = userProfile.IsEventsPrivate
            };
            await calendar.AuthorizeAsync(controller, cancellationToken);

            return calendar;
        }

        public static IEnumerable<PopulatedUser> GetUplineUsersWithoutSharedContacts(DefaultContext db, int userId)
        {
            IEnumerable<PopulatedUser> populatedUplineUsers = db.Database.SqlQuery<PopulatedUser>(String.Format(GetUplineUsersWithoutSharedContactsProcedureTemplate, userId));
            return populatedUplineUsers;
        }

        public static IEnumerable<PopulatedUser> GetDownlineUsersWithoutSharedContacts(DefaultContext db, int userId)
        {
            IEnumerable<PopulatedUser> populatedDownlineUsers = db.Database.SqlQuery<PopulatedUser>(String.Format(GetDownlineUsersWithoutSharedContactsProcedureTemplate, userId));
            return populatedDownlineUsers;
        }

        public static IEnumerable<PopulatedUser> GetAnyUsersWithoutSharedContacts(DefaultContext db, int userId)
        {
            IEnumerable<PopulatedUser> populatedAnyUsers = db.Database.SqlQuery<PopulatedUser>(String.Format(GetAnyUsersWithoutSharedContactsProcedureTemplate, userId));
            return populatedAnyUsers;
        }

        public static IEnumerable<PopulatedUser> GetDownlineUsersWithoutTopTenContacts(DefaultContext db, int userId)
        {
            IEnumerable<PopulatedUser> populatedDownlineUsers = db.Database.SqlQuery<PopulatedUser>(String.Format(GetDownlineUsersWithoutTopTenContactsProcedureTemplate, userId));
            return populatedDownlineUsers;
        }

        public static void ProcessAccessPayments(DefaultContext db)
        {
            DateTime currentDbDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1);

            // Zkontrolovat zaplacené CLA Trial účty
            CheckClaAccessTrialUsers(db, currentDbDate);

            // Zkontrolovat zaplacené účty
            CheckClaAccessUsers(db, currentDbDate);

            db.SaveChanges();

            // Upozornění na 1 měsíc před expirací.
            CheckNotificationClaUsers(db, currentDbDate, ClaAccessNotification.Monthly);

            // Upozornění na 14 dní, 3 dny před expirací a v den expirace.
            var claAccessNotifications = new[] { ClaAccessNotification.Fortnightly, ClaAccessNotification.ThreeDays, ClaAccessNotification.Current };
            foreach (ClaAccessNotification claAccessNotification in claAccessNotifications)
            {
                CheckNotificationClaUsers(db, currentDbDate, claAccessNotification);
            }

            // Upozornit čerstvě zablokované účty
            CheckNotificationNewBlockedClaUsers(db, currentDbDate);
        }

        private static void CheckClaAccessTrialUsers(DefaultContext db, DateTime currentDbDate)
        {
            // Okamžitá změna účtu Trial na standardní účet lídra.
            UserProfileWithRole[] userProfileWithRoles = db.Database.SqlQuery<UserProfileWithRole>(GetClaAccessTrialUsersProcedureTemplate).ToArray();
            foreach (UserProfileWithRole userProfileWithRole in userProfileWithRoles)
            {
                UserProfile userProfile = GetDetail(db, userProfileWithRole.UserId);
                if (userProfile == null)
                    continue;

                userProfile.ClaAccessAmount -= userProfileWithRole.ClaAccessYearlyAccess;
                userProfile.ClaAccessExpired = currentDbDate.AddYears(1);
                userProfile.ClaAccessNotification = ClaAccessNotification.None;
                userProfile.ClaAccessTrial = false;

                switch (userProfileWithRole.UserProfileRole)
                {
                    case RoleType.AdminLeader:
                    case RoleType.Leader:
                    case RoleType.AuthGuest:
                        break;

                    case RoleType.Admin:
                        ChangeUserRole(userProfileWithRole.UserName, RoleType.AdminLeader);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                BankAccount bankAccountForClaAccess = BankAccountCache.GetDetail(db, BankAccountType.ApplicationAccess, userProfile.ClaAccessCurrency);

                string invoiceNumber;
                string invoice = BankAccountCommon.GetInvoice(db, userProfile, bankAccountForClaAccess, currentDbDate, out invoiceNumber);
                string attachmentFilePath = FileService.SaveInvoiceFile(invoice, invoiceNumber);

                // Pokud právě změněný účet byl po expiraci, poslat oznámení, že je odblokovaný.
                if (userProfileWithRole.ClaAccessExpired < currentDbDate && userProfileWithRole.UseMail)
                {
                    Mail.SendEmail(userProfileWithRole.Email1,
                                   MailResource.TaskSchedulerController_NotificationAccessUnblock_Subject,
                                   MailResource.TaskSchedulerController_NotificationAccessUnblock_TextBody, true, true, attachmentFilePath);
                }
                else
                {
                    Mail.SendEmail(userProfileWithRole.Email1,
                               MailResource.TaskSchedulerController_Invoice_Subject,
                               MailResource.TaskSchedulerController_Invoice_Message, true, true, attachmentFilePath);
                }

                Mail.SendEmail(Properties.Settings.Default.AccountantEmail,
                               MailResource.TaskSchedulerController_Invoice_Subject,
                               MailResource.TaskSchedulerController_Invoice_Message, true, true, attachmentFilePath);
            }
        }

        private static void CheckClaAccessUsers(DefaultContext db, DateTime currentDbDate)
        {
            // Prodloužení účtu měsíc před expirací (poté kontrola každý den)
            DateTime checkDbDate = currentDbDate.AddMonths(1);
            UserProfileWithRole[] userProfileWithRoles = db.Database.SqlQuery<UserProfileWithRole>(String.Format(GetClaAccessUsersProcedureTemplate, checkDbDate)).ToArray();
            foreach (UserProfileWithRole userProfileWithRole in userProfileWithRoles)
            {
                UserProfile userProfile = GetDetail(db, userProfileWithRole.UserId);
                if (userProfile == null)
                    continue;

                BankAccount bankAccountForClaAccess = BankAccountCache.GetDetail(db, BankAccountType.ApplicationAccess, userProfile.ClaAccessCurrency);

                string invoiceNumber;
                string invoice = BankAccountCommon.GetInvoice(db, userProfile, bankAccountForClaAccess, currentDbDate, out invoiceNumber);
                string attachmentFilePath = FileService.SaveInvoiceFile(invoice, invoiceNumber);

                // Pokud právě změněný účet byl po expiraci, poslat oznámení, že je odblokovaný.
                if (userProfileWithRole.ClaAccessExpired < currentDbDate && userProfileWithRole.UseMail)
                {
                    Mail.SendEmail(userProfileWithRole.Email1,
                                   MailResource.TaskSchedulerController_NotificationAccessUnblock_Subject,
                                   MailResource.TaskSchedulerController_NotificationAccessUnblock_TextBody, true, true, attachmentFilePath);
                }
                else
                {
                    Mail.SendEmail(userProfileWithRole.Email1,
                               MailResource.TaskSchedulerController_Invoice_Subject,
                               MailResource.TaskSchedulerController_Invoice_Message, true, true, attachmentFilePath);
                }

                Mail.SendEmail(Properties.Settings.Default.AccountantEmail,
                               MailResource.TaskSchedulerController_Invoice_Subject,
                               MailResource.TaskSchedulerController_Invoice_Message, true, true, attachmentFilePath);

                DateTime claAccessExpired = userProfileWithRole.ClaAccessExpired >= currentDbDate
                                                ? userProfileWithRole.ClaAccessExpired.GetValueOrDefault().AddYears(1)
                                                : currentDbDate.AddYears(1);

                userProfile.ClaAccessAmount -= userProfileWithRole.ClaAccessYearlyAccess;
                userProfile.ClaAccessExpired = claAccessExpired;
                userProfile.ClaAccessNotification = ClaAccessNotification.None;
            }
        }

        private static void CheckNotificationClaUsers(DefaultContext db, DateTime currentDbDate, ClaAccessNotification claAccessNotification)
        {
            DateTime checkDbDate = UserProfile.GetCheckDbDateForClaAccessNotification(claAccessNotification, currentDbDate);
            UserProfileWithRole[] userProfileWithRoles = db.Database.SqlQuery<UserProfileWithRole>(String.Format(GetNotificationClaUsersProcedureTemplate, checkDbDate)).ToArray();
            if (userProfileWithRoles.Length == 0)
                return;

            var modifiedUserIds = new List<int>();
            foreach (UserProfileWithRole userProfileWithRole in userProfileWithRoles)
            {
                if (userProfileWithRole.IsAdmin)
                    continue;

                if (userProfileWithRole.ClaAccessNotification == claAccessNotification)
                    continue;

                decimal amountRemains = userProfileWithRole.ClaAccessYearlyAccess - userProfileWithRole.ClaAccessAmount;
                string paymentInformation = BankAccountCache.GetPaymentInfo(db, userProfileWithRole.ClaAccessCurrency, userProfileWithRole.LyonessId, amountRemains);
                string textBody = String.Format(MailResource.TaskSchedulerController_NotificationAccessExpiration_TextBody, checkDbDate, paymentInformation);
                Mail.SendEmail(userProfileWithRole.Email1, MailResource.TaskSchedulerController_NotificationAccessExpiration_Subject, textBody, true, true);
                modifiedUserIds.Add(userProfileWithRole.UserId);
            }

            if (modifiedUserIds.Count == 0)
                return;

            var sqlCommand = new StringBuilder();
            foreach (int modifiedUserId in modifiedUserIds)
            {
                sqlCommand.AppendLine(String.Format(SetNotificationClaUsersProcedureTemplate, modifiedUserId, (int)claAccessNotification));
            }
            db.Database.ExecuteSqlCommand(sqlCommand.ToString());
        }

        private static void CheckNotificationNewBlockedClaUsers(DefaultContext db, DateTime currentDbDate)
        {
            // Zkontrolovat všechny uživatele, kterým včera vypršel přístup do aplikace
            DateTime checkDbDate = currentDbDate.AddDays(-1);
            UserProfileWithRole[] userProfileWithRoles = db.Database.SqlQuery<UserProfileWithRole>(String.Format(GetNotificationNewBlockedClaUsersProcedureTemplate, checkDbDate)).ToArray();
            if (userProfileWithRoles.Length == 0)
                return;

            var modifiedUserIds = new List<int>();
            foreach (UserProfileWithRole userProfileWithRole in userProfileWithRoles)
            {
                if (userProfileWithRole.IsAdmin)
                    continue;

                if (userProfileWithRole.ClaAccessNotification == ClaAccessNotification.Blocked)
                    return;

                decimal amountRemains = userProfileWithRole.ClaAccessYearlyAccess - userProfileWithRole.ClaAccessAmount;
                string paymentInformation = BankAccountCache.GetPaymentInfo(db, userProfileWithRole.ClaAccessCurrency, userProfileWithRole.LyonessId, amountRemains);
                string textBody = String.Format(MailResource.TaskSchedulerController_NotificationAccessBlock_TextBody, paymentInformation);
                Mail.SendEmail(userProfileWithRole.Email1, MailResource.TaskSchedulerController_NotificationAccessBlock_Subject, textBody, true, true);
                modifiedUserIds.Add(userProfileWithRole.UserId);
            }

            if (modifiedUserIds.Count == 0)
                return;

            var sqlCommand = new StringBuilder();
            foreach (int modifiedUserId in modifiedUserIds)
            {
                sqlCommand.AppendLine(String.Format(SetNotificationClaUsersProcedureTemplate, modifiedUserId, (int)ClaAccessNotification.Blocked));
            }
            db.Database.ExecuteSqlCommand(sqlCommand.ToString());
        }

        public static bool Promote(DefaultContext db, UserProfile userProfile)
        {
            try
            {
                ChangeUserRole(userProfile.UserName, RoleType.Leader);

                userProfile.ClaAccessTrial = true;
                userProfile.ClaAccessExpired = DateTime.Now.Date.AddMonths(1).AddDays(1).AddSeconds(-1);

                db.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                Logger.SetLog(e);

                return false;
            }
        }
    }
}