using System.Linq;
using System.Web.Mvc;
using Hospital.Data;
using Hospital.Models;
using Hospital.Models.DTOs;

namespace Hospital.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly HospitalDbContext _db = new HospitalDbContext();

        public ActionResult Index()
        {
            ViewBag.Title = "Doctor's Availability";

            var today = System.DateTime.Today;

            ViewBag.ScheduledList = _db.ScheduledDoctors
                                        .Include("Doctor")
                                        .Where(s => s.ScheduledDate == today)
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

            ViewBag.LeaveList = _db.DoctorsOnLeave
                                    .Include("Doctor")
                                    .Where(l => l.ToDate >= today)
                                    .OrderByDescending(l => l.FromDate)
                                    .ToList();

            return View();
        }

        [HttpGet]
        public ActionResult AddDoctor()
        {
            ViewBag.Title = "Add Doctor";
            ViewBag.DoctorList = _db.Doctors.OrderByDescending(d => d.Id).ToList();
            return View(new AddDoctorDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDoctor(AddDoctorDto model)
        {
            ViewBag.Title = "Add Doctor";

            if (!ModelState.IsValid)
            {
                ViewBag.DoctorList = _db.Doctors.OrderByDescending(d => d.Id).ToList();
                return View(model);
            }

            var doctor = new Doctor
            {
                Name = model.Name,
                Specialist = model.Specialist
            };

            _db.Doctors.Add(doctor);
            _db.SaveChanges();

            TempData["Success"] = "Doctor added successfully.";
            return RedirectToAction("AddDoctor");
        }

        [HttpGet]
        public ActionResult ScheduledDoctor()
        {
            ViewBag.Title = "Scheduled Doctor";
            var dto = new ScheduledDoctorDto
            {
                ScheduledDate = System.DateTime.Today,
                DoctorList = new SelectList(_db.Doctors.OrderBy(d => d.Name).ToList(), "Id", "Name")
            };
            ViewBag.ScheduledList = _db.ScheduledDoctors
                                        .Include("Doctor")
                                        .OrderByDescending(s => s.ScheduledDate)
                                        .ToList();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ScheduledDoctor(ScheduledDoctorDto model)
        {
            ViewBag.Title = "Scheduled Doctor";
            if (!ModelState.IsValid)
            {
                model.DoctorList = new SelectList(_db.Doctors.OrderBy(d => d.Name).ToList(), "Id", "Name", model.DoctorId);
                ViewBag.ScheduledList = _db.ScheduledDoctors.Include("Doctor").OrderByDescending(s => s.ScheduledDate).ToList();
                return View(model);
            }
            var entity = new ScheduledDoctor
            {
                DoctorId = model.DoctorId,
                ScheduledDate = model.ScheduledDate,
                RoomNo = model.RoomNo,
                Forenoon = model.Forenoon,
                Afternoon = model.Afternoon
            };
            _db.ScheduledDoctors.Add(entity);
            _db.SaveChanges();
            return RedirectToAction("ScheduledDoctor");
        }

        [HttpGet]
        public ActionResult DoctorInTelemedicine()
        {
            ViewBag.Title = "Doctors in Telemedicine";
            ViewBag.TelemedicineList = _db.DoctorsTelemedicine
                                            .OrderByDescending(t => t.Id)
                                            .ToList();
            return View(new DoctorTelemedicineDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoctorInTelemedicine(DoctorTelemedicineDto model)
        {
            ViewBag.Title = "Doctors in Telemedicine";

            if (!ModelState.IsValid)
            {
                ViewBag.TelemedicineList = _db.DoctorsTelemedicine.OrderByDescending(t => t.Id).ToList();
                return View(model);
            }

            var entity = new DoctorTelemedicine
            {
                DoctorName = model.DoctorName,
                Specialist = model.Specialist,
                RoomNo = model.RoomNo,
                Forenoon = model.Forenoon,
                Afternoon = model.Afternoon
            };
            _db.DoctorsTelemedicine.Add(entity);
            _db.SaveChanges();

            TempData["Success"] = "Telemedicine doctor added successfully.";
            return RedirectToAction("DoctorInTelemedicine");
        }

        [HttpGet]
        public ActionResult VisitingDoctor()
        {
            ViewBag.Title = "Visiting Doctors";
            ViewBag.VisitingList = _db.VisitingDoctors
                                        .OrderByDescending(v => v.Id)
                                        .ToList();
            return View(new VisitingDoctorDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VisitingDoctor(VisitingDoctorDto model)
        {
            ViewBag.Title = "Visiting Doctors";

            if (!ModelState.IsValid)
            {
                ViewBag.VisitingList = _db.VisitingDoctors.OrderByDescending(v => v.Id).ToList();
                return View(model);
            }

            var entity = new VisitingDoctor
            {
                DoctorName = model.DoctorName,
                Specialist = model.Specialist,
                RoomNo = model.RoomNo,
                Forenoon = model.Forenoon,
                Afternoon = model.Afternoon
            };
            _db.VisitingDoctors.Add(entity);
            _db.SaveChanges();

            TempData["Success"] = "Visiting doctor added successfully.";
            return RedirectToAction("VisitingDoctor");
        }

        [HttpGet]
        public ActionResult DoctorInShift()
        {
            ViewBag.Title = "Doctor In Shift";
            var dto = new DoctorInShiftDto
            {
                DoctorList = new SelectList(_db.Doctors.OrderBy(d => d.Name).ToList(), "Id", "Name")
            };
            ViewBag.ShiftList = _db.DoctorsInShift
                                    .Include("Doctor")
                                    .OrderByDescending(s => s.Id)
                                    .ToList();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoctorInShift(DoctorInShiftDto model)
        {
            ViewBag.Title = "Doctor In Shift";

            if (!ModelState.IsValid)
            {
                model.DoctorList = new SelectList(_db.Doctors.OrderBy(d => d.Name).ToList(), "Id", "Name", model.DoctorId);
                ViewBag.ShiftList = _db.DoctorsInShift.Include("Doctor").OrderByDescending(s => s.Id).ToList();
                return View(model);
            }

            var entity = new DoctorInShift
            {
                DoctorId = model.DoctorId,
                Shift = model.Shift
            };
            _db.DoctorsInShift.Add(entity);
            _db.SaveChanges();

            TempData["Success"] = "Doctor added to shift successfully.";
            return RedirectToAction("DoctorInShift");
        }

        [HttpGet]
        public ActionResult DoctorsOnLeave()
        {
            ViewBag.Title = "Doctors on Leave";
            var dto = new DoctorOnLeaveDto
            {
                FromDate = System.DateTime.Today,
                ToDate = System.DateTime.Today,
                DoctorList = new SelectList(_db.Doctors.OrderBy(d => d.Name).ToList(), "Id", "Name")
            };
            ViewBag.LeaveList = _db.DoctorsOnLeave
                                    .Include("Doctor")
                                    .OrderByDescending(l => l.FromDate)
                                    .ToList();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoctorsOnLeave(DoctorOnLeaveDto model)
        {
            ViewBag.Title = "Doctors on Leave";

            if (model.ToDate < model.FromDate)
            {
                ModelState.AddModelError("ToDate", "To Date cannot be earlier than From Date");
            }

            if (!ModelState.IsValid)
            {
                model.DoctorList = new SelectList(_db.Doctors.OrderBy(d => d.Name).ToList(), "Id", "Name", model.DoctorId);
                ViewBag.LeaveList = _db.DoctorsOnLeave.Include("Doctor").OrderByDescending(l => l.FromDate).ToList();
                return View(model);
            }

            var entity = new DoctorOnLeave
            {
                DoctorId = model.DoctorId,
                FromDate = model.FromDate,
                ToDate = model.ToDate
            };
            _db.DoctorsOnLeave.Add(entity);
            _db.SaveChanges();

            TempData["Success"] = "Leave recorded successfully.";
            return RedirectToAction("DoctorsOnLeave");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}