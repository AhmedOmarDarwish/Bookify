namespace Bookify.Web.Core.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Author : BasseModel
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = null!;
    }
}
