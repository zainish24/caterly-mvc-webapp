using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebApplication1.Migrations;
using WebApplication1.Models;

public class BookingController : Controller
{
    private readonly ApplicationDbContext db;

    public BookingController(ApplicationDbContext context)
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



        var bookings = db.Bookings
            .Where(b => b.CatererId == catererId)
            .Include(b => b.Category)
            .Include(b => b.Menu)
            .Include(b => b.Caterer)
            .Include(b => b.User)
            .ToList();
        return View(bookings);
    }

    // GET: View booking details
    public IActionResult Details(int id)
    {
        var booking = db.Bookings
            .Include(b => b.Caterer)
            .Include(b => b.User)
            .Include(b => b.Category)
            .Include(b => b.Menu)
            .FirstOrDefault(b => b.Id == id);

        if (booking == null)
        {
            return RedirectToAction("CatererLogin", "Caterer");
        }

        return View(booking);
    }

    // POST: Approve Booking
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Approve(int id)
    {
        var booking = db.Bookings.Find(id);
        if (booking != null)
        {
            booking.Status = "Approved";
            db.SaveChanges();
            TempData["Message"] = "Booking Approved!";
        }
        else
        {
            TempData["Error"] = "Booking not found!";
        }
        return RedirectToAction("Index");
    }
}
