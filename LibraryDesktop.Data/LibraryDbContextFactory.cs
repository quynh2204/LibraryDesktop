using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LibraryDesktop.Data
{
    public class LibraryDbContextFactory : IDesignTimeDbContextFactory<LibraryDbContext>
    {
        public LibraryDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<LibraryDbContext>();
            
            // Use a default connection string for migrations
            optionsBuilder.UseSqlite("Data Source=Library.db");
            
            return new LibraryDbContext(optionsBuilder.Options);
        }
    }
}
