using MicorRMQ.Consumer.Domain;
using Microsoft.EntityFrameworkCore;
namespace MicorRMQ.Consumer.Persistence
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<TransferLog> TransferLogs { get; set; }
    }
}