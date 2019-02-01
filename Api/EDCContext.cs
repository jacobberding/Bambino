using Api.Models;
using System.Data.Entity;

namespace Api
{
    public class EDCContext : DbContext
    {
        public EDCContext() : base("Data Source=3.16.85.101;Initial Catalog=edc;Integrated Security=False;Persist Security Info=True;User ID=WebDeploy;Password=VXN526ZJ9nWvRTVn;MultipleActiveResultSets=true;") { }

        public DbSet<ACAreaCategory> ACAreaCategories { get; set; }
        public DbSet<ACLayer> ACLayers { get; set; }
        public DbSet<ACLayerCategory> ACLayerCategories { get; set; }
        public DbSet<ApiToken> ApiTokens { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<MaterialTag> MaterialTags { get; set; }
        public DbSet<MaterialPriceOption> MaterialPriceOptions { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<MemberIpAddress> MemberIpAddresses { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectPhase> ProjectPhases { get; set; }
        public DbSet<ProjectZone> ProjectZones { get; set; }
        public DbSet<ProjectAttraction> ProjectAttractions { get; set; }
        public DbSet<ProjectElement> ProjectElements { get; set; }
        public DbSet<ProjectWritingDocument> ProjectWritingDocuments { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Tsk> Tsks { get; set; }
        public DbSet<SubTsk> SubTsks { get; set; }
        public DbSet<TimeTracker> TimeTrackers { get; set; }
        public DbSet<TimeTrackerProject> TimeTrackerProjects { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Material>().Property(x => x.priceMin).HasPrecision(18, 4);
            modelBuilder.Entity<Material>().Property(x => x.priceMax).HasPrecision(18, 4);
            modelBuilder.Entity<TimeTracker>().Property(x => x.totalHours).HasPrecision(18, 4);
            modelBuilder.Entity<TimeTrackerProject>().Property(x => x.totalHours).HasPrecision(18, 4);

            //Database.SetInitializer<MESHContext>(null);

            //modelBuilder.Entity<Member>()
            //    .HasMany(u => u.invitationRequests) // <--
            //    .WithRequired(r => r.requestedByMember) // <--
            //    .HasForeignKey(r => r.requestedByMemberId);

            modelBuilder.Entity<Member>()
                .HasMany<Company>(s => s.companies)
                .WithMany(c => c.members)
                .Map(cs =>
                {
                    cs.MapLeftKey("memberId");
                    cs.MapRightKey("companyId");
                    cs.ToTable("MemberCompanies");
                });

            modelBuilder.Entity<Project>()
                .HasMany<Member>(s => s.members)
                .WithMany(c => c.projects)
                .Map(cs =>
                {
                    cs.MapLeftKey("projectId");
                    cs.MapRightKey("memberId");
                    cs.ToTable("ProjectMembers");
                });

            modelBuilder.Entity<Material>()
                .HasMany<MaterialTag>(s => s.materialTags)
                .WithMany(c => c.materials)
                .Map(cs =>
                {
                    cs.MapLeftKey("materialId");
                    cs.MapRightKey("materialTagId");
                    cs.ToTable("MaterialTagMaterials");
                });

            modelBuilder.Entity<Member>()
                .HasMany<Role>(s => s.roles)
                .WithMany(c => c.members)
                .Map(cs =>
                {
                    cs.MapLeftKey("memberId");
                    cs.MapRightKey("roleId");
                    cs.ToTable("MemberRoles");
                });
            
        }

    }
}