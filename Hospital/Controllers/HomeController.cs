using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Hospital.Data;

namespace Hospital.Controllers
{
    public class HomeController : Controller
    {
        private readonly HospitalDbContext _db = new HospitalDbContext();

        public ActionResult Index(DateTime? date)
        {
            var selectedDate = (date ?? System.DateTime.Today).Date;

            ViewBag.ScheduledList = _db.ScheduledDoctors
                                        .Include("Doctor")
                                        .Where(s => DbFunctions.TruncateTime(s.ScheduledDate) == selectedDate)
                                        .OrderBy(s => s.Doctor.Name)
                                        .ToList();

            ViewBag.VisitingList = _db.VisitingDoctors
                                        .OrderByDescending(v => v.Id)
                                        .ToList();

            ViewBag.TelemedicineList = _db.DoctorsTelemedicine
                                            .OrderByDescending(t => t.Id)
                                            .ToList();

            ViewBag.ShiftList = _db.DoctorsInShift
                                    .Include("Doctor")
                                    .OrderByDescending(s => s.Id)
                                    .ToList();

            // Leave records active on the selected date (FromDate <= selectedDate <= ToDate)
            ViewBag.LeaveList = _db.DoctorsOnLeave
                                    .Include("Doctor")
                                    .Where(l => DbFunctions.TruncateTime(l.FromDate) <= selectedDate
                                             && DbFunctions.TruncateTime(l.ToDate) >= selectedDate)
                                    .OrderBy(l => l.Doctor.Name)
                                    .ToList();

            ViewBag.SelectedDate = selectedDate;
            ViewBag.PageDate = selectedDate.ToString("dd-MMM-yyyy");
            ViewBag.PrevDate = selectedDate.AddDays(-1).ToString("yyyy-MM-dd");
            ViewBag.NextDate = selectedDate.AddDays(1).ToString("yyyy-MM-dd");
            ViewBag.DateValue = selectedDate.ToString("yyyy-MM-dd");
            ViewBag.IsToday = selectedDate == System.DateTime.Today;
            ViewBag.LastUpdated = System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}