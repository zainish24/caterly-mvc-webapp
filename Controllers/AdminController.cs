using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<CatererController> _logger;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment env, ILogger<CatererController> logger)
        {
            db = context;
            _env = env;
            _logger = logger;
        }


        public IActionResult Index()
        {
            var status = HttpContext.Session.GetString("Status");
            var adminId = HttpContext.Session.GetInt32("userid");

            if (status != "Admin" || adminId == null)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }



        public IActionResult Profile()
        {
            var userIdNullable = HttpContext.Session.GetInt32("userid");

            if (userIdNullable == null)
            {
                return RedirectToAction("Login", "Home");
            }

            int userId = userIdNullable.Value;

            var users = db.Users
                .Where(b => b.Id == userId)
                .ToList();
            return View(users);
        }

        public IActionResult EditProfile(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditProfileModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Contact = user.Contact,
                Gender = user.Gender,
                Address = user.Address
            };

            return View(model); // Ensure this is EditProfileModel
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(EditProfileModel model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by ID
                var user = db.Users.Find(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                // Update user properties with values from the model
                user.Name = model.Name;
                user.Email = model.Email;
                user.Contact = model.Contact;
                user.Gender = model.Gender;
                user.Address = model.Address;

                // Save changes to the database
                db.Users.Update(user); // This line is not necessary if you are using the same instance
                db.SaveChanges();

                // Redirect to the Profile action
                return RedirectToAction(nameof(Profile));
            }

            // If the model state is not valid, return the same view with the model
            return View(model);
        }


        public IActionResult ResetPassword(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ResetPasswordModel { Id = user.Id };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                var passhasher = new PasswordHasher<UserModel>();
                user.Password = passhasher.HashPassword(user, model.NewPassword);
                db.Users.Update(user);
                db.SaveChanges();
                return RedirectToAction(nameof(Profile));
            }

            return View(model);
        }


        public IActionResult Logout()
        {
            // Clear the session
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home"); 
        }


        public IActionResult Booking()
        {
            var status = HttpContext.Session.GetString("Status");
            var adminId = HttpContext.Session.GetInt32("userid");

            if (status != "Admin" || adminId == null)
            {
                return RedirectToAction("Login", "Home");
            }


            var bookings = db.Bookings
                .Include(b => b.Category)
                .Include(b => b.Menu)
                .Include(b => b.Caterer)
                .Include(b => b.User)
                .ToList();
            return View(bookings);
        }

        public IActionResult Caterer()
        {

            var status = HttpContext.Session.GetString("Status");
            var adminId = HttpContext.Session.GetInt32("userid");

            if (status != "Admin" || adminId == null)
            {
                return RedirectToAction("Login", "Home");
            }


            var caterers = db.Caterers
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




        // GET: /VenuCategory/
        public IActionResult Category()
        {
            var status = HttpContext.Session.GetString("Status");
            var adminId = HttpContext.Session.GetInt32("userid");

            if (status != "Admin" || adminId == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var categories = db.CatererCategories
                .ToList();
            return View(categories);
        }

        // GET: /VenuCategory/Create
        public IActionResult AddCategory() => View();

        // POST: /VenuCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCategory(CatererCategory model)
        {
            if (ModelState.IsValid)
            {
                db.CatererCategories.Add(model);
                db.SaveChanges();
                return RedirectToAction("Category", "Admin");
            }
            return View(model);
        }

        // GET: /VenuCategory/Edit/5
        public IActionResult EditCategory(int id)
        {
            var category = db.CatererCategories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: /VenuCategory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCategory(CatererCategory model)
        {
            if (ModelState.IsValid)
            {
                db.CatererCategories.Update(model);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult DeleteCategory(int id)
        {
            var category = db.CatererCategories.Find(id);
            if (category == null)
            {
                return NotFound(); // Return a 404 if the menu is not found
            }
            return View(category); // Return the view with the menu model
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var category = db.CatererCategories.Find(id);
            if (category != null)
            {
                db.CatererCategories.Remove(category);
                db.SaveChanges();
                return RedirectToAction("Category", "Admin"); // Redirect to the index action after deletion
            }
            return NotFound(); // Return a 404 if the menu is not found
        }



        public IActionResult Menu()
        {
            var status = HttpContext.Session.GetString("Status");
            var adminId = HttpContext.Session.GetInt32("userid");

            if (status != "Admin" || adminId == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var menus = db.Menus.Include(c => c.Caterer).ToList();
            return View(menus);
        }



        public IActionResult Message()
        {
            var status = HttpContext.Session.GetString("Status");
            var adminId = HttpContext.Session.GetInt32("userid");

            if (status != "Admin" || adminId == null)
            {
                return RedirectToAction("Login", "Home ");
            }
            


            var message = db.Messages
                .Include(b => b.Category)
                .Include(b => b.Caterer)
                .Include(b => b.User)
                .ToList();
            return View(message);
        }

    }
}
