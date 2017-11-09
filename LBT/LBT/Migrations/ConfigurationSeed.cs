// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 01-26-2014
//
// Last Modified By : zmikeska
// Last Modified On : 05-09-2014
// ***********************************************************************
// <copyright file="ConfigurationSeed.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using LBT.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Security;
using WebMatrix.WebData;

namespace LBT.Migrations
{
    /// <summary>
    /// Class ConfigurationSeed
    /// </summary>
    public class ConfigurationSeed
    {
        //  This method will be called after migrating to the latest version.

        //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
        //  to avoid creating duplicate seed data. E.g.
        //
        //    context.People.AddOrUpdate(
        //      p => p.FullName,
        //      new Person { FullName = "Andrew Peters" },
        //      new Person { FullName = "Brice Lambson" },
        //      new Person { FullName = "Rowan Miller" }
        //    );
        //

        /// <summary>
        /// Seeds the default context.
        /// </summary>
        /// <param name="context">The context.</param>
        public static void SeedContext(DAL.DefaultContext context)
        {
            DbSet<District> districtSet = context.Districts;
            FixDistricts(ref districtSet);

            DbSet<PhoneNumberPrefix> phoneNumberPrefixSet = context.PhoneNumberPrefixes;
            FixPhoneNumberPrefixes(ref phoneNumberPrefixSet);

            DbSet<Meeting> meetingSet = context.Meetings;
            if (context.MeetingTitleTypes.All(mtt => mtt.MeetingType != MeetingType.SetkaniTymu))
            {
                DbSet<MeetingTitleType> meetingTitleTypeSet = context.MeetingTitleTypes;
                DbContext dbContext = context;
                FixMeetingTitleTypesAndMeetings(ref dbContext, ref meetingTitleTypeSet, ref meetingSet);
            }

            if (context.BankAccounts.Any(ba => ba.Token == "V2-1-0-21"))
            {
                DbSet<BankAccount> bankAccountSet = context.BankAccounts;
                FixBankAccountsAndMeetings(ref bankAccountSet, ref meetingSet);
            }

            context.SaveChanges();

            if (context.UserProfiles.Any())
                return;

            CreateAdminAccount("DefaultConnection");
        }

        /// <summary>
        /// Seeds the debug context.
        /// </summary>
        /// <param name="context">The context.</param>
        public static void SeedContext(DAL.DebugContext context)
        {
            DbSet<District> districtSet = context.Districts;
            FixDistricts(ref districtSet);

            DbSet<PhoneNumberPrefix> phoneNumberPrefixSet = context.PhoneNumberPrefixes;
            FixPhoneNumberPrefixes(ref phoneNumberPrefixSet);

            DbSet<Meeting> meetingSet = context.Meetings;
            if (context.MeetingTitleTypes.All(mtt => mtt.MeetingType != MeetingType.SetkaniTymu))
            {
                DbSet<MeetingTitleType> meetingTitleTypeSet = context.MeetingTitleTypes;
                DbContext dbContext = context;
                FixMeetingTitleTypesAndMeetings(ref dbContext, ref meetingTitleTypeSet, ref meetingSet);
            }

            if (context.BankAccounts.Any(ba => ba.Token == "V2-1-0-21"))
            {
                DbSet<BankAccount> bankAccountSet = context.BankAccounts;
                FixBankAccountsAndMeetings(ref bankAccountSet, ref meetingSet);
            }

            context.SaveChanges();

            if (context.UserProfiles.Any())
                return;

            CreateAdminAccount("DebugConnection");
        }

        /// <summary>
        /// Seeds the release context.
        /// </summary>
        /// <param name="context">The context.</param>
        public static void SeedContext(DAL.ReleaseContext context)
        {
            DbSet<District> districtSet = context.Districts;
            FixDistricts(ref districtSet);

            DbSet<PhoneNumberPrefix> phoneNumberPrefixSet = context.PhoneNumberPrefixes;
            FixPhoneNumberPrefixes(ref phoneNumberPrefixSet);

            DbSet<Meeting> meetingSet = context.Meetings;
            if (context.MeetingTitleTypes.All(mtt => mtt.MeetingType != MeetingType.SetkaniTymu))
            {
                DbSet<MeetingTitleType> meetingTitleTypeSet = context.MeetingTitleTypes;
                DbContext dbContext = context;
                FixMeetingTitleTypesAndMeetings(ref dbContext, ref meetingTitleTypeSet, ref meetingSet);
            }

            if (context.BankAccounts.Any(ba => ba.Token == "V2-1-0-21"))
            {
                DbSet<BankAccount> bankAccountSet = context.BankAccounts;
                FixBankAccountsAndMeetings(ref bankAccountSet, ref meetingSet);
            }

            context.SaveChanges();

            if (context.UserProfiles.Any())
                return;

            CreateAdminAccount("ReleaseConnection");
        }

        private static void FixDistricts(ref DbSet<District> districtSet)
        {
            var district = new District
            {
                DistrictId = 1,
                Title = "Mladá Boleslav"
            };
            districtSet.AddOrUpdate(district);
        }

