using Microsoft.EntityFrameworkCore;
using WikiArticles.Models;

namespace WikiArticles.Services
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<Article> Articles { get; set; } = null!;
    }
}