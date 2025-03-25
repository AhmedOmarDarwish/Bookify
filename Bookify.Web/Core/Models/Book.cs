using System.ComponentModel.DataAnnotations.Schema;

namespace Bookify.Web.Core.Models
{
    [Index(nameof(Title), nameof(AuthorId), IsUnique = true)]

    public class Book : BasseModel
    {
        public int ID { get; set; }

        [MaxLength(100)]
        public string Title { get; set; } = null!;

        [MaxLength(200)]
        public string Description { get; set; } = null!;

        [MaxLength(200)]
        public string Publisher { get; set; } = null!;

        public DateTime PublishingDate { get; set; }

        public string? ImageUrl { get; set; }

        [MaxLength(50)]
        public string Hall { get; set; } = null!;

        public bool IsAvailableForRental { get; set; }

     
        public int AuthorId { get; set; }
        public Author? Author { get; set; }

        public ICollection<BookCategory> Categories { get; set; } = new List<BookCategory>();
    }
}
