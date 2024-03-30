using Microsoft.EntityFrameworkCore;

namespace WebAPI.Model
{
    public class MyDbContext : DbContext
    {

        public MyDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Emp> empapi { get; set; }

    }
}
