using MessagingApp.api.Models;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext>options):base(options)
        {

        }

       
        public DbSet<Value> Values  { get; set; }
    }
}