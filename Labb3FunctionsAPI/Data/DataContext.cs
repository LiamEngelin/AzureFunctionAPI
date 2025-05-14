using Labb3FunctionsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Labb3FunctionsAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Character> Characters { get; set; }
    }
}
