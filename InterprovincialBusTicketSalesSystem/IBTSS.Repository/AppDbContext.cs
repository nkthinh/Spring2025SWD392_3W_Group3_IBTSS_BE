using IBTSS.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace IBTSS.Repository
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<LocationRoute> LocationRoutes { get; set; }
        public DbSet<Membership> Memberships { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Membership)
                .WithMany(m => m.Customers)
                .HasForeignKey(c => c.MembershipId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Bus)
                .WithMany(b => b.Seats)
                .HasForeignKey(s => s.BusId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Route)
                .WithMany(r => r.Trips)
                .HasForeignKey(t => t.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Bus)
                .WithMany(b => b.Trips)
                .HasForeignKey(t => t.BusId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Trip>()
                .Property(t => t.DepartureTime)
                .HasConversion(v => v.ToTimeSpan(), v => TimeOnly.FromTimeSpan(v));

            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Driver)
                .WithMany(u => u.Trips)
                .HasForeignKey(t => t.DriverId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Book)
                .WithMany(b => b.Tickets)
                .HasForeignKey(t => t.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Trip)
                .WithMany(tr => tr.Tickets)
                .HasForeignKey(t => t.TripId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Book>()
     .HasOne(b => b.Customer)
     .WithMany(c => c.Books) // ✅ sửa từ Tickets -> Books
     .HasForeignKey(b => b.CustomerId)
     .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Transaction)
                .WithMany(t => t.Books) // ✅ sửa từ Tickets -> Books
                .HasForeignKey(b => b.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Seat)
                .WithOne(s => s.Ticket)
                .HasForeignKey<Ticket>(t => t.SeatId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LocationRoute>()
                .HasOne(lr => lr.Route)
                .WithMany(r => r.LocationRoutes)
                .HasForeignKey(lr => lr.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LocationRoute>()
                .HasOne(lr => lr.Location)
                .WithMany()
                .HasForeignKey(lr => lr.LocationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

