using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TestCreator.Data.Models;

namespace TestCreator.Data.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
                
        }

        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Token> Tokens { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().ToTable("Users");
            modelBuilder.Entity<ApplicationUser>().HasMany(a => a.Tests).WithOne(i => i.User);
            modelBuilder.Entity<ApplicationUser>().HasMany(a => a.Tokens).WithOne(i => i.User);

            modelBuilder.Entity<Test>().ToTable("Tests");
            modelBuilder.Entity<Test>().Property(t => t.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Test>().HasOne(t => t.User).WithMany(u => u.Tests);
            modelBuilder.Entity<Test>().HasMany(t => t.Questions).WithOne(q => q.Test);

            modelBuilder.Entity<Question>().ToTable("Questions");
            modelBuilder.Entity<Question>().Property(q => q.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Question>().HasOne(q => q.Test).WithMany(t => t.Questions);
            modelBuilder.Entity<Question>().HasMany(q => q.Answers).WithOne(a => a.Question);

            modelBuilder.Entity<Answer>().ToTable("Answers");
            modelBuilder.Entity<Answer>().Property(a => a.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Answer>().HasOne(a => a.Question).WithMany(q => q.Answers);

            modelBuilder.Entity<Result>().ToTable("Results");
            modelBuilder.Entity<Result>().Property(r => r.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Result>().HasOne(r => r.Test).WithMany(t => t.Results);

            modelBuilder.Entity<Token>().ToTable("Tokens");
            modelBuilder.Entity<Token>().Property(t => t.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Token>().HasOne(t => t.User).WithMany(a => a.Tokens);
        }
    }
}
