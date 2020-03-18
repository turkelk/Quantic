using Microsoft.EntityFrameworkCore;

namespace Identity.Api
{
    public class DataContext : DbContext
    {      
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }  

        public DbSet<User> Users { get; set; }              
    }
}