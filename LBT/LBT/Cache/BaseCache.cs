using System;
using System.Data.Entity.Validation;
using System.Text;
using LBT.DAL;
using LBT.Helpers;

namespace LBT.Cache
{
    public enum DeleteResult
    {
        Ok,
        AuthorizationFailed,
        DbFailed,
        UnlinkFailed,
        AdminAccountProtection
    }

    public abstract class BaseCache
    {
        public const string ActualCareerStageField = "ActualCareerStage";
        public const string AddressLine1Field = "AddressLine1";
        public const string BankAccountIdField = "BankAccountId";
        public const string BankAccountTypeField = "BankAccountType";
        public const string BodyField = "Body";
        public const string BusinessInfoParticipatedField = "BusinessInfoParticipated";
        public const string CapacityField = "Capacity";
        public const string CityField = "City";
        public const string ClaAccessChangedCurrencyField = "ClaAccessChangedCurrency";
        public const string ConfirmPersonalDataField = "ConfirmPersonalData";
        public const string ConfirmTermsAndConditionsField = "ConfirmTermsAndConditions";
        public const string CurrencyTypeField = "CurrencyType";
        public const string DateField = "Date";
        public const string DateFinishTimeField = "DateFinishTime";
        public const string DistrictIdField = "DistrictId";
        public const string Email1Field = "Email1";
        public const string EmailBodyField = "EmailBody";
        public const string EmailSenderBodyField = "EmailSenderBody";
        public const string EmptyField = "";
        public const string ExchangeRateToCZKField = "ExchangeRateToCZK";
        public const string FieldValueField = "FieldValue";
        public const string FileField = "File";
        public const string FinishTimeField = "FinishTime";
        public const string FirstNameField = "FirstName";
        public const string FullNameField = "FullName";
        public const string GoogleCredentialsJsonField = "GoogleCredentialsJson";
        public const string LastContactField = "LastContact";
        public const string LastNameField = "LastName";
        public const string LyonessIdField = "LyonessId";
        public const string ManualTypeIdField = "ManualTypeId";
        public const string MeetingTitleTypeField = "MeetingTitleType";
        public const string MeetingTypeField = "MeetingType";
        public const string MultiplePeopleContactsJsonField = "MultiplePeopleContactsJson";
        public const string NewPasswordField = "NewPassword";
        public const string OrganizerField = "Organizer";
        public const string PhoneNumber1Field = "PhoneNumber1";
        public const string PhoneNumberPrefix1IdField = "PhoneNumberPrefix1Id";
        public const string PhoneNumberPrefixIdField = "PhoneNumberPrefixId";
        public const string PotentialField = "Potential";
        public const string PresentedField = "Presented";
        public const string PriceField = "Price";
        public const string RegisterDeadlineField = "RegisterDeadline";
        public const string RoleField = "Role";
        public const string SecondaryLeaderIdField = "SecondaryLeaderId";
        public const string SecondaryOrganizerIdField = "SecondaryOrganizerId";
        public const string SecondBankAccountIdField = "SecondBankAccountId";
        public const string SecondContactedField = "SecondContacted";
        public const string SecondMeetingField = "SecondMeeting";
        public const string SecondPriceField = "SecondPrice";
        public const string SecondTrackingEmailSentField = "SecondTrackingEmailSent";
        public const string SkypeField = "Skype";
        public const string StartTimeField = "StartTime";
        public const string StatusField = "Status";
        public const string SubjectField = "Subject";
        public const string ThirdMeetingField = "ThirdMeeting";
        public const string TitleField = "Title";
        public const string TokenField = "Token";
        public const string TrackingEmailSentField = "TrackingEmailSent";
        public const string UserNameField = "UserName";
        public const string ValidToField = "ValidTo";
        public const string VideoIdField = "VideoId";
        public const string WebinarUrlField = "WebinarUrl";

        public const string SearchStringAccordingField = "SearchStringAccording";

