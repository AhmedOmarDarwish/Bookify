namespace Bookify.Web.Core.ViewModels
{
    public class CategoryViewModel : BaseModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
    }
}
