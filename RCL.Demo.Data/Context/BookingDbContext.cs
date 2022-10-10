#nullable disable

using Microsoft.EntityFrameworkCore;

namespace RCL.Demo.Data
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext()
        {
        }

        public BookingDbContext(DbContextOptions<BookingDbContext> options)
              : base(options)
        {
        }

        public virtual DbSet<Booking> Bookings { get; set; }
    }
}
