using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Migrations;
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Controllers
{
    public class MessageController : Controller
    {
        private readonly ApplicationDbContext db;

        public MessageController(ApplicationDbContext context)
        {
            db = context;
        }

        // GET: Show all bookings
        public IActionResult Index()
        {
            var catererIdNullable = HttpContext.Session.GetInt32("catererid");
            if (catererIdNullable == null)
            {
                return RedirectToAction("CatererLogin", "Caterer");
            }
            int catererId = catererIdNullable.Value;


            var message = db.Messages
                .Where(b => b.CatererId == catererId )
                .Include(b => b.Category)
                .Include(b => b.Caterer)
                .Include(b => b.User)
                .ToList();
            return View(message);
        }



        // GET: View booking details
        public IActionResult Details(int id)
        {
            var message = db.Messages
                .Include(m => m.Caterer)
                .Include(m => m.User)
                .Include(m => m.Category)
                .FirstOrDefault(m => m.Id == id);

            if (message == null)
            {
                return RedirectToAction("CatererLogin", "Caterer");
            }

            var viewModel = new DataViewModel
            {
                Message = message,
                Caterers = db.Caterers.ToList(),
                CatererCategories = db.CatererCategories.ToList(),
                Menus = db.Menus.ToList(),
                Messages = db.Messages
                    .Where(m => m.CatererId == message.CatererId)
                    .ToList()
            };

            return View(viewModel);
        }

        // Message POST Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details(Message message)
        {
            var userIdNullable = HttpContext.Session.GetInt32("catererid");
            if (userIdNullable == null)
            {
                return RedirectToAction("CatererLogin", "Caterer");
            }

            message.UserId = message.UserId;
            message.SenderType = "Caterer"; 
            message.CatererId = message.CatererId; 
            message.CategoryId = message.CategoryId;

            db.Messages.Add(message);
            db.SaveChanges();
            return RedirectToAction("Index", "Message");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(int id)
        {
            var message = db.Messages.Find(id);
            if (message == null)
            {
                TempData["Error"] = "Message not found.";
                return RedirectToAction("Index");
            }

            db.Messages.Remove(message);
            db.SaveChanges();

            TempData["Success"] = "Message deleted successfully.";
            return RedirectToAction("Index");
        }


    }
}