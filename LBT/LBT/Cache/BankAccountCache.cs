using LBT.DAL;
using LBT.Helpers;
using LBT.Models;
using LBT.ModelViews;
using LBT.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;

namespace LBT.Cache
{
    public class BankAccountCache : BaseCache
    {
        public static BankAccount[] GetIndex(DefaultContext db, int exceptBankAccountId)
        {
            IQueryable<BankAccount> bankAccounts = db.BankAccounts.Where(ba => ba.BankAccountId != exceptBankAccountId);
            return bankAccounts.ToArray();
        }

        public static BankAccount[] GetIndex(DefaultContext db, BankAccountType bankAccountType, int userId)
        {
            IQueryable<BankAccount> bankAccounts = db.BankAccounts.Where(ba => ba.BankAccountType == bankAccountType && ba.BankAccountUsers.Any(bau => bau.UserId == userId));
            return bankAccounts.ToArray();
        }

        public static BankAccount[] GetUniqueAccessAccounts(DefaultContext db)
        {
            BankAccount[] bankAccounts = db.BankAccounts.Where(ba => ba.BankAccountType == BankAccountType.ApplicationAccess).ToArray();

            IEnumerable<CurrencyType> currencyTypes = bankAccounts.Select(ba => ba.CurrencyType).Distinct();

            return currencyTypes.Select(ct => bankAccounts.First(ba => ba.CurrencyType == ct)).ToArray();
        }

        public static BankAccount[] GetIndex(DefaultContext db, string sortOrder)
        {
            IQueryable<BankAccount> bankAccounts = db.BankAccounts.AsQueryable();

            switch (sortOrder)
            {
                default:
                    bankAccounts = bankAccounts.OrderBy(ba => ba.Title);
                    break;

                case TitleDescSortOrder:
                    bankAccounts = bankAccounts.OrderByDescending(ba => ba.Title);
                    break;

                case BankAccountTypeAscSortOrder:
                    bankAccounts = bankAccounts.OrderBy(ba => ba.BankAccountType);
                    break;

                case BankAccountTypeDescSortOrder:
                    bankAccounts = bankAccounts.OrderByDescending(ba => ba.BankAccountType);
                    break;

                case CurrencyTypeAscSortOrder:
                    bankAccounts = bankAccounts.OrderBy(ba => ba.CurrencyType);
                    break;

                case CurrencyTypeDescSortOrder:
                    bankAccounts = bankAccounts.OrderByDescending(ba => ba.CurrencyType);
                    break;

                case ValidToAscSortOrder:
                    bankAccounts = bankAccounts.OrderBy(ba => ba.ValidTo);
                    break;

                case ValidToDescSortOrder:
                    bankAccounts = bankAccounts.OrderByDescending(ba => ba.ValidTo);
                    break;
            }

            return bankAccounts.ToArray();
        }

        public static BankAccount[] GetValidAccounts(DefaultContext db)
        {
            IQueryable<BankAccount> bankAccounts = db.BankAccounts.Where(ba => ba.ValidTo >= DateTime.Now);
            return bankAccounts.ToArray();
        }

        public static void Insert(DefaultContext db, BankAccount bankAccount, int[] userIds)
        {
            db.BankAccounts.Add(bankAccount);

            IEnumerable<BankAccountUser> bankAccountUsers = userIds == null
                                                                ? new BankAccountUser[0]
                                                                : userIds.Select(ui => new BankAccountUser
                                                                                           {
                                                                                               BankAccountId = bankAccount.BankAccountId,
                                                                                               UserId = ui
                                                                                           });
            foreach (BankAccountUser bankAccountUser in bankAccountUsers)
            {
                db.BankAccountUsers.Add(bankAccountUser);
            }

            db.SaveChanges();
        }

        public static BankAccount GetDetail(DefaultContext db, int id)
        {
            BankAccount bankAccount = db.BankAccounts.Find(id);
            return bankAccount;
        }

        public static BankAccount GetDetail(DefaultContext db, BankAccountType bankAccountType, CurrencyType currencyType)
        {
            BankAccount bankAccount = db.BankAccounts.FirstOrDefault(ba => ba.BankAccountType == bankAccountType && ba.CurrencyType == currencyType);
            return bankAccount;
        }

