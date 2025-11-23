using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db;

        public HomeController(ApplicationDbContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
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

            return RedirectToAction("Index", "Home"); // Redirect to the home page or login page
        }

        // GET: Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        public IActionResult Register(UserModel user)
        {
            if (ModelState.IsValid)
            {
                var data = db.Users.FirstOrDefault(a => a.Email == user.Email);
                if (data == null)
                {
                    if (user.Password == user.ConfirmPassword)
                    {
                        var passhasher = new PasswordHasher<UserModel>();
                        var password = passhasher.HashPassword(user, user.Password);

                        UserModel finaldata = new UserModel()
                        {
                            Name = user.Name,
                            Email = user.Email,
                            Password = password,
                            ConfirmPassword = password,
                            Contact = user.Contact,
                            Gender = user.Gender,
                            Address = user.Address,
                            Status = "User"
                        };

                        db.Users.Add(finaldata);
                        db.SaveChanges();

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Password not match");
                        return View(user);
                    }
                }
                else
                {
                    ModelState.AddModelError("Email", "Email already taken");
                    return View(user);
                }
            }
            return View();
        }

        // GET: Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(UserModel user)
        {
            var users = db.Users.FirstOrDefault(u => u.Email == user.Email);
            if (users != null)
            {
                var passhasher = new PasswordHasher<UserModel>();
                var result = passhasher.VerifyHashedPassword(user, users.Password, user.Password);
                if (result == PasswordVerificationResult.Success)
                {
                    HttpContext.Session.SetString("Status", users.Status);
                    HttpContext.Session.SetInt32("userid", users.Id);

                    // Check user status and redirect accordingly
                    if (users.Status == "Admin")
                    {
                        return RedirectToAction("Index", "Admin"); // Redirect to Admin Index
                    }
                    else if (users.Status == "User ")
                    {
                        return RedirectToAction("Index", "Home"); // Redirect to User Index
                    }
                    else
                    {
                        // Handle other statuses if necessary
                        return RedirectToAction("Index", "Home"); // Default redirect
                    }
                }
                else
                {
                    ModelState.AddModelError("Password", "Invalid Password");
                    return View(user);
                }
            }
            else
            {
                ModelState.AddModelError("Email", "Invalid Email Address");
                return View(user);
            }
        }




        // Caterer Page
        public IActionResult Caterer()
        {
            var userIdNullable = HttpContext.Session.GetInt32("userid");
            if (userIdNullable == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var catererCategories = db.CatererCategories.ToList();
            var caterers = db.Caterers.ToList();

            var model = new DataViewModel
            {
                CatererCategories = catererCategories,
                Caterers = caterers
            };

            return View(model);
        }

        // AddBooking
        public IActionResult AddBooking(int id)
        {
            var userIdNullable = HttpContext.Session.GetInt32("userid");
            if (userIdNullable == null)
            {
                return RedirectToAction("Login", "Home");
            }
            int userId = userIdNullable.Value;

            // Find the Caterer by ID
            var caterer = db.Caterers.FirstOrDefault(z => z.Id == id);
            if (caterer == null)
            {
                return NotFound();
            }

            // Fetch the category using the CategoryId from the Caterer
            var category = db.CatererCategories.FirstOrDefault(y => y.Id == caterer.CategoryId);
            if (category == null)
            {
                return NotFound();
            }

            // Fetch all menus related to this caterer
            var menus = db.Menus.Where(m => m.CatererId == caterer.Id).ToList();

            var viewModel = new DataViewModel
            {
                Booking = new Booking
                {
                    CatererId = caterer.Id,
                    CategoryId = category.Id, // Now correctly assigned
                    UserId = userId
                },
                Caterers = new List<Caterer> { caterer },
                CatererCategories = new List<CatererCategory> { category },
                Menus = menus
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddBooking(Booking booking)
        {
            var userIdNullable = HttpContext.Session.GetInt32("userid");
            if (userIdNullable == null)
            {
                return RedirectToAction("Login", "Home");
            }

            booking.UserId = userIdNullable.Value; // Assign logged-in user ID
            booking.Status = "Pending"; // Set default status

            db.Bookings.Add(booking);
            db.SaveChanges();
            return RedirectToAction("ViewBooking");
        }

        // View Booking List

        public IActionResult ViewBooking()
        {
            var userIdNullable = HttpContext.Session.GetInt32("userid");

            if (userIdNullable == null)
            {
                return RedirectToAction("Login", "Home");
            }

            int userId = userIdNullable.Value;

            // Filter bookings by the logged-in user's UserId
            var bookings = db.Bookings
                .Where(b => b.UserId == userId)  // Only show the logged-in user's bookings
                .Include(b => b.Category)
                .Include(b => b.Menu)
                .Include(b => b.Caterer)
                .Include(b => b.User)
                .ToList();

            return View(bookings);
        }


        // GET: View booking details
        public IActionResult BookingDetails(int id)
        {
            var userIdNullable = HttpContext.Session.GetInt32("userid");

            if (userIdNullable == null)
            {
                return RedirectToAction("Login", "Home");
            }

            int userId = userIdNullable.Value;

            // Fetch the booking that matches both the ID and the logged-in user's UserId
            var booking = db.Bookings
                .Include(b => b.Caterer)
                .Include(b => b.User)
                .Include(b => b.Category)
                .Include(b => b.Menu)
                .FirstOrDefault(b => b.Id == id && b.UserId == userId); // Ensure booking belongs to the logged-in user

            if (booking == null)
            {
                return NotFound(); // If the booking is not found or does not belong to the user, return NotFound
            }

            return View(booking);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(int id)
        {
            var userIdNullable = HttpContext.Session.GetInt32("userid");

            if (userIdNullable == null)
            {
                return RedirectToAction("Login", "Home");
            }

            int userId = userIdNullable.Value;

            // Find the booking by its ID and check if it belongs to the logged-in user
            var booking = db.Bookings.FirstOrDefault(b => b.Id == id && b.UserId == userId);

            if (booking == null)
            {
                TempData["Error"] = "Booking not found or you do not have permission to cancel it!";
                return RedirectToAction("ViewBooking");
            }

            // Logic to check if a payment deduction is involved (this can be customized based on your rules)
            bool isPaymentDeducted = booking.Status == "Confirmed"; // Example condition: only if the booking is confirmed, payment is deducted

            if (isPaymentDeducted)
            {
                // Add a message to alert the user about the payment deduction
                TempData["Warning"] = "Warning: Canceling this booking will result in a payment deduction.";
            }

            // Remove the booking from the database
            db.Bookings.Remove(booking);
            db.SaveChanges();
            TempData["Message"] = "Booking Cancelled!";

            return RedirectToAction("ViewBooking");
        }


        // Message GET Action
        public IActionResult Message()
        {
            var userIdNullable = HttpContext.Session.GetInt32("userid");
            if (userIdNullable == null)
            {
                return RedirectToAction("Login", "Home");
            }
            int userId = userIdNullable.Value;

            var messages = db.Messages
                .Where(b => b.UserId == userId)
                .Include(b => b.Category)
                .Include(b => b.Caterer)
                .Include(b => b.User)
                .ToList();

            var viewModel = new DataViewModel
            {
                Messages = messages, // Correctly assign to Messages property
                Caterers = db.Caterers.ToList(),
                CatererCategories = db.CatererCategories.ToList(),
                Menus = db.Menus.ToList(),
            };

            return View(viewModel);
        }


        // Message POST Action
        [HttpPost]
       
        [ValidateAntiForgeryToken]
        public IActionResult Message(Message message)
        {
            var userIdNullable = HttpContext.Session.GetInt32("userid");
            if (userIdNullable == null)
            {
                return RedirectToAction("Login", "Home");
            }

            message.UserId = userIdNullable.Value;
            message.SenderType = "User ";

            db.Messages.Add(message);
            db.SaveChanges();
            return RedirectToAction("ViewMessages");
        }


        // GetCaterersByCategory
        [HttpPost]
        public IActionResult GetCaterersByCategory(int categoryId)
        {
            var caterers = db.Caterers
                .Where(e => e.CategoryId == categoryId)
                .Select(e => new { eId = e.Id, catererbycategoryName = e.Name })
                .ToList();

            return Json(caterers);
        }


        // Messages
        public IActionResult ViewMessages()
        {
            var userIdNullable = HttpContext.Session.GetInt32("userid");

            if (userIdNullable == null)
            {
                return RedirectToAction("Login", "Home");
            }

            int userId = userIdNullable.Value;

            // Filter bookings by the logged-in user's UserId
            var message = db.Messages
                .Where(b => b.UserId == userId)  // Only show the logged-in user's bookings
                .Include(b => b.Category)
                .Include(b => b.Caterer)
                .Include(b => b.User)
                .ToList();

            return View(message);
        }


        // GET: View booking details
        public IActionResult MessageDetails(int id)
        {
            var userIdNullable = HttpContext.Session.GetInt32("userid");

            if (userIdNullable == null)
            {
                return RedirectToAction("Login", "Home");
            }

            int userId = userIdNullable.Value;

            // Fetch the message and ensure it belongs to the logged-in user
            var message = db.Messages
                .Include(m => m.Caterer)
                .Include(m => m.User)
                .Include(m => m.Category)
                .FirstOrDefault(b => b.Id == id && b.UserId == userId); // Check if the message belongs to the user

            if (message == null)
            {
                return NotFound(); // Return NotFound if the message does not exist or does not belong to the user
            }

            return View(message);
        }


       
        // Post: /booking/Delete/5
        [HttpPost]
        public IActionResult DeleteBooking(int id)
        {
            var booking = db.Bookings.Find(id);
            if (booking != null)
            {
                booking.Status = "Cancelled";
                db.SaveChanges();
                TempData["Message"] = "Booking Cancelled!";
            }
            else
            {
                TempData["Error"] = "Booking not found!";
            }
            return RedirectToAction("ViewBooking", "Home"); // Return the view with the booking model
        }

    }
}