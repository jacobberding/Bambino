using Api.Models;
using System.Data.Entity;

namespace Api
{
    public class EDCContext : DbContext
    {
        public EDCContext() : base("Data Source=3.16.85.101;Initial Catalog=edc;Integrated Security=False;Persist Security Info=True;User ID=WebDeploy;Password=VXN526ZJ9nWvRTVn;MultipleActiveResultSets=true;") { }

        public DbSet<Layer> Layers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TagAreaType> TagAreaTypes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            
            //modelBuilder.Entity<ShippingRate>().Property(x => x.price).HasPrecision(18, 4);
            
            //Database.SetInitializer<MESHContext>(null);
            
            //modelBuilder.Entity<Member>()
            //    .HasMany(u => u.invitationRequests) // <--
            //    .WithRequired(r => r.requestedByMember) // <--
            //    .HasForeignKey(r => r.requestedByMemberId);
            
            //modelBuilder.Entity<Member>()
            //    .HasMany<Role>(s => s.roles)
            //    .WithMany(c => c.members)
            //    .Map(cs =>
            //            {
            //                cs.MapLeftKey("memberId");
            //                cs.MapRightKey("roleId");
            //                cs.ToTable("MemberRoles");
            //            });
            
        }

    }
}