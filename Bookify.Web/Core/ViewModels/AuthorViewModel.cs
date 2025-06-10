namespace Bookify.Web.Core.ViewModels
{
    public class AuthorViewModel:BaseModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
    }
}