        public static bool Update(DefaultContext db, int[] userIds, bool saveOnlyUsersOrToken, ref BankAccount bankAccount)
        {
            int bankAccountId = bankAccount.BankAccountId;
            BankAccount dbBankAccount = GetDetail(db, bankAccountId);
            if (dbBankAccount == null)
                return false;

            if (!saveOnlyUsersOrToken)
            {
                if (bankAccount.NeedResetLastDownloadId(dbBankAccount))
                {
                    bankAccount.LastDownloadId = null;
                }

                dbBankAccount.CopyFrom(bankAccount);
            }
            else
            {
                dbBankAccount.Token = bankAccount.Token;
                dbBankAccount.ValidTo = bankAccount.ValidTo;
            }

            int[] dbUserIds = dbBankAccount.BankAccountUsers.Select(bau => bau.UserId).ToArray();
            int[] deletedUserIds = dbUserIds.Except(userIds).ToArray();
            int[] newUserIds = userIds.Except(dbUserIds).ToArray();

            BankAccountUser[] deletedBankAccountUsers =
                deletedUserIds.Select(dui => db.BankAccountUsers.Where(bau => bau.BankAccountId == bankAccountId && bau.UserId == dui))
                .SelectMany(bau => bau).ToArray();
            foreach (BankAccountUser bankAccountUser in deletedBankAccountUsers)
            {
                db.BankAccountUsers.Remove(bankAccountUser);
            }

            List<BankAccountUser> newBankAccountUsers = newUserIds.Select(nui => new BankAccountUser
                                                                                     {
                                                                                         BankAccountId = bankAccountId,
                                                                                         UserId = nui
                                                                                     }).ToList();
            foreach (BankAccountUser bankAccountUser in newBankAccountUsers)
            {
                db.BankAccountUsers.Add(bankAccountUser);
            }

            bool success = TrySaveChanges(db);
            if (!success)
                return false;

            bankAccount = dbBankAccount;
            return true;
        }

        public static DeleteResult Delete(DefaultContext db, int id, out BankAccount bankAccount)
        {
            bankAccount = GetDetail(db, id);
            if (bankAccount == null)
                return DeleteResult.AuthorizationFailed;

            // Pokud je bankovní účet součástí stále platných akcí, nesmí se smazat!
            if (db.Meetings.Any(m => m.BankAccountId == id && m.Finished > DateTime.Now))
                return DeleteResult.UnlinkFailed;

            try
            {
                var parameter = new SqlParameter(BankAccountIdSqlParameter, bankAccount.BankAccountId);
                db.Database.ExecuteSqlCommand(CascadeRemoveBankAccountProcedureTemplate, parameter);
                return DeleteResult.Ok;
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
                return DeleteResult.DbFailed;
            }
        }

