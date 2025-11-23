using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using WebApplication1.Models;
//using WebApplication1.Models.ViewModels;

namespace WebApplication1.Controllers
{
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<MenuController> _logger;

        public MenuController(ApplicationDbContext context, IWebHostEnvironment env, ILogger<MenuController> logger)
        {
            db = context;
            _env = env;
            _logger = logger;
        }

        // GET: /Caterer/Index
        public IActionResult Index()
        {

            var catererIdNullable = HttpContext.Session.GetInt32("catererid");
            if (catererIdNullable == null)
            {
                return RedirectToAction("CatererLogin", "Caterer");
            }
            int catererId = catererIdNullable.Value;

            var menus = db.Menus.Where(c => c.CatererId == catererId).Include(c => c.Caterer).ToList();
            return View(menus);
        }


        // GET: /Caterer/AddCaterer
        public IActionResult AddMenu()
        {
            return View();
        }

        // POST: /Caterer/AddCaterer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddMenu(MenuBridgeModel menu)
        {
            if (ModelState.IsValid)
            {
                // Ensure the upload directory exists
                var folderPath = Path.Combine(_env.WebRootPath, "UploadImage");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Generate a unique filename
                var fileName = Guid.NewGuid() + "_" + menu.MenuImage.FileName;

                var filePath = Path.Combine(folderPath, fileName);

                // Save the uploaded files
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    menu.MenuImage.CopyTo(stream);
                }

                // Create a new Caterer instance
                var newMenu = new Menu
                {
                    MenuTitle = menu.MenuTitle,
                    MenuDescription = menu.MenuDescription,
                    MenuImage = fileName,
                    NumberOfPeople = menu.NumberOfPeople,
                    Price = menu.Price,
                    CatererId = menu.CatererId
                };

                // Add the new caterer to the database
                db.Menus.Add(newMenu);
                db.SaveChanges();

                //// Redirect to the Index action
                return RedirectToAction(nameof(Index));
            }

            // If model state is invalid, return the view with the current model
            return View(menu);
        }

        // GET: /Menu/Edit/5
        public IActionResult Edit(int id)
        {
            var menu = db.Menus.Find(id);
            if (menu == null)
            {
                _logger.LogWarning($"Menu with ID {id} not found.");
                return NotFound();
            }

            var menuEditModel = new MenuEditModel
            {
                Id = menu.Id,
                MenuTitle = menu.MenuTitle,
                MenuDescription = menu.MenuDescription,
                NumberOfPeople = menu.NumberOfPeople,
                Price = menu.Price,
                CatererId = menu.CatererId
            };

            ViewBag.ExistingMenuImage = menu.MenuImage; // Pass existing image path to the view
            return View(menuEditModel);
        }

        // POST: /Menu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, MenuEditModel menu)
        {
            if (id != menu.Id)
            {
                _logger.LogWarning($"ID mismatch: {id} does not match {menu.Id}.");
                return NotFound();
            }

            var existingMenu = db.Menus.Find(id);
            if (existingMenu == null)
            {
                _logger.LogWarning($"Menu with ID {id} not found for update.");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update fields
                    existingMenu.MenuTitle = menu.MenuTitle;
                    existingMenu.MenuDescription = menu.MenuDescription;
                    existingMenu.NumberOfPeople = menu.NumberOfPeople;
                    existingMenu.Price = menu.Price;
                    existingMenu.CatererId = menu.CatererId;

                    // Handle image upload
                    if (menu.MenuImage != null)
                    {
                        var folderPath = Path.Combine(_env.WebRootPath, "UploadImage");
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        // Delete old image if it exists
                        if (!string.IsNullOrEmpty(existingMenu.MenuImage))
                        {
                            var oldFilePath = Path.Combine(folderPath, existingMenu.MenuImage);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Save new image
                        var fileName = Guid.NewGuid() + Path.GetExtension(menu.MenuImage.FileName);
                        var filePath = Path.Combine(folderPath, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            menu.MenuImage.CopyTo(stream);
                        }

                        existingMenu.MenuImage = fileName; // Save new image name in DB
                    }

                    db.SaveChanges();
                    _logger.LogInformation($"Menu with ID {id} updated successfully.");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating menu with ID {id}.");
                    ModelState.AddModelError("", "An error occurred while updating the menu. Please try again.");
                }
            }

            // Preserve existing image in case of validation error
            ViewBag.ExistingMenuImage = existingMenu.MenuImage;
            return View(menu);
        }




        // GET: /Menu/Delete/5
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var menu = db.Menus.Find(id);
            if (menu == null)
            {
                return NotFound();
            }

            bool hasBookings = db.Bookings.Any(b => b.MenuId == id);

            if (hasBookings)
            {
                TempData["Error"] = "Cannot delete this menu because it has existing bookings.";
                return RedirectToAction("Index");
            }

            // Only show confirmation page if deletion is allowed
            return View(menu);
        }


        // POST: /Menu/DeleteConfirmed/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var menu = db.Menus.Find(id);
            if (menu == null)
            {
                return NotFound();
            }

            bool hasBookings = db.Bookings.Any(b => b.MenuId == id);

            if (hasBookings)
            {
                TempData["Error"] = "Cannot delete this menu because it has existing bookings.";
                return RedirectToAction("Index");
            }

            db.Menus.Remove(menu);
            db.SaveChanges();

            TempData["Success"] = "Menu deleted successfully.";
            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult GetCaterers()
        {
            var caterers = db.Caterers.Select(d => new
            {
                cId = d.Id,
                catererName = d.Name
            }).ToList();

            return Json(caterers);
        }

    }
}
