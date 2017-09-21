using LBT.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace LBT.DAL
{
    public class ReleaseContext : DbContext
    {
        public ReleaseContext()
            : base("ReleaseConnection")
        {
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
        }
    }
}