
using Bookify.Web.Core.Models;

namespace Bookify.Web.Controllers
{
    [Authorize(Roles = AppRoles.Archive)]
    public class GovernoratesController (ApplicationDbContext context, IMapper mapper) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public IActionResult Index()
        {
            var governorate = _context.Governorates.AsNoTracking().ToList();

            var viewModel = _mapper.Map<IEnumerable<GovernorateViewModel>>(governorate);

            return View(viewModel);
        }

        [HttpGet]
        [AjaxOnly]
        public IActionResult Create()
        {
            return PartialView("_Form");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(GovernorateFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var governorate = _mapper.Map<Governorate>(model);
            governorate.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _context.Add(governorate);
            _context.SaveChanges();

            var viewModel = _mapper.Map<GovernorateViewModel>(governorate);

            return PartialView("_GovernorateRow", viewModel);
        }

        [HttpGet]
        [AjaxOnly]
        public IActionResult Edit(int id)
        {
            var governorate = _context.Governorates.Find(id);

            if (governorate is null)
                return NotFound();

            var viewModel = _mapper.Map<GovernorateFormViewModel>(governorate);

            return PartialView("_Form", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(GovernorateFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var governorate = _context.Governorates.Find(model.Id);

            if (governorate is null)
                return NotFound();

            governorate = _mapper.Map(model, governorate);
            governorate.LastUpdatedOn = DateTime.Now;
            governorate.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            _context.SaveChanges();

            var viewModel = _mapper.Map<GovernorateViewModel>(governorate);

            return PartialView("_GovernorateRow", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            var governorate = _context.Governorates.Find(id);

            if (governorate is null)
                return NotFound();

            governorate.IsDeleted = !governorate.IsDeleted;
            governorate.LastUpdatedOn = DateTime.Now;
            governorate.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            _context.SaveChanges();

            return Ok(governorate.LastUpdatedOn.ToString());
        }

        public IActionResult AllowItem(GovernorateFormViewModel model)
        {
            var governorate = _context.Governorates.SingleOrDefault(c => c.Name == model.Name);
            var isAllowed = governorate is null || governorate.Id.Equals(model.Id);

            return Json(isAllowed);
        }
    }
}
