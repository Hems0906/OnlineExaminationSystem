using System.Collections.Generic;
using System.Data.Entity;

namespace MVC_OES.Models
{
    public class ApplicationDbContext : DbContext
    {
        // Constructor calling the base connection string
        public ApplicationDbContext() : base("DefaultConnection") { }

// Your database tables as DbSets
        public DbSet<Question> Questions { get; set; }

        // You can add more DbSet<T> for other tables later
    }
}
