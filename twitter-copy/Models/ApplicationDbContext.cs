using Microsoft.EntityFrameworkCore;

namespace twitter_copy.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<AddPosts> AddPosts { get; set; }
        public DbSet<User> User { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
