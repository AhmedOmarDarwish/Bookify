
namespace Bookify.Web.Core.ViewModels
{
    public class CategoryFormViewModel
    {
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = Errors.MaxLength), Display(Name = "Category")]
        [Remote("AllowItem", null, AdditionalFields = nameof(Id), ErrorMessage = Errors.Duplicated)]
        public string Name { get; set; } = string.Empty;
    }
}
