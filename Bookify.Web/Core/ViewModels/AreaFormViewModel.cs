namespace Bookify.Web.Core.ViewModels
{
    public class AreaFormViewModel
    {
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = Errors.MaxLength)]
        [Remote("AllowItem", null!, AdditionalFields = "Id,GovernorateId", ErrorMessage = Errors.DuplicatedArea)]
        public string Name { get; set; } = null!;

        [Display(Name = "Governorate")]
        [Remote("AllowItem", null!, AdditionalFields = "Id,Name", ErrorMessage = Errors.DuplicatedArea)]
        public int GovernorateId { get; set; }

        public IEnumerable<SelectListItem>? Governorates { get; set; }
    }
}
