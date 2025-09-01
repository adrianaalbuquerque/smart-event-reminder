using Microsoft.EntityFrameworkCore;
using SmartEventReminder.Domain.Models;

namespace SmartEventReminder.Domain
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Event> Events { get; set; }

       protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=smart_event;Username=postgres;Password=1234");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<Event>().ToTable("events");
        }

    }
}
