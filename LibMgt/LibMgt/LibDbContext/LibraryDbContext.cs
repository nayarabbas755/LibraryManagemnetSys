using LibMgt.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace LibMgt.LibDbContext
{


    public class LibraryDbContext : IdentityDbContext<User,Role,Guid>
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options)
        { }


        public DbSet<User> User { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Patron> Patrons { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Fine> Fines { get; set; }
        public DbSet<Genre> Genres { get; set; }
    }
}