        protected const string ActualCareerStageAscSortOrder = "ActualCareerStage_asc";
        protected const string ActualCareerStageDescSortOrder = "ActualCareerStage_desc";
        protected const string AddressLine1AscSortOrder = "AddressLine1_asc";
        protected const string AddressLine1DescSortOrder = "AddressLine1_desc";
        protected const string BankAccountTypeAscSortOrder = "BankAccountType_asc";
        protected const string BankAccountTypeDescSortOrder = "BankAccountType_desc";
        protected const string CityAscSortOrder = "City_asc";
        protected const string CityDescSortOrder = "City_desc";
        protected const string CurrencyTypeAscSortOrder = "CurrencyType_asc";
        protected const string CurrencyTypeDescSortOrder = "CurrencyType_desc";
        protected const string DateDescSortOrder = "Date_desc";
        protected const string Email1AscSortOrder = "Email1_asc";
        protected const string Email1DescSortOrder = "Email1_desc";
        protected const string FirstNameAscSortOrder = "FirstName_asc";
        protected const string FirstNameDescSortOrder = "FirstName_desc";
        protected const string LastContactAscSortOrder = "LastContact_asc";
        protected const string LastContactDescSortOrder = "LastContact_desc";
        protected const string LastNameDescSortOrder = "LastName_desc";
        protected const string LyonessIdAscSortOrder = "LyonessId_asc";
        protected const string LyonessIdDescSortOrder = "LyonessId_desc";
        protected const string MeetingTitleTypeAscSortOrder = "MeetingTitleType_asc";
        protected const string MeetingTitleTypeDescSortOrder = "MeetingTitleType_desc";
        protected const string MeetingTypeAscSortOrder = "MeetingType_asc";
        protected const string MeetingTypeDescSortOrder = "MeetingType_desc";
        protected const string OrganizerAscSortOrder = "Organizer_asc";
        protected const string OrganizerDescSortOrder = "Organizer_desc";
        protected const string PhoneNumber1AscSortOrder = "PhoneNumber1_asc";
        protected const string PhoneNumber1DescSortOrder = "PhoneNumber1_desc";
        protected const string PotentialAscSortOrder = "Potential_asc";
        protected const string PotentialDescSortOrder = "Potential_desc";
        protected const string PremiumMembershipGrantedAscSortOrder = "PremiumMembershipGranted_asc";
        protected const string PremiumMembershipGrantedDescSortOrder = "PremiumMembershipGranted_desc";
        protected const string RoleAscSortOrder = "Role_asc";
        protected const string RoleDescSortOrder = "Role_desc";
        protected const string SkypeAscSortOrder = "Skype_asc";
        protected const string SkypeDescSortOrder = "Skype_desc";
        protected const string StatusAscSortOrder = "Status_asc";
        protected const string StatusDescSortOrder = "Status_desc";
        protected const string TitleDescSortOrder = "Title_desc";
        protected const string ValidToAscSortOrder = "ValidTo_asc";
        protected const string ValidToDescSortOrder = "ValidTo_desc";

