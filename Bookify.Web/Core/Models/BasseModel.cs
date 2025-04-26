namespace Bookify.Web.Core.Models
{
    public class BasseModel
    {
        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public DateTime? LastUpdatedOn { get; set; }
    }
}
