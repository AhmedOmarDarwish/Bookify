namespace Bookify.Web.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Author> Authors { get; set; } = default!;

        public DbSet<Book> Books { get; set; } = default!;

        public DbSet<BookCategory> BookCategories { get; set; } = default!;

        public DbSet<BookCopy> BookCopies { get; set; } = default!;

        public DbSet<Category> Categories { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasSequence<int>("SerialNumber", schema: "shared")
                .StartsAt(1000001);

            builder.Entity<BookCopy>()
                .Property(e => e.SerialNumber)
                .HasDefaultValueSql("NEXT VALUE FOR shared.SerialNumber");

            builder.Entity<BookCategory>().HasKey(e => new { e.BookId, e.CategoryId });
            base.OnModelCreating(builder);
        }
    }
}
