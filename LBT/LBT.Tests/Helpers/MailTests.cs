// ***********************************************************************
// Assembly         : LBT.Tests
// Author           : zmikeska
// Created          : 02-03-2014
//
// Last Modified By : zmikeska
// Last Modified On : 02-03-2014
// ***********************************************************************
// <copyright file="MailTests.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using LBT.DAL;
using LBT.Helpers;
using NUnit.Framework;
using System;
using System.Linq;

namespace LBT.Tests.Helpers
{
    /// <summary>
    /// Class MailTests
    /// </summary>
    [TestFixture]
    public class MailTests
    {
        /// <summary>
        /// Tests the send email.
        /// </summary>
        [Test]
        public void TestSendEmail()
        {
            bool succeed = Mail.SendEmail("12th.century.fox@seznam.cz", "SUBJECT", "Automatic mail: TEXTBODY ěščřžýáíéůúďťňĚŠČŘŽÝÁÍÉÚŮĎŤŇ", true, true);
            succeed &= Mail.SendEmail("12th.century.fox@seznam.cz", "SUBJECT", "Manual mail: TEXTBODY ěščřžýáíéůúďťňĚŠČŘŽÝÁÍÉÚŮĎŤŇ", true, false);

            Assert.True(succeed);
        }

        [Test]
        public void SendMailToAllUsers()
        {
            //Mail.SendEmail("12th.century.fox@seznam.cz", "MIGRACE CLA Dokončena", Properties.Resources.MailBody, true, false);

            // select Email1 from UserProfile where Email1 is not null;
            string[] userProfileMails = Properties.Resources.Users.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string userProfileMail in userProfileMails)
            {
                Mail.SendEmail(userProfileMail, "MIGRACE CLA Dokončena", Properties.Resources.MailBody, true, false);
            }
        }

        [Test]
        public void SendApologizeMailToUsersWithAllowedMail()
        {
            using (var db = new DefaultContext())
            {
                string[] userProfileMails = db.UserProfiles.Where(up => !String.IsNullOrEmpty(up.Email1) && up.UseMail).Select(up => up.Email1).ToArray();
                foreach (string userProfileMail in userProfileMails)
                {
                    Mail.SendEmail(userProfileMail, "CLA - Omluva za úkoly z testovacího prostředí",
                                        "Pěkný den,\n\nběhem testování nové verze CLA zhruba kolem 17:45 došlo několikrát ke spuštění mechanismu pro rozesílání seznamu úkolů. Úkoly mohou být zastaralé. Email obsahuje odkaz na testovací prostředí.\n\nProsím neklikejte na odkaz.\n\nJeště jednou se omlouváme a děkujeme za pochopení.\n\nS pozdravem,\n\nCLA System.",
                                        true, false);
                }
            }
        }

        [Test]
        public void TestConvertHtmlToPlainText()
        {
            string plainEmail = HtmlToText.ConvertHtml(Properties.Resources.HtmlEmail);
            Assert.IsTrue(plainEmail == Properties.Resources.PlainEmail);
        }
    }
}
