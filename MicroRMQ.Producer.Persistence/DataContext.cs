using MicroRMQ.Producer.Domain;
using Microsoft.EntityFrameworkCore;

namespace MicroRMQ.Producer.Persistence
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
    }
}