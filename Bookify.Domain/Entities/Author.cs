namespace Bookify.Domain.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Author : BaseEntity
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = null!;
    }
}
