using Microsoft.EntityFrameworkCore;

namespace CLSWeb.Models
{
    public class CLSDBContext : DbContext
    {
        public CLSDBContext(
            DbContextOptions<CLSDBContext> options)
            : base(options)
        {
        }

        public DbSet<Member> Members { get; set; }

        public DbSet<Listing> Listings { get; set; }

        public DbSet<Photo> Photos { get; set; }
    }
}