        public static string GetPaymentInfo(DefaultContext db, CurrencyType currencyType, string lyonessId, decimal amountRemains)
        {
            BankAccount bankAccount = GetDetail(db, BankAccountType.ApplicationAccess, currencyType);

            if (bankAccount == null)
            {
                string errorMessage = GetPaymentInfoRow(ValidationResource.BankAccount_CannotFindBankAccount_ErrorMessage);
                return errorMessage;
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(GetPaymentInfoRow(ViewResource.BankAccount_AccountPaymentInfo_Text, bankAccount.AccountId));
            stringBuilder.AppendLine(GetPaymentInfoRow(ViewResource.BankAccount_VsPaymentInfo_Text, BankAccountHistory.GetVs(lyonessId)));
            stringBuilder.AppendLine(GetPaymentInfoRow(ViewResource.BankAccount_AmmountPaymentInfo_Text, amountRemains, bankAccount.CurrencyType));

            return stringBuilder.ToString();
        }

        public static string GetPaymentInfoRow(string text, params object[] args)
        {
            string rowText = String.Format("  {0}", text);
            rowText = String.Format(rowText, args);
            return rowText;
        }

        public static void ImportBankTransactions(DefaultContext db, out string bankToken, out string failedUrl)
        {
            bankToken = "Unknown";
            failedUrl = "Unknown";
            string bankTransactionUrlTemplate = Properties.Settings.Default.BankTransactionUrl;
            string bankTransactionSetStartDateUrlTemplate = Properties.Settings.Default.BankTransactionSetStartDateUrl;
            string bankTransactionSetStartIdUrlTemplate = Properties.Settings.Default.BankTransactionSetStartIdUrl;

            BankAccount[] bankAccounts = GetValidAccounts(db);
            foreach (BankAccount bankAccount in bankAccounts)
            {
                bankToken = bankAccount.Token;

                InternetResponse internetResponse;
                if (!bankAccount.LastDownloadId.HasValue)
                {
                    string bankTransactionSetStartDateUrl = String.Format(bankTransactionSetStartDateUrlTemplate, bankAccount.Token, bankAccount.TransactionStartDate.ToString("yyyy-MM-dd"));
                    failedUrl = bankTransactionSetStartDateUrl;
                    internetResponse = InternetRequest.SendRequest(bankTransactionSetStartDateUrl, InternetRequestType.OnlyStatusCode);
                }
                else
                {
                    string bankTransactionSetStartIdUrl = String.Format(bankTransactionSetStartIdUrlTemplate, bankAccount.Token, bankAccount.LastDownloadId.GetValueOrDefault());
                    failedUrl = bankTransactionSetStartIdUrl;
                    internetResponse = InternetRequest.SendRequest(bankTransactionSetStartIdUrl, InternetRequestType.OnlyStatusCode);
                }

                if (internetResponse.HttpStatusCode != HttpStatusCode.OK)
                    continue;

                string bankTransactionUrl = String.Format(bankTransactionUrlTemplate, bankAccount.Token);
                failedUrl = bankTransactionUrl;
                BankAccountHistory[] bankAccountHistories;
                Int64? lastDownloadId;
                HttpStatusCode httpStatusCode = BankAccountHistoryCache.ProcessTransactionRequest(bankTransactionUrl, bankAccount.BankAccountId, out bankAccountHistories, out lastDownloadId);

                if (httpStatusCode != HttpStatusCode.OK || bankAccountHistories.Length == 0)
                    continue;

                foreach (BankAccountHistory bankAccountHistory in bankAccountHistories.Where(bah => bah.Ammount > 0))
                {
                    db.BankAccountHistories.Add(bankAccountHistory);

                    string logMessage = String.Format("Imported bank transaction: account: {0}, date: {1}, ammount: {2}, exchange: {3}, ks: {4}, vs: {5}, ss: {6}", bankAccount.AccountId,bankAccountHistory.Date, bankAccountHistory.Ammount,bankAccountHistory.Exchange, bankAccountHistory.Ks,bankAccountHistory.Vs, bankAccountHistory.Ss);
                    Logger.SetLog(logMessage);
                }

                bankAccount.LastDownloadId = lastDownloadId;

                db.Entry(bankAccount).State = EntityState.Modified;
            }

            db.SaveChanges();
        }

        public static void ProcessBankAccountHistory(DefaultContext db)
        {
            DateTime currentDateTime = DateTime.Now;
            BankAccount[] bankAccounts = db.BankAccounts.ToArray();
            Currency[] currencies = CurrencyCache.GetIndex(db);
            foreach (BankAccount bankAccount in bankAccounts)
            {
                var conversionExclamationMessages = new StringBuilder();
                string conversionExclamationSubject = String.Empty;
                string conversionExclamationTextBodyTemplate = String.Empty;
                switch (bankAccount.BankAccountType)
                {
                    case BankAccountType.TeamMeeting:
                    case BankAccountType.DavidKotasekTraining:
                    case BankAccountType.Others:
                        ProcessPayableMeeting(db, bankAccount, currencies, currentDateTime, ref conversionExclamationMessages);
                        conversionExclamationSubject = MailResource.TaskSchedulerController_FoundConvertedAmountForMeeting_Subject;
                        conversionExclamationTextBodyTemplate = MailResource.TaskSchedulerController_FoundConvertedAmountForMeeting_TextBody;
                        break;

                    case BankAccountType.ApplicationAccess:
                        ProcessChangingApplicationAccessAmount(db, bankAccount, currencies, ref conversionExclamationMessages);
                        conversionExclamationSubject = MailResource.TaskSchedulerController_FoundConvertedAmountForAccess_Subject;
                        conversionExclamationTextBodyTemplate = MailResource.TaskSchedulerController_FoundConvertedAmountForAccess_TextBody;
                        break;
                }

                if (conversionExclamationMessages.Length == 0)
                    continue;

                string textBody = String.Format(conversionExclamationTextBodyTemplate, conversionExclamationMessages);
                Mail.SendEmailToAdmins(db, conversionExclamationSubject, textBody);
            }

            db.SaveChanges();
        }

        private static void ProcessPayableMeeting(DefaultContext db, BankAccount bankAccount, Currency[] currencies, DateTime currentDateTime, ref StringBuilder conversionExclamationMessages)
        {
            // Seskupit bankovní historii podle ID akce (Specifický symbol)
            var groupedBySsBankAccountHistories = bankAccount.BankAccountHistories
                .Where(bah => bah.Ss.HasValue)
                .GroupBy(bah => bah.Ss, bah => bah,
                         (key, g) => new
                         {
                             MeetingId = key,
                             BankAccountHistories = g.ToArray()
                         }).ToArray();
            foreach (var groupedBySsBankAccountHistory in groupedBySsBankAccountHistories)
            {
                int meetingId;
                try
                {
                    meetingId = Convert.ToInt32(groupedBySsBankAccountHistory.MeetingId.GetValueOrDefault());
                }
                catch (Exception e)
                {
                    Logger.SetLog(e);
                    continue;
                }

                Meeting meeting = MeetingCache.GetDetail(db, meetingId);
                if (meeting == null || meeting.Started < currentDateTime || (meeting.BankAccountId != bankAccount.BankAccountId && meeting.SecondBankAccountId != bankAccount.BankAccountId))
                    continue;

                MeetingAttendee[] meetingAttendees = meeting.MeetingAttendees.ToArray();
                int freeCapacity = meeting.Capacity - meetingAttendees.Length;

                // Seskupit bankovní historii podle ID akce a Lyoness Id (Specifický symbol a Variabilní symbol)
                var groupedBySsAndVsBankAccountHistories = groupedBySsBankAccountHistory.BankAccountHistories
                    .Where(bah => bah.Vs.HasValue)
                    .GroupBy(bah => bah.LyonessId, bah => bah,
                    (key, g) => new
                    {
                        LyonessId = key,
                        BankAccountHistories = g.ToArray()
                    }).ToArray();
                var conversionExclamationMessagesForOrganizer = new StringBuilder();
                foreach (var groupedBySsAndVsBankAccountHistory in groupedBySsAndVsBankAccountHistories)
                {
                    int amount = 0;
                    var notes = new StringBuilder();
                    foreach (BankAccountHistory bankAccountHistory in groupedBySsAndVsBankAccountHistory.BankAccountHistories)
                    {
                        notes.AppendLine(bankAccountHistory.Note);

                        if (bankAccountHistory.CurrencyType == bankAccount.CurrencyType)
                        {
                            amount += (int)bankAccountHistory.Ammount;
                            continue;
                        }

                        decimal convertedAmount;
                        CurrencyHelper.TryConvertTo(bankAccountHistory.Ammount, bankAccountHistory.CurrencyType, bankAccount.CurrencyType, currencies, out convertedAmount);
                        amount += (int)Math.Round(convertedAmount);

                        string conversionExclamationMessage =
                            String.Format(ViewResource.BankAccountHistory_MeetingPaymentWasConverted_Text,
                                          bankAccount.Title,
                                          bankAccount.CurrencyType, bankAccountHistory.CurrencyType,
                                          bankAccountHistory.Vs.GetValueOrDefault(), bankAccountHistory.Note);
                        conversionExclamationMessages.AppendLine(conversionExclamationMessage);
                        conversionExclamationMessagesForOrganizer.AppendLine(conversionExclamationMessage);
                    }

                    bool isSecondBankAccount = meeting.BankAccountId != bankAccount.BankAccountId;
                    bool success = RegisterAttendee(db, meetingAttendees, groupedBySsAndVsBankAccountHistory.LyonessId, notes.ToString(), meeting, amount, currentDateTime, isSecondBankAccount, ref freeCapacity);
                    if (!success)
                        continue;

                    foreach (BankAccountHistory bankAccountHistory in groupedBySsAndVsBankAccountHistory.BankAccountHistories)
                    {
                        db.BankAccountHistories.Remove(bankAccountHistory);
                    }
                }

                if (conversionExclamationMessages.Length == 0)
                    continue;

                string textBody = String.Format(MailResource.TaskSchedulerController_FoundConvertedAmountForMeeting_TextBody, conversionExclamationMessagesForOrganizer);
                Mail.SendEmail(meeting.Organizer.Email1, MailResource.TaskSchedulerController_FoundConvertedAmountForMeeting_Subject, textBody, meeting.Organizer.UseMail, true);
            }
        }

        private static bool RegisterAttendee(DefaultContext db, MeetingAttendee[] meetingAttendees, string bankAccountHistoryLyonessId, string bankAccountHistoryNote, Meeting meeting, int paidAmount, DateTime currentDateTime, bool isSecondBankAccount, ref int freeCapacity)
        {
            int meetingPrice = isSecondBankAccount ? meeting.SecondPrice.GetValueOrDefault() : meeting.Price.GetValueOrDefault();
            string maxReservationDate = MeetingAttendeeCache.GetMaxReservationDate(currentDateTime);

            // Registrace nebo akumulace nižší platby přihlášeného uživatele
            IEnumerable<MeetingAttendee> userMeetingAttendees = meetingAttendees.Where(ma => ma.UserAttendeeId.HasValue);
            MeetingAttendee meetingAttendee = userMeetingAttendees.FirstOrDefault(ua => ua.UserAttendee.LyonessId == bankAccountHistoryLyonessId);
            if (meetingAttendee != null)
            {
                if (meetingAttendee.Registered.HasValue)
                    return false;

                int totalPaidAmount = (isSecondBankAccount ? meetingAttendee.SecondPaidAmount : meetingAttendee.PaidAmount) + paidAmount;
                string totalBankAccountHistoryNote = String.IsNullOrEmpty(meetingAttendee.BankAccountHistoryNote)
                                                         ? bankAccountHistoryNote
                                                         : String.Format("{0}\n\n{1}", bankAccountHistoryNote, meetingAttendee.BankAccountHistoryNote);

                if (isSecondBankAccount)
                {
                    meetingAttendee.SecondPaidAmount = totalPaidAmount;
                }
                else
                {
                    meetingAttendee.PaidAmount = totalPaidAmount;
                }

                meetingAttendee.Reserved = currentDateTime;
                meetingAttendee.BankAccountHistoryNote = totalBankAccountHistoryNote;

                string meetingDetail = MeetingCommon.GetMeetingDetail(meeting);
                string paymentInfo = MeetingCommon.GetPaymentInfo(db, meeting, bankAccountHistoryLyonessId);
                string textBody;
                if (totalPaidAmount < meetingPrice)
                {
                    textBody = String.Format(MailResource.MeetingController_PaymentIsLow_TextBody, meeting.Title, meetingDetail, paymentInfo, totalPaidAmount);
                    Mail.SendEmail(meetingAttendee.UserAttendee.Email1, MailResource.MeetingController_PaymentIsLow_Subject, textBody, true, true);

                    textBody = String.Format(MailResource.MeetingController_OrganizerPaymentIsLow_TextBody, meetingAttendee.UserAttendee.FullName, meeting.Title, meetingDetail, paymentInfo, totalPaidAmount);
                    Mail.SendEmail(meeting.Organizer.Email1, MailResource.MeetingController_PaymentIsLow_Subject, textBody, true, true);

                    if (meeting.SecondaryOrganizerId.HasValue)
                    {
                        Mail.SendEmail(meeting.SecondaryOrganizer.Email1, MailResource.MeetingController_PaymentIsLow_Subject, textBody, true, true);
                    }
                }
                else if (totalPaidAmount > meetingPrice)
                {
                    meetingAttendee.MeetingAttendeeType = MeetingAttendeeType.Registred;
                    meetingAttendee.Registered = currentDateTime;

                    textBody = String.Format(MailResource.MeetingController_PaymentIsHigh_TextBody, meeting.Title, meetingDetail, paymentInfo, totalPaidAmount);
                    Mail.SendEmail(meetingAttendee.UserAttendee.Email1, MailResource.MeetingController_PaymentIsHigh_Subject, textBody, true, true);

                    textBody = String.Format(MailResource.MeetingController_Registered_TextBody, meeting.Title, meetingDetail);
                    Mail.SendEmail(meetingAttendee.UserAttendee.Email1, MailResource.MeetingController_Registered_Subject, textBody, true, true);

                    textBody = String.Format(MailResource.MeetingController_OrganizerPaymentIsHigh_TextBody, meetingAttendee.UserAttendee.FullName, meeting.Title, meetingDetail, paymentInfo, totalPaidAmount);
                    Mail.SendEmail(meeting.Organizer.Email1, MailResource.MeetingController_PaymentIsHigh_Subject, textBody, true, true);

                    if (meeting.SecondaryOrganizerId.HasValue)
                    {
                        Mail.SendEmail(meeting.SecondaryOrganizer.Email1, MailResource.MeetingController_PaymentIsHigh_Subject, textBody, true, true);
                    }
                }
                else
                {
                    meetingAttendee.MeetingAttendeeType = MeetingAttendeeType.Registred;
                    meetingAttendee.Registered = currentDateTime;

                    textBody = String.Format(MailResource.MeetingController_Registered_TextBody, meeting.Title, meetingDetail);
                    Mail.SendEmail(meetingAttendee.UserAttendee.Email1, MailResource.MeetingController_Registered_Subject, textBody, true, true);
                }

                return true;
            }

            // Registrace nebo akumulace nižší platby přihlášeného kontaktu
            var peopleContactMeetingAttendees = meetingAttendees.Where(ma => ma.AttendeeId.HasValue);
            meetingAttendee = peopleContactMeetingAttendees.FirstOrDefault(a => a.Attendee.LyonessId == bankAccountHistoryLyonessId);
            if (meetingAttendee != null)
            {
                if (meetingAttendee.Registered.HasValue)
                    return false;

                int totalPaidAmount = (isSecondBankAccount ? meetingAttendee.SecondPaidAmount : meetingAttendee.PaidAmount) + paidAmount;
                string totalBankAccountHistoryNote = String.IsNullOrEmpty(meetingAttendee.BankAccountHistoryNote)
                                                         ? bankAccountHistoryNote
                                                         : String.Format("{0}\n\n{1}", bankAccountHistoryNote, meetingAttendee.BankAccountHistoryNote);

                if (isSecondBankAccount)
                {
                    meetingAttendee.SecondPaidAmount = totalPaidAmount;
                }
                else
                {
                    meetingAttendee.PaidAmount = totalPaidAmount;
                }

                meetingAttendee.Reserved = currentDateTime;
                meetingAttendee.BankAccountHistoryNote = totalBankAccountHistoryNote;

                string meetingDetail = MeetingCommon.GetMeetingDetail(meeting);
                string paymentInfo = MeetingCommon.GetPaymentInfo(db, meeting, bankAccountHistoryLyonessId);
                string textBody;
                if (totalPaidAmount < meetingPrice)
                {
                    if (!String.IsNullOrEmpty(meetingAttendee.Attendee.Email1))
                    {
                        textBody = String.Format(MailResource.MeetingController_PaymentIsLow_TextBody, meeting.Title, meetingDetail, paymentInfo, totalPaidAmount);
                        Mail.SendEmail(meetingAttendee.Attendee.Email1, MailResource.MeetingController_PaymentIsLow_Subject, textBody, true, true);
                    }

                    textBody = String.Format(MailResource.MeetingController_OwnerPaymentIsLow_TextBody, meetingAttendee.Attendee.FullName, meeting.Title, meetingDetail, paymentInfo, totalPaidAmount);
                    Mail.SendEmail(meetingAttendee.Attendee.Registrar.Email1, MailResource.MeetingController_PaymentIsLow_Subject, textBody, true, true);

                    textBody = String.Format(MailResource.MeetingController_OrganizerContactPaymentIsLow_TextBody, meetingAttendee.Attendee.FullName, meeting.Title, meetingDetail, paymentInfo, totalPaidAmount);
                    Mail.SendEmail(meeting.Organizer.Email1, MailResource.MeetingController_PaymentIsLow_Subject, textBody, true, true);

                    if (meeting.SecondaryOrganizerId.HasValue)
                    {
                        Mail.SendEmail(meeting.SecondaryOrganizer.Email1, MailResource.MeetingController_PaymentIsLow_Subject, textBody, true, true);
                    }
                }
                else if (totalPaidAmount > meetingPrice)
                {
                    meetingAttendee.MeetingAttendeeType = MeetingAttendeeType.Registred;
                    meetingAttendee.Registered = currentDateTime;

                    if (!String.IsNullOrEmpty(meetingAttendee.Attendee.Email1))
                    {
                        textBody = String.Format(MailResource.MeetingController_PaymentIsHigh_TextBody, meeting.Title, meetingDetail, paymentInfo, totalPaidAmount);
                        Mail.SendEmail(meetingAttendee.Attendee.Email1, MailResource.MeetingController_PaymentIsHigh_Subject, textBody, true, true);

                        textBody = String.Format(MailResource.MeetingController_Registered_TextBody, meeting.Title, meetingDetail);
                        Mail.SendEmail(meetingAttendee.Attendee.Email1, MailResource.MeetingController_Registered_Subject, textBody, true, true);
                    }

                    textBody = String.Format(MailResource.MeetingController_OwnerPaymentIsHigh_TextBody, meetingAttendee.Attendee.FullName, meeting.Title, meetingDetail, paymentInfo, totalPaidAmount);
                    Mail.SendEmail(meetingAttendee.Attendee.Registrar.Email1, MailResource.MeetingController_PaymentIsHigh_Subject, textBody, true, true);

                    textBody = String.Format(MailResource.MeetingController_OwnerRegisteredContact_TextBody, meetingAttendee.Attendee.FullName, meeting.Title, meetingDetail);
                    Mail.SendEmail(meetingAttendee.Attendee.Registrar.Email1, MailResource.MeetingController_PaymentIsHigh_Subject, textBody, true, true);

                    textBody = String.Format(MailResource.MeetingController_OrganizerContactPaymentIsHigh_TextBody, meetingAttendee.Attendee.FullName, meeting.Title, meetingDetail, paymentInfo, totalPaidAmount);
                    Mail.SendEmail(meeting.Organizer.Email1, MailResource.MeetingController_PaymentIsHigh_Subject, textBody, true, true);

                    if (meeting.SecondaryOrganizerId.HasValue)
                    {
                        Mail.SendEmail(meeting.SecondaryOrganizer.Email1, MailResource.MeetingController_PaymentIsHigh_Subject, textBody, true, true);
                    }
                }
                else
                {
                    meetingAttendee.MeetingAttendeeType = MeetingAttendeeType.Registred;
                    meetingAttendee.Registered = currentDateTime;

                    if (!String.IsNullOrEmpty(meetingAttendee.Attendee.Email1))
                    {
                        textBody = String.Format(MailResource.MeetingController_Registered_TextBody, meeting.Title, meetingDetail);
                        Mail.SendEmail(meetingAttendee.Attendee.Email1, MailResource.MeetingController_Registered_Subject, textBody, true, true);
                    }

                    textBody = String.Format(MailResource.MeetingController_OwnerRegisteredContact_TextBody, meetingAttendee.Attendee.FullName, meeting.Title, meetingDetail);
                    Mail.SendEmail(meetingAttendee.Attendee.Registrar.Email1, MailResource.MeetingController_Registered_Subject, textBody, true, true);
                }

                return true;
            }

            if (freeCapacity <= 0)
                return false;

            meetingAttendee = new MeetingAttendee
            {
                MeetingId = meeting.MeetingId,
                MeetingAttendeeType = paidAmount >= meetingPrice ? MeetingAttendeeType.Registred : MeetingAttendeeType.Reserved,
                PaidAmount = isSecondBankAccount ? 0 : paidAmount,
                SecondPaidAmount = isSecondBankAccount ? paidAmount : 0,
                Reserved = currentDateTime,
                Registered = paidAmount >= meetingPrice ? currentDateTime : new DateTime?(),
                BonusLyonessId = bankAccountHistoryLyonessId,
                BankAccountHistoryNote = bankAccountHistoryNote
            };

            // Registrace nebo rezervace nepřihlášeného uživatele
            UserProfile[] userProfiles = db.UserProfiles.Where(up => up.LyonessId == bankAccountHistoryLyonessId).ToArray();
            if (userProfiles.Length == 1)
            {
                freeCapacity--;

                UserProfile userProfile = userProfiles[0];
                meetingAttendee.UserAttendeeId = userProfile.UserId;
                meetingAttendee.RegistrarId = userProfile.UserId;

                db.MeetingAttendees.Add(meetingAttendee);

                string meetingDetail = MeetingCommon.GetMeetingDetail(meeting);
                string paymentInfo = MeetingCommon.GetPaymentInfo(db, meeting, bankAccountHistoryLyonessId);
                string textBody;
                if (paidAmount < meetingPrice)
                {
                    textBody = String.Format(MailResource.MeetingController_Signed_TextBody, meeting.Title, maxReservationDate, meetingDetail, paymentInfo);
                    Mail.SendEmail(userProfile.Email1, MailResource.MeetingController_Signed_Subject, textBody, true, true);

                    textBody = String.Format(MailResource.MeetingController_PaymentIsLow_TextBody, meeting.Title, meetingDetail, paymentInfo, paidAmount);
                    Mail.SendEmail(userProfile.Email1, MailResource.MeetingController_PaymentIsLow_Subject, textBody, true, true);

                    textBody = String.Format(MailResource.MeetingController_OrganizerPaymentIsLow_TextBody, userProfile.FullName, meeting.Title, meetingDetail, paymentInfo, paidAmount);
                    Mail.SendEmail(meeting.Organizer.Email1, MailResource.MeetingController_PaymentIsLow_Subject, textBody, true, true);

                    if (meeting.SecondaryOrganizerId.HasValue)
                    {
                        Mail.SendEmail(meeting.SecondaryOrganizer.Email1, MailResource.MeetingController_PaymentIsLow_Subject, textBody, true, true);
                    }
                }
                else if (paidAmount > meetingPrice)
                {
                    textBody = String.Format(MailResource.MeetingController_PaymentIsHigh_TextBody, meeting.Title, meetingDetail, paymentInfo, paidAmount);
                    Mail.SendEmail(userProfile.Email1, MailResource.MeetingController_PaymentIsHigh_Subject, textBody, true, true);

                    textBody = String.Format(MailResource.MeetingController_Registered_TextBody, meeting.Title, meetingDetail);
                    Mail.SendEmail(userProfile.Email1, MailResource.MeetingController_Registered_Subject, textBody, true, true);

                    textBody = String.Format(MailResource.MeetingController_OrganizerPaymentIsHigh_TextBody, userProfile.FullName, meeting.Title, meetingDetail, paymentInfo, paidAmount);
                    Mail.SendEmail(meeting.Organizer.Email1, MailResource.MeetingController_PaymentIsHigh_Subject, textBody, true, true);

                    if (meeting.SecondaryOrganizerId.HasValue)
                    {
                        Mail.SendEmail(meeting.SecondaryOrganizer.Email1, MailResource.MeetingController_PaymentIsHigh_Subject, textBody, true, true);
                    }
                }
                else
                {
                    textBody = String.Format(MailResource.MeetingController_Registered_TextBody, meeting.Title, meetingDetail);
                    Mail.SendEmail(userProfile.Email1, MailResource.MeetingController_Registered_Subject, textBody, true, true);
                }

                return true;
            }

            // Registrace nebo rezervace nepřihlášeného kontaktu
            var peopleContacts = db.PeopleContacts.Where(pc => pc.LyonessId == bankAccountHistoryLyonessId).ToArray();
            if (peopleContacts.Length == 1)
            {
                freeCapacity--;

                PeopleContact peopleContact = peopleContacts[0];

                meetingAttendee.AttendeeId = peopleContact.PeopleContactId;
                meetingAttendee.RegistrarId = peopleContact.RegistrarId;

                db.MeetingAttendees.Add(meetingAttendee);

                string meetingDetail = MeetingCommon.GetMeetingDetail(meeting);
                string paymentInfo = MeetingCommon.GetPaymentInfo(db, meeting, bankAccountHistoryLyonessId);
                string textBody;
                if (paidAmount < meetingPrice)
                {
                    if (!String.IsNullOrEmpty(peopleContact.Email1))
                    {
                        textBody = String.Format(MailResource.MeetingController_Signed_TextBody, meeting.Title, maxReservationDate, meetingDetail, paymentInfo);
                        Mail.SendEmail(peopleContact.Email1, MailResource.MeetingController_Signed_Subject, textBody, true, true);

                        textBody = String.Format(MailResource.MeetingController_PaymentIsLow_TextBody, meeting.Title, meetingDetail, paymentInfo, paidAmount);
                        Mail.SendEmail(peopleContact.Email1, MailResource.MeetingController_PaymentIsLow_Subject, textBody, true, true);
                    }

                    textBody = String.Format(MailResource.MeetingController_OwnerSignedContact_TextBody, peopleContact.FullName, meeting.Title, maxReservationDate, meetingDetail, paymentInfo);
                    Mail.SendEmail(peopleContact.Registrar.Email1, MailResource.MeetingController_Signed_Subject, textBody, true, true);

                    textBody = String.Format(MailResource.MeetingController_OwnerPaymentIsLow_TextBody, peopleContact.FullName, meeting.Title, meetingDetail, paymentInfo, paidAmount);
                    Mail.SendEmail(peopleContact.Registrar.Email1, MailResource.MeetingController_PaymentIsLow_Subject, textBody, true, true);

                    textBody = String.Format(MailResource.MeetingController_OrganizerContactPaymentIsLow_TextBody, peopleContact.FullName, meeting.Title, meetingDetail, paymentInfo, paidAmount);
                    Mail.SendEmail(meeting.Organizer.Email1, MailResource.MeetingController_PaymentIsLow_Subject, textBody, true, true);

                    if (meeting.SecondaryOrganizerId.HasValue)
                    {
                        Mail.SendEmail(meeting.SecondaryOrganizer.Email1, MailResource.MeetingController_PaymentIsLow_Subject, textBody, true, true);
                    }
                }
                else if (paidAmount > meetingPrice)
                {
                    if (!String.IsNullOrEmpty(peopleContact.Email1))
                    {
                        textBody = String.Format(MailResource.MeetingController_PaymentIsHigh_TextBody, meeting.Title, meetingDetail, paymentInfo, paidAmount);
                        Mail.SendEmail(peopleContact.Email1, MailResource.MeetingController_PaymentIsHigh_Subject, textBody, true, true);

                        textBody = String.Format(MailResource.MeetingController_Registered_TextBody, meeting.Title, meetingDetail);
                        Mail.SendEmail(peopleContact.Email1, MailResource.MeetingController_Registered_Subject, textBody, true, true);
                    }

                    textBody = String.Format(MailResource.MeetingController_OwnerPaymentIsHigh_TextBody, peopleContact.FullName, meeting.Title, meetingDetail, paymentInfo, paidAmount);
                    Mail.SendEmail(peopleContact.Registrar.Email1, MailResource.MeetingController_PaymentIsHigh_Subject, textBody, true, true);

                    textBody = String.Format(MailResource.MeetingController_OwnerRegisteredContact_TextBody, peopleContact.FullName, meeting.Title, meetingDetail);
                    Mail.SendEmail(peopleContact.Registrar.Email1, MailResource.MeetingController_PaymentIsHigh_Subject, textBody, true, true);

                    textBody = String.Format(MailResource.MeetingController_OrganizerContactPaymentIsHigh_TextBody, peopleContact.FullName, meeting.Title, meetingDetail, paymentInfo, paidAmount);
                    Mail.SendEmail(meeting.Organizer.Email1, MailResource.MeetingController_PaymentIsHigh_Subject, textBody, true, true);

                    if (meeting.SecondaryOrganizerId.HasValue)
                    {
                        Mail.SendEmail(meeting.SecondaryOrganizer.Email1, MailResource.MeetingController_PaymentIsHigh_Subject, textBody, true, true);
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(peopleContact.Email1))
                    {
                        textBody = String.Format(MailResource.MeetingController_Registered_TextBody, meeting.Title, meetingDetail);
                        Mail.SendEmail(peopleContact.Email1, MailResource.MeetingController_Registered_Subject, textBody, true, true);
                    }

                    textBody = String.Format(MailResource.MeetingController_OwnerRegisteredContact_TextBody, peopleContact.FullName, meeting.Title, meetingDetail);
                    Mail.SendEmail(peopleContact.Registrar.Email1, MailResource.MeetingController_PaymentIsHigh_Subject, textBody, true, true);
                }

                return true;
            }

            return false;
        }

        private static void ProcessChangingApplicationAccessAmount(DefaultContext db, BankAccount bankAccount, Currency[] currencies, ref StringBuilder conversionExclamationMessages)
        {
            // Seskupit bankovní historii podle ID uživatele (Variabilní symbol)
            var groupedByVsBankAccountHistories = bankAccount.BankAccountHistories
                .Where(bah => bah.Vs.HasValue)
                .GroupBy(bah => bah.LyonessId, bah => bah,
                         (key, g) => new
                         {
                             LyonessId = key,
                             BankAccountHistories = g.ToArray()
                         }).ToArray();

            foreach (var groupedByVsBankAccountHistory in groupedByVsBankAccountHistories)
            {
                UserProfile userProfile = UserProfileCache.GetDetail(db, lyonessId: groupedByVsBankAccountHistory.LyonessId);
                if (userProfile == null)
                    continue;

                decimal amount = 0;
                foreach (BankAccountHistory bankAccountHistory in groupedByVsBankAccountHistory.BankAccountHistories)
                {
                    if (bankAccountHistory.CurrencyType == userProfile.ClaAccessCurrency)
                    {
                        amount += bankAccountHistory.Ammount;
                        continue;
                    }

                    decimal convertedAmount;
                    CurrencyHelper.TryConvertTo(bankAccountHistory.Ammount, bankAccountHistory.CurrencyType, userProfile.ClaAccessCurrency, currencies, out convertedAmount);
                    amount += convertedAmount;

                    conversionExclamationMessages.AppendLine(
                            String.Format(ViewResource.BankAccountHistory_MeetingPaymentWasConverted_Text,
                                          bankAccount.Title,
                                          userProfile.ClaAccessCurrency, bankAccountHistory.CurrencyType,
                                          bankAccountHistory.Vs.GetValueOrDefault(), bankAccountHistory.Note));
                }

                userProfile.ClaAccessAmount += amount;

                string textBody = String.Format(MailResource.TaskSchedulerController_AccessPaymentAccepted_TextBody, amount, userProfile.ClaAccessCurrency, userProfile.ClaAccessAmount);
                Mail.SendEmail(userProfile.Email1, MailResource.TaskSchedulerController_AccessPaymentAccepted_Subject, textBody, userProfile.UseMail, true);

                foreach (BankAccountHistory bankAccountHistory in groupedByVsBankAccountHistory.BankAccountHistories)
                {
                    db.BankAccountHistories.Remove(bankAccountHistory);
                }
            }
        }
    }
}