using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Dolarium.Models;

namespace Dolarium.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) {}

        public DbSet<Keys> Keys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Keys>(entity =>
                {
                    entity.ToTable("Keys");
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.Limit).IsRequired();
                    entity.Property(e => e.Used).IsRequired();
                    entity.Property(e => e.CreatedAt).IsRequired();
                });
        }
    }
}
