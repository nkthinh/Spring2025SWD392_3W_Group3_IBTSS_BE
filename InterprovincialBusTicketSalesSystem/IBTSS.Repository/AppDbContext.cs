using IBTSS.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<LocationRoute> LocationRoutes { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(GetConnectionString());
        //}

        //private string GetConnectionString()
        //{
        //    IConfiguration configuration = new ConfigurationBuilder()
        //            .SetBasePath(Directory.GetCurrentDirectory())
        //            .AddJsonFile("appsettings.json", true, true).Build();
        //    return configuration["ConnectionStrings:DefaultConnectionString"];
        //}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Customer - Membership (N-1)
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Membership)
                .WithMany(m => m.Customers)
                .HasForeignKey(c => c.MembershipId)
                .OnDelete(DeleteBehavior.SetNull);

            // Bus - Seat (1-N)
            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Bus)
                .WithMany(b => b.Seats)
                .HasForeignKey(s => s.BusId)
                .OnDelete(DeleteBehavior.Cascade);

            // Route - Trip (1-N)
            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Route)
                .WithMany(r => r.Trips)
                .HasForeignKey(t => t.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Trip - Bus (N-1)
            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Bus)
                .WithMany(b => b.Trips)
                .HasForeignKey(t => t.BusId)
                .OnDelete(DeleteBehavior.Cascade);

            // Trip - Driver (User) (N-1)
            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Driver)
                .WithMany(u => u.Trips)
                .HasForeignKey(t => t.DriverId)
                .OnDelete(DeleteBehavior.Cascade);

            // Trip - Ticket (1-N)
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Trip)
                .WithMany(tr => tr.Tickets)
                .HasForeignKey(t => t.TripId)
                .OnDelete(DeleteBehavior.Restrict);

            // Customer - Ticket (1-N)
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Customer)
                .WithMany(c => c.Tickets)
                .HasForeignKey(t => t.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ticket - Transaction (1-N)
            modelBuilder.Entity<Transaction>()
                .HasOne(tr => tr.Ticket)
                .WithMany(t => t.Transactions)
                .HasForeignKey(tr => tr.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // Route - LocationRoute (1-N)
            modelBuilder.Entity<LocationRoute>()
                .HasOne(lr => lr.Route)
                .WithMany(r => r.LocationRoutes)
                .HasForeignKey(lr => lr.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Location - LocationRoute (1-N)
            modelBuilder.Entity<LocationRoute>()
                .HasOne(lr => lr.Location)
                .WithMany()
                .HasForeignKey(lr => lr.LocationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seat - Ticket (1-1)
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Seat)
                .WithOne(s => s.Ticket)
                .HasForeignKey<Ticket>(t => t.SeatId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
