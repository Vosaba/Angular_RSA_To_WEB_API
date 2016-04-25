using System.Data.Entity;
using WebApp.Models;

namespace WebApp.DataAccessLayer
{
    public class DataContext : DbContext
    {

        public DataContext() : base("DefaultConnection")
        {
            Database.SetInitializer<DataContext>
                (new DropCreateDatabaseIfModelChanges<DataContext>());
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<User> UsersKey { get; set; }

    }

    
}