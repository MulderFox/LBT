// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 01-24-2014
//
// Last Modified By : zmikeska
// Last Modified On : 01-24-2014
// ***********************************************************************
// <copyright file="MailHelper.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Runtime.InteropServices;
using LBT.Cache;
using LBT.DAL;
using LBT.Models;
using LBT.Properties;
using LBT.Resources;
using System;
using System.Linq;
using System.Text;

namespace LBT.Helpers
{
    /// <summary>
    /// Class MailHelper
    /// </summary>
    public static class Mail
    {
        private const string SmtpServer = "mail4.aspone.cz";
        private const int SmtpServerPort = 465;
        private const CDO.CdoSendUsing SendUsing = CDO.CdoSendUsing.cdoSendUsingPort;
        private const CDO.CdoProtocolsAuthentication SmtpAuthenticate = CDO.CdoProtocolsAuthentication.cdoBasic;
        private const string SendUserName = "system@lbt.aspone.cz";
        private const string SmtpUseSsl = "true";

        private static readonly string SendPassword;

        static Mail()
        {
            SendPassword = Cryptography.Decrypt(Settings.Default.MailServerPassword);
        }

        public static bool SendEmailToAdmins(DefaultContext db, string subject, string textBody)
        {
            UserProfile[] userProfiles = UserProfileCache.GetAdmins(db);
            bool success = userProfiles.Aggregate(true, (current, userProfile) => current & SendEmail(userProfile.Email1, subject, textBody, true, true));
            return success;
        }

        /// <summary>
        /// Sends the email or SMS.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="textBody">The text body.</param>
        /// <param name="useMailOrSms">if set to <c>true</c> [use email or SMS].</param>
        /// <param name="isAutomaticMail">if set to <c>true</c> [is automatic mail].</param>
        /// <param name="attachmentFilePath">The attachment file path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public static bool SendEmail(string address, string subject, string textBody, bool useMailOrSms, bool isAutomaticMail, string attachmentFilePath = null)
        {
            bool useMail = Settings.Default.UseMail;
            if (!useMail || !useMailOrSms || String.IsNullOrEmpty(address))
                return false;

            try
            {
                var message = new CDO.Message();
                message.Configuration.Fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"].Value =
                    SmtpServer;
                message.Configuration.Fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"].Value =
                    SmtpServerPort;
                message.Configuration.Fields["http://schemas.microsoft.com/cdo/configuration/sendusing"].Value =
                    SendUsing;
                message.Configuration.Fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"].Value =
                    SmtpAuthenticate;
                message.Configuration.Fields["http://schemas.microsoft.com/cdo/configuration/sendusername"].Value =
                    SendUserName;
                message.Configuration.Fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"].Value =
                    SendPassword;
                message.Configuration.Fields["http://schemas.microsoft.com/cdo/configuration/smtpusessl"].Value =
                    SmtpUseSsl;
                message.Configuration.Fields.Update();

                message.From = SendUserName;
                message.To = address;
                message.Subject = String.Format("{0}{1}", Settings.Default.EmailPrefix, subject);
                message.BodyPart.Charset = "UTF-8";

                Encoding defaultEncoding = Encoding.Default;
                Encoding utf8 = Encoding.UTF8;
                string wholeTextBody = isAutomaticMail
                                           ? String.Format(MailResource.Mail_AutomaticMailTextBodyTemplate, textBody)
                                           : textBody;
                byte[] defaultBytes = defaultEncoding.GetBytes(wholeTextBody);
                byte[] uftBytes = Encoding.Convert(defaultEncoding, utf8, defaultBytes);
                var utfChars = new char[utf8.GetCharCount(uftBytes, 0, uftBytes.Length)];
                utf8.GetChars(uftBytes, 0, uftBytes.Length, utfChars, 0);
                var text = new string(utfChars);
                message.TextBody = text;

                if (!String.IsNullOrEmpty(attachmentFilePath))
                {
                    message.AddAttachment(attachmentFilePath);
                }
                
                message.Send();
                ReleaseComObject(message);
            }
            catch (Exception e)
            {
                var logParameter = new Logger.LogParameter {AdditionalMessage = String.Format("address: {0}", address)};
                Logger.SetLog(e, logParameter);
                return false;
            }

            return true;
        }

        private static void ReleaseComObject(object obj)
        {
            try
            {
                Marshal.ReleaseComObject(obj);
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

    }
}