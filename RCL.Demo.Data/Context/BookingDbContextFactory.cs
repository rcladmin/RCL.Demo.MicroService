#nullable disable

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace RCL.Demo.Data
{
    public class BookingDbContextFactory : IDesignTimeDbContextFactory<BookingDbContext>
    {
        public BookingDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BookingDbContext>();
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("ConnectionStrings:Database"),
            sqlServerOptions => sqlServerOptions.MigrationsAssembly("RCL.Demo.Data"));

            return new BookingDbContext(optionsBuilder.Options);
        }
    }
}
