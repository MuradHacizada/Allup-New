using Allup111.DAL;
using Allup111.Models;
using Allup111.Utilities.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Allup111.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public CategoryController(AppDbContext db, IWebHostEnvironment env)
        {

            _db = db;
            _env = env;
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
        [ValidateAntiForgeryToken]
        [HttpPost]
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
                #region Save Image
                if (category.Photo == null)
                {
                    ModelState.AddModelError("Photo", "Image can not be null");
                    return View();
                }
                if (!category.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "Pls select image type");
                    return View();
                }
                if (category.Photo.IsOlderMb())
                {
                    ModelState.AddModelError("Photo", "max 1 mb");
                    return View();
                }
                string folder = Path.Combine(_env.WebRootPath,"assets", "images");
                category.Image = await category.Photo.SaveFileAsync(folder);
                #endregion
            }
            else
            {
                category.ParentId = mainCatId;
            }
            await _db.Categories.AddAsync(category);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
