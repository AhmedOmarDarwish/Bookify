namespace Bookify.Web.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Author> Authors { get; set; } = default!;

        public DbSet<Book> Books { get; set; } = default!;

        public DbSet<BookCategory> BookCategories { get; set; } = default!;

        public DbSet<Category> Categories { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<BookCategory>().HasKey(e => new { e.BookId, e.CategoryId });
            base.OnModelCreating(builder);
        }
    }
}
