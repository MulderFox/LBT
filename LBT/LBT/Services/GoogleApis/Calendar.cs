// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 01-29-2014
//
// Last Modified By : zmikeska
// Last Modified On : 01-29-2014
// ***********************************************************************
// <copyright file="Calendar.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using LBT.Helpers;
using LBT.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LBT.Services.GoogleApis
{
    /// <summary>
    /// Class Calendar
    /// </summary>
    public class Calendar
    {
        private const string DefaultGoogleCalendarId = "primary";
        private const string EmailMethod = "email";
        private const string PopupMethod = "popup";
        private const string PrivateEventVisibility = "private";
        private const string DefaultEventVisibility = "default";

        /// <summary>
        /// The need redirect to google
        /// </summary>
        public bool NeedRedirectToGoogle { get; private set; }

        /// <summary>
        /// The redirect URI
        /// </summary>
        public string RedirectUri { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [use google calendar].
        /// </summary>
        /// <value><c>true</c> if [use google calendar]; otherwise, <c>false</c>.</value>
        public bool UseGoogleCalendar { get { return UseGoogleCalendarBySystem && UseGoogleCalendarByUser && !String.IsNullOrEmpty(GoogleCredentialsJson); } }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Calendar"/> is authorized.
        /// </summary>
        /// <value><c>true</c> if authorized; otherwise, <c>false</c>.</value>
        public bool Authorized { get; private set; }

        /// <summary>
        /// The google credentials json
        /// </summary>
        public string GoogleCredentialsJson;

        /// <summary>
        /// The google calendar id
        /// </summary>
        public string GoogleCalendarId;

        /// <summary>
        /// Gets a value indicating whether [use google calendar by user].
        /// </summary>
        /// <value><c>true</c> if [use google calendar by user]; otherwise, <c>false</c>.</value>
        public bool UseGoogleCalendarByUser;

        /// <summary>
        /// Gets a value indicating whether [use mail].
        /// </summary>
        /// <value><c>true</c> if [use mail]; otherwise, <c>false</c>.</value>
        public bool UseMail;

        /// <summary>
        /// Gets or sets the email to.
        /// </summary>
        /// <value>The email to.</value>
        public string EmailTo;

        /// <summary>
        /// The reminder time
        /// </summary>
        public string ReminderTime;

        /// <summary>
        /// The is events private
        /// </summary>
        public bool IsEventsPrivate;

        /// <summary>
        /// Gets a value indicating whether [use google calendar].
        /// </summary>
        /// <value><c>true</c> if [use google calendar]; otherwise, <c>false</c>.</value>
        private bool UseGoogleCalendarBySystem { get { return Properties.Settings.Default.UseGoogleCalendar; } }

        /// <summary>
        /// The _calendar service
        /// </summary>
        private CalendarService _calendarService;

        /// <summary>
        /// Authorizes the specified user id.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{System.Boolean}.</returns>
        public async Task AuthorizeAsync(Controller controller, CancellationToken cancellationToken)
        {
            Authorized = false;

            if (!UseGoogleCalendar)
                return;

            try
            {
                using (var appFlowMetadata = new AppFlowMetadata(HttpContext.Current.Request.PhysicalApplicationPath,GoogleCredentialsJson))
                {
                    if (!appFlowMetadata.Valid)
                        throw new Exception("AppFlowMetadata is not valid.");

                    var authorizationCodeMvcApp = new AuthorizationCodeMvcApp(controller, appFlowMetadata);
                    AuthorizationCodeWebApp.AuthResult result = await authorizationCodeMvcApp.AuthorizeAsync(cancellationToken);
                    if (result.Credential == null)
                    {
                        NeedRedirectToGoogle = true;
                        RedirectUri = result.RedirectUri;

                        Cookie.SetNewCookie(Cookie.GoogleCredentialsJsonCookieKey, GoogleCredentialsJson, controller.HttpContext.Response);
                        return;
                    }

                    var initializer = new BaseClientService.Initializer
                    {
                        HttpClientInitializer = result.Credential,
                        ApplicationName = Properties.Settings.Default.WebTitle
                    };
                    _calendarService = new CalendarService(initializer);

                    bool calendarValid = !String.IsNullOrEmpty(GoogleCalendarId)
                                             ? _calendarService.CalendarList.List().Execute().Items.Any(
                                                 cle => cle.Id == GoogleCalendarId)
                                             : _calendarService.CalendarList.List().Execute().Items.Any();
                    if (!calendarValid)
                        throw new Exception("Calendar Id is not valid.");                    
                }
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
                return;
            }

            Authorized = true;
        }

        /// <summary>
        /// Inserts events.
        /// </summary>
        /// <param name="events">The events.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public bool InsertEvents(Event[] events)
        {
            try
            {
                if (!UseGoogleCalendar)
                    return false;

                if (!Authorized)
                {
                    Mail.SendEmail(EmailTo, MailResource.Calendar_InsertEvents_Subject,
                                        MailResource.Calendar_InsertEvents_TextBody, UseMail, true);
                    return false;
                }

                if (String.IsNullOrEmpty(GoogleCalendarId))
                {
                    GoogleCalendarId = DefaultGoogleCalendarId;
                }

                foreach (Event @event in events)
                {
                    if (String.IsNullOrEmpty(ReminderTime))
                    {
                        var remindersData = new Event.RemindersData { UseDefault = false };
                        @event.Reminders = remindersData;
                    }
                    else
                    {
                        TimeSpan reminderTimeSpan;
                        if (TimeSpan.TryParse(ReminderTime, out reminderTimeSpan))
                        {
                            var emailEventReminder = new EventReminder
                                                         {
                                                             Method = EmailMethod,
                                                             Minutes = 0
                                                         };
                            var popupEventReminder = new EventReminder
                                                         {
                                                             Method = PopupMethod,
                                                             Minutes = 0
                                                         };
                            var remindersData = new Event.RemindersData
                                                    {
                                                        Overrides = new List<EventReminder>
                                                                        {
                                                                            emailEventReminder,
                                                                            popupEventReminder
                                                                        },
                                                        UseDefault = false
                                                    };
                            @event.Reminders = remindersData;
                            @event.Start.DateTime = @event.Start.DateTime.GetValueOrDefault().Add(reminderTimeSpan);
                        }
                    }

                    @event.Visibility = IsEventsPrivate ? PrivateEventVisibility : DefaultEventVisibility;
                    _calendarService.Events.Insert(@event, GoogleCalendarId).Execute();
                }
            }
            catch (Exception e)
            {
                Mail.SendEmail(EmailTo, MailResource.Calendar_InsertEvents_Subject,
                                        MailResource.Calendar_InsertEvents_TextBody, UseMail, true);
                Logger.SetLog(e);
                return false;
            }

            return true;
        }
    }
}