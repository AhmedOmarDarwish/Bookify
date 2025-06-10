namespace Bookify.Web.Core.ViewModels
{
    public class AreaViewModel : BaseModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Governorate { get; set; } = null!;
    }
}
