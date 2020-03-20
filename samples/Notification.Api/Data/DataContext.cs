using Microsoft.EntityFrameworkCore;

namespace Notification.Api
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
            
        }
        public DbSet<User> Users { get; set; }
        public DbSet<MailTemplate> MailTemplates { get; set; }
    }
}