// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 01-26-2014
//
// Last Modified By : zmikeska
// Last Modified On : 06-04-2014
// ***********************************************************************
// <copyright file="DefaultContext.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using LBT.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace LBT.DAL
{
    /// <summary>
    /// Class DefaultContext
    /// </summary>
    public class DefaultContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultContext"/> class.
        /// </summary>
        public DefaultContext()
            : base("DefaultConnection")
        {
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 180;
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<PhoneNumberPrefix> PhoneNumberPrefixes { get; set; }
        public DbSet<PeopleContact> PeopleContacts { get; set; }
        //public DbSet<MSPContact> MSPContacts { get; set; }
        public DbSet<SharedContact> SharedContacts { get; set; }
        //public DbSet<SharedMSPContact> SharedMSPContacts { get; set; }
        public DbSet<Statistics> Statistics { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<BankAccountHistory> BankAccountHistories { get; set; }
        public DbSet<TopTen> TopTens { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<MeetingAttendee> MeetingAttendees { get; set; }
        public DbSet<MeetingTitleType> MeetingTitleTypes { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<BankAccountUser> BankAccountUsers { get; set; }
        public DbSet<PropertiesBag> PropertiesBags { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<VideoUser> VideoUsers { get; set; }
        public DbSet<VideoToken> VideoTokens { get; set; }
        public DbSet<LazyMail> LazyMails { get; set; }
        public DbSet<ManualType> ManualTypes { get; set; }
        public DbSet<Manual> Manuals { get; set; }

        /// <summary>
        /// This method is called when the model for a derived context has been initialized, but
        /// before the model has been locked down and used to initialize the context.  The default
        /// implementation of this method does nothing, but it can be overridden in a derived class
        /// such that the model can be further configured before it is locked down.
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
        /// <remarks>Typically, this method is called only once when the first instance of a derived context
        /// is created.  The model for that context is then cached and is for all further instances of
        /// the context in the app domain.  This caching can be disabled by setting the ModelCaching
        /// property on the given ModelBuidler, but note that this can seriously degrade performance.
        /// More control over caching is provided through use of the DbModelBuilder and DbContextFactory
        /// classes directly.</remarks>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
        }
    }
}