        protected const string CascadeRemoveBankAccountProcedureTemplate = "EXEC dbo.CascadeRemoveBankAccount @BankAccountId";
        protected const string CascadeRemoveArchivedMeetingTemplate = "EXEC dbo.CascadeRemoveArchivedMeeting";
        protected const string CascadeRemoveMeetingProcedureTemplate = "EXEC dbo.CascadeRemoveMeeting @MeetingId";
        protected const string CascadeRemovePeopleContactProcedureTemplate = "EXEC dbo.CascadeRemovePeopleContact @PeopleContactId";
        protected const string CascadeRemoveTeamProcedureTemplate = "EXEC dbo.CascadeRemoveTeam @TeamId";
        protected const string CascadeRemoveUserProcedureTemplate = "EXEC dbo.CascadeRemoveUser @UserId";
        protected const string CascadeRemoveUserProfileProcedureTemplate = "EXEC dbo.CascadeRemoveUserProfile @UserId";
        protected const string CascadeRemoveVideoProcedureTemplate = "EXEC dbo.CascadeRemoveVideo @VideoId";
        protected const string ExpireVideoTokensTemplate = "EXEC dbo.ExpireVideoTokens";
        protected const string GetAdminsProcedureTemplate = "EXEC dbo.GetAdmins";
        protected const string GetAnyUsersProcedureTemplate = "SELECT * FROM GetAnyUsers()";
        protected const string GetAnyUsersWithoutSharedContactsProcedureTemplate = "EXEC dbo.GetAnyUsersWithoutSharedContacts {0}";
        protected const string GetClaAccessTrialUsersProcedureTemplate = "EXEC dbo.GetClaAccessTrialUsers";
        protected const string GetClaAccessUsersProcedureTemplate = "SET DATEFORMAT ymd; EXEC dbo.GetClaAccessUsers '{0:yyyy-MM-dd HH:mm:ss}'";
        protected const string GetDonwlinesProcedureTemplate = "SELECT * FROM GetDownlines(@UserId)";
        protected const string GetDownlinesAndUplinesProcedureTemplate = "SELECT * FROM GetDownlinesAndUplines(@UserId)";
        protected const string GetDownlineUsersAllProcedureTemplate = "EXEC dbo.GetDownlineUsersAll {0}";
        protected const string GetDownlineUsersProcedureTemplate = "SELECT * FROM GetDownlineUsers(@UserId)";
        protected const string GetDownlineUsersWithoutSharedContactsProcedureTemplate = "EXEC dbo.GetDownlineUsersWithoutSharedContacts {0}";
        protected const string GetDownlineUsersWithoutTopTenContactsProcedureTemplate = "EXEC dbo.GetDownlineUsersWithoutTopTenContacts {0}";
        protected const string GetLeadedUserIdsProcedureTemplate = "EXEC dbo.GetLeadedUserIds @UserId";
        protected const string GetMeetingArchiveIndexProcedureTemplate = "EXEC dbo.GetMeetingArchiveIndex {0}";
        protected const string GetMeetingFilteredArchiveIndexProcedureTemplate = "EXEC dbo.GetMeetingFilteredArchiveIndex {0}, {1}";
        protected const string GetMeetingFilteredIndexProcedureTemplate = "EXEC dbo.GetMeetingFilteredIndex {0}, {1}";
        protected const string GetMeetingIndexForUserProcedureTemplate = "EXEC dbo.GetMeetingIndexForUser {0}, {1}";
        protected const string GetMeetingIndexProcedureTemplate = "EXEC dbo.GetMeetingIndex {0}";
        protected const string GetNotificationClaUsersProcedureTemplate = "SET DATEFORMAT ymd; EXEC dbo.GetNotificationClaUsers '{0:yyyy-MM-dd HH:mm:ss}'";
        protected const string GetNotificationNewBlockedClaUsersProcedureTemplate = "SET DATEFORMAT ymd; EXEC dbo.GetNotificationNewBlockedClaUsers '{0:yyyy-MM-dd HH:mm:ss}'";
        protected const string GetPopulateDownlineUsersProcedureTemplate = "EXEC dbo.GetPopulateDownlineUsers {0}";
        protected const string GetUplineUsersProcedureTemplate = "EXEC dbo.GetUplineUsers {0}";
        protected const string GetUplineUsersWithoutSharedContactsProcedureTemplate = "EXEC dbo.GetUplineUsersWithoutSharedContacts {0}";
        protected const string GetUserProfileIndexForAdminProcedureTemplate = "EXEC dbo.GetUserProfileIndexForAdmin @UserId";
        protected const string GetUserProfileIndexProcedureTemplate = "EXEC dbo.GetUserProfileIndex @UserId";
        protected const string GetUserProfileIndexTreeForAdminProcedureTemplate = "EXEC dbo.GetUserProfileIndexTreeForAdmin @UserId";
        protected const string GetUserProfileIndexTreeProcedureTemplate = "EXEC dbo.GetUserProfileIndexTree @UserId";
        protected const string RefreshStatisticsProcedureTemplate = "EXEC dbo.RefreshStatistics";
        protected const string RefreshTopTenRolesProcedureTemplate = "EXEC dbo.RefreshTopTenRoles";
        protected const string SetNotificationClaUsersProcedureTemplate = "EXEC dbo.SetNotificationClaUsers @UserId = {0}, @ClaAccessNotification = {1};";

        protected const string BankAccountIdSqlParameter = "@BankAccountId";
        protected const string MeetingIdSqlParameter = "@MeetingId";
        protected const string MeetingTypeSqlParameter = "MeetingType";
        protected const string PeopleContactIdSqlParameter = "@PeopleContactId";
        protected const string UserIdSqlParameter = "@UserId";
        protected const string TeamIdSqlParameter = "@TeamId";
        protected const string VideoIdSqlparameter = "@VideoId";

        protected static bool TrySaveChanges(DefaultContext db)
        {
            try
            {
                db.SaveChanges();
                return true;
            }
            catch (DbEntityValidationException e)
            {
                var sb = new StringBuilder();
                sb.AppendLine(e.Message);

                foreach (DbEntityValidationResult dbEntityValidationResult in e.EntityValidationErrors)
                {
                    foreach (DbValidationError dbValidationError in dbEntityValidationResult.ValidationErrors)
                    {
                        sb.AppendLine(String.Format("{0}: {1}.", dbValidationError.PropertyName,dbValidationError.ErrorMessage));
                    }
                }

                sb.AppendLine(e.StackTrace);
                Logger.SetLog(sb.ToString());
                return false;
            }
        }
    }
}