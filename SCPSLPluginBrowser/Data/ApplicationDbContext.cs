using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SCPSLPluginBrowser.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {

        public DbSet<DllFile> DllFiles { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Flag> Flags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Like>()
                .HasOne(l => l.DllFile)
                .WithMany(d => d.Likes)
                .HasForeignKey(l => l.DllFileId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.DllFile)
                .WithMany(d => d.Comments)
                .HasForeignKey(c => c.DllFileId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Flag>()
                .HasOne(f => f.DllFile)
                .WithMany(d => d.Flags)
                .HasForeignKey(f => f.DllFileId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Flag>()
                .HasOne(f => f.User)
                .WithMany(u => u.Flags)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict); 
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
