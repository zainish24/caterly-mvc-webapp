using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class CatererController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<CatererController> _logger;

        public CatererController(ApplicationDbContext context, IWebHostEnvironment env, ILogger<CatererController> logger)
        {
            db = context;
            _env = env;
            _logger = logger;
        }


        public IActionResult Home()
        {
            var catererIdNullable = HttpContext.Session.GetInt32("catererid");
            if (catererIdNullable == null)
            {
                return RedirectToAction("CatererLogin", "Caterer");
            }
            int catererId = catererIdNullable.Value;
            return View();
        }




        //GET: /Caterer/Index
        public IActionResult Index()
        {
            var catererIdNullable = HttpContext.Session.GetInt32("catererid");
            if (catererIdNullable == null)
            {
                return RedirectToAction("CatererLogin", "Caterer");
            }
            int catererId = catererIdNullable.Value;


            var caterers = db.Caterers
                .Where(b => b.Id == catererId)
                .Include(c => c.Category)
                .ToList();
            return View(caterers);
        }


        // GET: /Caterer/AddCaterer
        public IActionResult AddCaterer()
        {
            return View();
        }

        // POST: /Caterer/AddCaterer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCaterer(CatererBridgeModel caterer)
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
                var fileName = Guid.NewGuid() + "_" + caterer.CatererImage.FileName;

                var filePath = Path.Combine(folderPath, fileName);

                // Save the uploaded files
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    caterer.CatererImage.CopyTo(stream);
                }

                // Create a new Caterer instance
                var newCaterer = new Caterer
                {
                    Name = caterer.Name,
                    Description = caterer.Description,
                    Email = caterer.Email,
                    Phone = caterer.Phone,
                    Location = caterer.Location,
                    CatererImage = fileName,
                    CategoryId = caterer.CategoryId
                };

                // Add the new caterer to the database
                db.Caterers.Add(newCaterer);
                db.SaveChanges();

                // Store the caterer ID in the session
                HttpContext.Session.SetInt32("catererid", newCaterer.Id);

                // Redirect to the Index action
                return RedirectToAction(nameof(Index));
            }

            // If model state is invalid, return the view with the current model
            return View(caterer);
        }

        // GET: /Caterer/Edit/5
        public IActionResult Edit(int id)
        {
            var caterer = db.Caterers.Find(id);
            if (caterer == null)
            {
                _logger.LogWarning($"Caterer with ID {id} not found.");
                return NotFound();
            }

            var catererEditModel = new CatererEditModel
            {
                Id = caterer.Id,
                Name = caterer.Name,
                Email = caterer.Email,
                Phone = caterer.Phone,
                Location = caterer.Location,
                Description = caterer.Description,
                CategoryId = caterer.CategoryId
            };

            ViewBag.ExistingCatererImage = caterer.CatererImage; // Pass existing image path to the view
            return View(catererEditModel);
        }

        // POST: /Caterer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CatererEditModel caterer)
        {
            if (id != caterer.Id)
            {
                _logger.LogWarning($"ID mismatch: {id} does not match {caterer.Id}.");
                return NotFound();
            }

            var existingCaterer = db.Caterers.Find(id);
            if (existingCaterer == null)
            {
                _logger.LogWarning($"Caterer with ID {id} not found for update.");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update fields
                    existingCaterer.Name = caterer.Name;
                    existingCaterer.Email = caterer.Email;
                    existingCaterer.Phone = caterer.Phone;
                    existingCaterer.Description = caterer.Description;
                    existingCaterer.Location = caterer.Location;
                    existingCaterer.CategoryId = caterer.CategoryId;

                    // Handle image upload
                    if (caterer.CatererImage != null)
                    {
                        var folderPath = Path.Combine(_env.WebRootPath, "UploadImage");
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        // Delete old image if it exists
                        if (!string.IsNullOrEmpty(existingCaterer.CatererImage))
                        {
                            var oldFilePath = Path.Combine(folderPath, existingCaterer.CatererImage);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Save new image
                        var fileName = Guid.NewGuid() + Path.GetExtension(caterer.CatererImage.FileName);
                        var filePath = Path.Combine(folderPath, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            caterer.CatererImage.CopyTo(stream);
                        }

                        existingCaterer.CatererImage = fileName; // Save new image name in DB
                    }

                    db.SaveChanges();
                    _logger.LogInformation($"Caterer with ID {id} updated successfully.");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating caterer with ID {id}.");
                    ModelState.AddModelError("", "An error occurred while updating the caterer. Please try again.");
                }
            }

            // Preserve existing image in case of validation error
            ViewBag.ExistingCatererImage = existingCaterer.CatererImage;
            return View(caterer);
        }






        // Action to get categories for the dropdown
        [HttpPost]
        public IActionResult GetCategories()
        {
            var categories = db.CatererCategories.Select(c => new
            {
                cId = c.Id,
                categoryName = c.Name
            }).ToList();

            return Json(categories);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var caterer = db.Caterers.Find(id);
            if (caterer == null)
            {
                return NotFound();
            }
            return View(caterer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var caterer = db.Caterers.Find(id);
            if (caterer != null)
            {
                db.Caterers.Remove(caterer);
                db.SaveChanges();
                return RedirectToAction("Index", "Caterer"); // Redirect to the index action after deletion
            }
            return NotFound(); // Return a 404 if the menu is not found
        }




        // GET: Login
        public IActionResult CatererLogin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CatererLogin(Caterer user)
        {
            var users = db.Caterers.FirstOrDefault(u => u.Email == user.Email);
            if (users != null)
            {

                HttpContext.Session.SetInt32("catererid", users.Id);
                return RedirectToAction("Home", "Caterer");
            }
            else
            {
                ModelState.AddModelError("Email", "Invalid Email Address");
                return View(user);
            }
        }



        public IActionResult CatererLogout()
        {
            // Clear the session
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home"); // Redirect to the home page or login page
        }

    }
}

