using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Hospital.Data;
using Hospital.Models.DTOs;

namespace Hospital.Controllers
{
    public class AccountController : Controller
    {
        private readonly HospitalDbContext _db = new HospitalDbContext();

        // GET: Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _db.Users.FirstOrDefault(x => x.EmployeeNumber == model.EmployeeNumber);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid Employee Number or Password");
                return View(model);
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                ModelState.AddModelError("", "Invalid Employee Number or Password");
                return View(model);
            }

            user.LastLoginAt = DateTime.Now;
            _db.SaveChanges();

            FormsAuthentication.SetAuthCookie(user.EmployeeNumber, model.RememberMe);
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View(new ForgotPasswordDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _db.Users.FirstOrDefault(x => x.EmployeeNumber == model.EmployeeNumber);
            if (user == null)
            {
                ModelState.AddModelError("", "Employee Number not found.");
                return View(model);
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            _db.SaveChanges();

            TempData["Success"] = "Password reset successfully. Please log in.";
            return RedirectToAction("Login");
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}