namespace Bookify.Web.Core.Models
{
    public record Category
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn {  get; set; } = DateTime.UtcNow;

        public DateTime? LastUpdatedOn { get; set; }
    }
}
