using Allup111.DAL;
using Allup111.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Allup111.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _db;
        public CategoryController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            List<Category> categories = await _db.Categories.Include(c=>c.Children).Include(c=>c.Parent).ToListAsync();
            return View(categories);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.MainCategories = await _db.Categories.Where(x => x.IsMain).ToListAsync();
            return View();
        }
        public async Task<IActionResult> Create(Category category ,int? mainCatId)
        {
            ViewBag.MainCategories = await _db.Categories.Where(x => x.IsMain).ToListAsync();
            if (category.IsMain)
            {
                bool isExits = await _db.Categories.AnyAsync(x => x.Name == category.Name);
                if(isExits)
                {
                    ModelState.AddModelError("Name", "This Category is already exist");
                    return View();
                }
            }
            else
            {

            }
            return RedirectToAction("Index");
        }
    }
}
