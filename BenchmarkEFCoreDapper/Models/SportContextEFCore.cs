using Microsoft.EntityFrameworkCore;

namespace BenchmarkEFCoreDapper
{
    public class SportContextEFCore : DbContext
    {
        public SportContextEFCore(DbContextOptions options) : base(options) { }

        public DbSet<Player> Players { get; set; }
        public DbSet<Sport> Sports { get; set; }
        public DbSet<Team> Teams { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(Constants.SportsConnectionString);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Player
            modelBuilder.Entity<Player>().Property(p => p.Id).ValueGeneratedNever();
            modelBuilder.Entity<Player>()
                .Property(p => p.FirstName).HasMaxLength(200).IsRequired();
            modelBuilder.Entity<Player>()
                .Property(p => p.LastName).HasMaxLength(200).IsRequired();

            // Sport
            modelBuilder.Entity<Sport>().Property(s => s.Id).ValueGeneratedNever();
            modelBuilder.Entity<Sport>()
                .HasMany(s => s.Teams);

            // Team
            modelBuilder.Entity<Team>().Property(t => t.Id).ValueGeneratedNever();
            modelBuilder.Entity<Team>()
                .HasMany(t => t.Players);
            modelBuilder.Entity<Team>()
                .Property(t => t.Name).HasMaxLength(200).IsRequired();
        }
    }
}
