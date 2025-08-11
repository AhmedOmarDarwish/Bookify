namespace Bookify.Domain.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Category : BaseEntity
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public ICollection<BookCategory> Books { get; set; } = new List<BookCategory>();


    }
}
