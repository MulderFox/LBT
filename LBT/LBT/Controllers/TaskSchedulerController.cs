// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 02-21-2014
//
// Last Modified By : zmikeska
// Last Modified On : 02-26-2014
// ***********************************************************************
// <copyright file="TaskSchedulerController.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using LBT.Cache;
using LBT.DAL;
using LBT.Helpers;
using System;
using System.Text;
using System.Web.Mvc;

namespace LBT.Controllers
{
    /// <summary>
    /// Class TaskSchedulerController
    /// </summary>
    public class TaskSchedulerController : Controller
    {
        private static bool _isCheckTasksManualRunning;
        private static bool _isExecuteHourlyJobsRunning;
        private static bool _isExecuteDailyJobsRunning;

        /// <summary>
        /// Disposes the refresh statistics.
        /// </summary>
        public static void DisposeTaskScheduler()
        {
        }

        /// <summary>
        /// Checks the tasks manual.
        /// </summary>
        /// <returns>ActionResult.</returns>
        [AllowAnonymous]
        public ActionResult CheckTasksManual()
        {
            if (_isCheckTasksManualRunning)
                return View("CheckTasks", "_ConsoleLayout");

            _isCheckTasksManualRunning = true;

            try
            {
                using (var db = new DefaultContext())
                {
                    ProcessCheckTasks(db);
                }
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
            }

            _isCheckTasksManualRunning = false;

            return View("CheckTasks", "_ConsoleLayout");
        }

        /// <summary>
        /// Checks the tasks manual.
        /// </summary>
        /// <returns>ActionResult.</returns>
        [AllowAnonymous]
        public ActionResult ExecuteHourlyJobs()
        {
            if (_isExecuteHourlyJobsRunning)
                return View("ExecuteHourlyJobs", "_ConsoleLayout");

            _isExecuteHourlyJobsRunning = true;

            try
            {
                using (var db = new DefaultContext())
                {
                    ProcessImportBankTransactions(db);
                    ProcessBankAccountHistory(db);
                    ProcessAccessPayments(db);
                    ProcessReservationExpiration(db);
                    ProcessRefreshTopTenRoles(db);
                    ProcessExpireVideoTokens(db);
                    ProcessSendLazyMails(db);
                }
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
            }

            _isExecuteHourlyJobsRunning = false;

            return View("ExecuteHourlyJobs", "_ConsoleLayout");
        }

        [AllowAnonymous]
        public ActionResult ExecuteDailyJobs()
        {
            if (_isExecuteDailyJobsRunning)
                return View("ExecuteDailyJobs", "_ConsoleLayout");

            _isExecuteDailyJobsRunning = true;

            try
            {
                using (var db = new DefaultContext())
                {
                    ProcessRefreshStatistics(db);
                    ProcessCascadeRemoveArchivedMeeting(db);
                }
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
            }

            _isExecuteDailyJobsRunning = false;

            return View("ExecuteDailyJobs", "_ConsoleLayout");
        }

        /// <summary>
        /// Executes the refresh statistics.
        /// </summary>
        private void ProcessRefreshStatistics(DefaultContext db)
        {
            Logger.SetLog("ProcessRefreshStatistics was started.");

            try
            {
                StatisticsCache.RefreshStatistics(db);
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
            }

            Logger.SetLog("ProcessRefreshStatistics was finished.");
        }

        private void ProcessCascadeRemoveArchivedMeeting(DefaultContext db)
        {
            Logger.SetLog("ProcessCascadeRemoveArchivedMeeting was started.");

            try
            {
                MeetingCache.CascadeRemoveArchivedMeeting(db);
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
            }

            Logger.SetLog("ProcessCascadeRemoveArchivedMeeting was finished.");
        }

        /// <summary>
        /// Executes the check tasks.
        /// </summary>
        private void ProcessCheckTasks(DefaultContext db)
        {
            Logger.SetLog("ProcessCheckTasks was started.");

            try
            {
                PeopleContactCache.CheckTasks(db);
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
            }

            Logger.SetLog("ProcessCheckTasks was finished.");
        }

        /// <summary>
        /// Executes the import bank transactions.
        /// </summary>
        private void ProcessImportBankTransactions(DefaultContext db)
        {
            Logger.SetLog("ProcessImportBankTransactions was started.");

            string bankToken = "Unknown";
            string failedUrl = "Unknown";
            try
            {
                BankAccountCache.ImportBankTransactions(db, out bankToken, out failedUrl);
            }
            catch (Exception e)
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(String.Format("Bank Token: {0}", bankToken));
                stringBuilder.AppendLine(String.Format("Failed URL: {0}", failedUrl));
                Logger.SetLog(e, new Logger.LogParameter { AdditionalMessage = stringBuilder.ToString() });
            }

            Logger.SetLog("ProcessImportBankTransactions was finished.");
        }

        private void ProcessBankAccountHistory(DefaultContext db)
        {
            Logger.SetLog("ProcessBankAccountHistory was started.");

            try
            {
                BankAccountCache.ProcessBankAccountHistory(db);
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
            }

            Logger.SetLog("ProcessBankAccountHistory was finished.");
        }

        private void ProcessAccessPayments(DefaultContext db)
        {
            Logger.SetLog("ProcessAccessPayments was started.");

            try
            {
                UserProfileCache.ProcessAccessPayments(db);
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
            }

            Logger.SetLog("ProcessAccessPayments was finished.");
        }

        private void ProcessReservationExpiration(DefaultContext db)
        {
            Logger.SetLog("ProcessReservationExpiration was started.");

            try
            {
                MeetingAttendeeCache.ProcessReservationExpiration(db);
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
            }

            Logger.SetLog("ProcessReservationExpiration was finished.");
        }

        private void ProcessRefreshTopTenRoles(DefaultContext db)
        {
            Logger.SetLog("ProcessRefreshTopTenRoles was started.");

            try
            {
                TopTenCache.RefreshTopTenRoles(db);
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
            }

            Logger.SetLog("ProcessRefreshTopTenRoles was finished.");
        }

        private void ProcessExpireVideoTokens(DefaultContext db)
        {
            Logger.SetLog("ProcessExpireVideoTokens was started.");

            try
            {
                VideoTokenCache.ExpireVideoTokens(db);
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
            }

            Logger.SetLog("ProcessExpireVideoTokens was finished.");
        }

        private void ProcessSendLazyMails(DefaultContext db)
        {
            Logger.SetLog("ProcessSendLazyMails was started.");

            try
            {
                LazyMailCache.SendLazyMails(db);
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
            }

            Logger.SetLog("ProcessSendLazyMails was finished.");
        }
    }
}
