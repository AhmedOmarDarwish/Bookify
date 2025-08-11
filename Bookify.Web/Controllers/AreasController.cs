namespace Bookify.Web.Controllers
{
    [Authorize(Roles = AppRoles.Archive)]
    public class AreasController(IApplicationDbContext context, IMapper mapper) : Controller
    {
        private readonly IApplicationDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public IActionResult Index()
        {
            var areas = _context.Areas
                .Include(g => g.Governorate).ToList();

            var viewModel = _mapper.Map<IEnumerable<AreaViewModel>>(areas);

            return View(viewModel);
        }

        [HttpGet]
        [AjaxOnly]
        public IActionResult Create()
        {

            return PartialView("_Form", PopulateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AreaFormViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_Form", PopulateViewModel());

            var area = _mapper.Map<Area>(model);
            area.CreatedById = User.GetUserId();
            _context.Areas.Add(area);
            _context.SaveChanges();

            _context.Areas.Entry(area).Reference(a => a.Governorate).Load();

            var viewModel = _mapper.Map<AreaViewModel>(area);

            return PartialView("_AreaRow", viewModel);
        }

        [HttpGet]
        [AjaxOnly]
        public IActionResult Edit(int id)
        {
            var area = _context.Areas.Include(g => g.Governorate).SingleOrDefault(a => a.Id == id);

            if (area is null)
                return NotFound();

            var model = _mapper.Map<AreaFormViewModel>(area);
            var viewModel = PopulateViewModel(model);

            return PartialView("_Form", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(AreaFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var area = _context.Areas.Include(g => g.Governorate).SingleOrDefault(a => a.Id == model.Id);

            if (area is null)
                return NotFound();

            area = _mapper.Map(model, area);
            area.LastUpdatedOn = DateTime.Now;
            area.LastUpdatedById = User.GetUserId();

            _context.SaveChanges();
            _context.Areas.Entry(area).Reference(a => a.Governorate).Load();

            var viewModel = _mapper.Map<AreaViewModel>(area);

            return PartialView("_AreaRow", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            var area = _context.Categories.Find(id);

            if (area is null)
                return NotFound();

            area.IsDeleted = !area.IsDeleted;
            area.LastUpdatedOn = DateTime.Now;
            area.LastUpdatedById = User.GetUserId();

            _context.SaveChanges();

            return Ok(area.LastUpdatedOn.ToString());
        }

        public IActionResult AllowItem(AreaFormViewModel model)
        {
            var area = _context.Areas.SingleOrDefault(b => b.Name == model.Name && b.GovernorateId == model.GovernorateId);
            var isAllowed = area is null || area.Id.Equals(model.Id);

            return Json(isAllowed);
        }

        private AreaFormViewModel PopulateViewModel(AreaFormViewModel? model = null)
        {
            AreaFormViewModel viewModel = model is null ? new AreaFormViewModel() : model;

            var governorate = _context.Governorates.Where(a => !a.IsDeleted).OrderBy(a => a.Name).ToList();

            viewModel.Governorates = _mapper.Map<IEnumerable<SelectListItem>>(governorate);

            return viewModel;
        }
    }
}
