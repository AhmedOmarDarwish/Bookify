namespace Bookify.Web.Controllers
{
    public class CategoriesController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        public IActionResult Index()
        {
            //TODO: use viewModel
            var categories = _context.Categories.ToList();
            return View(categories);
        }
    }
}