        private static void FixPhoneNumberPrefixes(ref DbSet<PhoneNumberPrefix> phoneNumberPrefixSet)
        {
            var phoneNumberPrefixe = new PhoneNumberPrefix
            {
                PhoneNumberPrefixId = 1,
                Title = "CZ +420",
                MatchRegex = "^(00420|420).*$",
                ReplaceRegex = "^(00420|420)",
                ExportablePrefix = "00420"
            };
            phoneNumberPrefixSet.AddOrUpdate(phoneNumberPrefixe);
        }

        /// <summary>
        /// Creates the admin account.
        /// </summary>
        private static void CreateAdminAccount(string connectionStringName)
        {
            if (!WebSecurity.Initialized)
            {
                WebSecurity.InitializeDatabaseConnection(connectionStringName, "UserProfile", "UserId", "UserName",
                                                         autoCreateTables: true);
            }

            var propertyValues = new
            {
                DistrictId = 1,
                PhoneNumberPrefix1Id = 1,
                FirstName = "Leoš",
                LastName = "Červený",
                Title1 = "Ing.",
                City = "Kněžmost",
                PhoneNumber1 = "605293287",
                Email1 = "cerveny.leos@seznam.cz",
                AccessGranted = DateTime.Now,
                Ca = 0,
                Cc = 0,
                Presenting = 0,
                Mps = 0
            };
            WebSecurity.CreateUserAndAccount("dzccele", "bedovo", propertyValues);

            foreach (string role in Enum.GetNames(typeof(RoleType)).Where(role => !Roles.RoleExists(role)))
            {
                Roles.CreateRole(role);
            }

            Roles.AddUserToRole("dzccele", RoleType.AdminLeader.ToString());
        }

        private static void FixBankAccountsAndMeetings(ref DbSet<BankAccount> bankAccountSet, ref DbSet<Meeting> meetingSet)
        {
            Meeting[] meetings = meetingSet.Where(m => m.Finished > DateTime.Now).ToArray();
            BankAccount[] bankAccounts = bankAccountSet.Where(ba => ba.Token != "V2-1-0-21").ToArray();
            foreach (Meeting meeting in meetings)
            {
                BankAccountType bankAccountType;
                switch (meeting.MeetingType)
                {
                    case MeetingType.BusinessInfo:
                    case MeetingType.WorkshopyBi:
                    case MeetingType.Leaders:
                    case MeetingType.MspEvening:
                        bankAccountType = BankAccountType.BusinessInfoOrMspEveningOrWorkshopsOrLeaders;
                        break;

                    case MeetingType.SetkaniTymu:
                        bankAccountType = BankAccountType.TeamMeeting;
                        break;

                    case MeetingType.SkoleniDavidaKotaska:
                        bankAccountType = BankAccountType.DavidKotasekTraining;
                        break;

                    case MeetingType.Ostatni:
                        bankAccountType = BankAccountType.Others;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                BankAccount bankAccount = bankAccounts.FirstOrDefault(ba => ba.CurrencyType == CurrencyType.CZK && ba.BankAccountType == bankAccountType);
                if (bankAccount == null)
                    continue;

                meeting.BankAccountId = bankAccount.BankAccountId;
            }

            bankAccounts = bankAccountSet.Where(ba => ba.Title == String.Empty && ba.Token != "V2-1-0-21").ToArray();
            foreach (BankAccount bankAccount in bankAccounts)
            {
                bankAccount.Title = bankAccount.BankAccountTypeLocalizedText;
                bankAccount.Owner = bankAccount.BankAccountTypeLocalizedText;
            }

            bankAccounts = bankAccountSet.Where(ba => ba.Token == "V2-1-0-21").ToArray();
            foreach (BankAccount bankAccount in bankAccounts)
            {
                bankAccountSet.Remove(bankAccount);
            }
        }

        private static void FixMeetingTitleTypesAndMeetings(ref DbContext dbContext, ref DbSet<MeetingTitleType> meetingTitleTypeSet, ref DbSet<Meeting> meetingSet)
        {
            var meetingTitleType = meetingTitleTypeSet.FirstOrDefault(mtts => mtts.Title == "ST");
            if (meetingTitleType == null)
            {
                meetingTitleType = new MeetingTitleType
                {
                    MeetingType = MeetingType.SetkaniTymu,
                    Title = "ST"
                };
                meetingTitleTypeSet.AddOrUpdate(meetingTitleType);                
            }
            else
            {
                meetingTitleType.MeetingType = MeetingType.SetkaniTymu;
            }

            dbContext.SaveChanges();

            // Nastavit defaultní název setkání týmu
            meetingTitleType = meetingTitleTypeSet.First(mtt => mtt.Title == "ST");
            Meeting[] meetings = meetingSet.Where(m => m.MeetingType == MeetingType.SetkaniTymu && !m.MeetingTitleTypeId.HasValue).ToArray();
            if (meetings.Length == 0)
                return;

            foreach (Meeting meeting in meetings)
            {
                meeting.MeetingTitleTypeId = meetingTitleType.MeetingTitleTypeId;
            }
        }
    }
}