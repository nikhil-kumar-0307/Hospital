using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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

        public ActionResult Index(DateTime? date)
        {
            ViewBag.Title = "Doctor's Availability";

            var selectedDate = (date ?? DateTime.Today).Date;

            ViewBag.ScheduledList = _db.ScheduledDoctors
                                        .Include("Doctor")
                                        .Where(s => DbFunctions.TruncateTime(s.ScheduledDate) == selectedDate)
                                        .OrderBy(s => s.Doctor.Name)
                                        .ToList();

            ViewBag.VisitingList = _db.VisitingDoctors
                                        .Where(v => DbFunctions.TruncateTime(v.VisitDate) == selectedDate)
                                        .OrderBy(v => v.DoctorName)
                                        .ToList();

            ViewBag.TelemedicineList = _db.DoctorsTelemedicine
                                            .Where(t => DbFunctions.TruncateTime(t.SessionDate) == selectedDate)
                                            .OrderBy(t => t.DoctorName)
                                            .ToList();

            ViewBag.ShiftList = _db.DoctorsInShift
                                    .Include("Doctor")
                                    .Where(s => DbFunctions.TruncateTime(s.ShiftDate) == selectedDate)
                                    .OrderBy(s => s.Doctor.Name)
                                    .ToList();

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
            ViewBag.IsToday = selectedDate == DateTime.Today;
            ViewBag.LastUpdated = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

            return View();
        }

        [HttpGet]
        public ActionResult AddDoctor(int? id)
        {
            ViewBag.Title = id.HasValue ? "Edit Doctor" : "Add Doctor";
            ViewBag.DoctorList = _db.Doctors.OrderByDescending(d => d.Id).ToList();

            if (id.HasValue)
            {
                var doctor = _db.Doctors.Find(id.Value);
                if (doctor == null)
                {
                    TempData["Error"] = "Doctor not found.";
                    return RedirectToAction("AddDoctor");
                }

                var editModel = new AddDoctorDto
                {
                    Id = doctor.Id,
                    Name = doctor.Name,
                    Specialist = doctor.Specialist
                };
                return View(editModel);
            }

            return View(new AddDoctorDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDoctor(AddDoctorDto model)
        {
            ViewBag.Title = model.Id > 0 ? "Edit Doctor" : "Add Doctor";

            if (!ModelState.IsValid)
            {
                ViewBag.DoctorList = _db.Doctors.OrderByDescending(d => d.Id).ToList();
                return View(model);
            }

            if (model.Id > 0)
            {
                var doctor = _db.Doctors.Find(model.Id);
                if (doctor == null)
                {
                    TempData["Error"] = "Doctor not found.";
                    return RedirectToAction("AddDoctor", new { id = (int?)null });
                }

                doctor.Name = model.Name;
                doctor.Specialist = model.Specialist;
                _db.SaveChanges();

                TempData["Success"] = "Doctor updated successfully.";
            }
            else
            {
                var doctor = new Doctor
                {
                    Name = model.Name,
                    Specialist = model.Specialist
                };

                _db.Doctors.Add(doctor);
                _db.SaveChanges();

                TempData["Success"] = "Doctor added successfully.";
            }

            // Explicitly clear id so it doesn't inherit the ambient route value
            return RedirectToAction("AddDoctor", new { id = (int?)null });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDoctor(int id)
        {
            var doctor = _db.Doctors.Find(id);
            if (doctor == null)
            {
                TempData["Error"] = "Doctor not found.";
                return RedirectToAction("AddDoctor");
            }

            try
            {
                _db.Doctors.Remove(doctor);
                _db.SaveChanges();
                TempData["Success"] = "Doctor deleted successfully.";
            }
            catch (DbUpdateException)
            {
                // Happens if the doctor is referenced elsewhere (scheduled, shift, leave etc.)
                TempData["Error"] = "Cannot delete this doctor because they have existing records (schedules, shifts, leaves, etc.).";
            }

            return RedirectToAction("AddDoctor");
        }

        [HttpGet]
        public ActionResult ScheduledDoctor(int? id)
        {
            ViewBag.Title = id.HasValue ? "Edit Scheduled Doctor" : "Scheduled Doctor";

            ScheduledDoctorDto dto;

            if (id.HasValue)
            {
                var entity = _db.ScheduledDoctors.Find(id.Value);
                if (entity == null)
                {
                    TempData["Error"] = "Scheduled record not found.";
                    return RedirectToAction("ScheduledDoctor", new { id = (int?)null });
                }

                dto = new ScheduledDoctorDto
                {
                    Id = entity.Id,
                    DoctorId = entity.DoctorId,
                    ScheduledDate = entity.ScheduledDate,
                    RoomNo = entity.RoomNo,
                    Forenoon = entity.Forenoon,
                    Afternoon = entity.Afternoon
                };
            }
            else
            {
                dto = new ScheduledDoctorDto { ScheduledDate = DateTime.Today };
            }

            dto.DoctorList = new SelectList(_db.Doctors.OrderBy(d => d.Name).ToList(), "Id", "Name", dto.DoctorId);
            ViewBag.ScheduledList = _db.ScheduledDoctors.Include("Doctor").OrderByDescending(s => s.ScheduledDate).ToList();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ScheduledDoctor(ScheduledDoctorDto model)
        {
            ViewBag.Title = model.Id > 0 ? "Edit Scheduled Doctor" : "Scheduled Doctor";

            if (!ModelState.IsValid)
            {
                model.DoctorList = new SelectList(_db.Doctors.OrderBy(d => d.Name).ToList(), "Id", "Name", model.DoctorId);
                ViewBag.ScheduledList = _db.ScheduledDoctors.Include("Doctor").OrderByDescending(s => s.ScheduledDate).ToList();
                return View(model);
            }

            if (model.Id > 0)
            {
                var entity = _db.ScheduledDoctors.Find(model.Id);
                if (entity == null)
                {
                    TempData["Error"] = "Scheduled record not found.";
                    return RedirectToAction("ScheduledDoctor", new { id = (int?)null });
                }

                entity.DoctorId = model.DoctorId;
                entity.ScheduledDate = model.ScheduledDate;
                entity.RoomNo = model.RoomNo;
                entity.Forenoon = model.Forenoon;
                entity.Afternoon = model.Afternoon;
                _db.SaveChanges();

                TempData["Success"] = "Scheduled record updated successfully.";
            }
            else
            {
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

                TempData["Success"] = "Doctor scheduled successfully.";
            }

            return RedirectToAction("ScheduledDoctor", new { id = (int?)null });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteScheduledDoctor(int id)
        {
            var entity = _db.ScheduledDoctors.Find(id);
            if (entity == null)
            {
                TempData["Error"] = "Scheduled record not found.";
                return RedirectToAction("ScheduledDoctor", new { id = (int?)null });
            }

            try
            {
                _db.ScheduledDoctors.Remove(entity);
                _db.SaveChanges();
                TempData["Success"] = "Scheduled record deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Cannot delete this record — it may be referenced elsewhere.";
            }

            return RedirectToAction("ScheduledDoctor", new { id = (int?)null });
        }

        [HttpGet]
        public ActionResult DoctorInTelemedicine(int? id)
        {
            ViewBag.Title = id.HasValue ? "Edit Doctor In Telemedicine" : "Doctors in Telemedicine";

            DoctorTelemedicineDto dto;

            if (id.HasValue)
            {
                var entity = _db.DoctorsTelemedicine.Find(id.Value);
                if (entity == null)
                {
                    TempData["Error"] = "Telemedicine record not found.";
                    return RedirectToAction("DoctorInTelemedicine", new { id = (int?)null });
                }

                dto = new DoctorTelemedicineDto
                {
                    Id = entity.Id,
                    DoctorName = entity.DoctorName,
                    Specialist = entity.Specialist,
                    RoomNo = entity.RoomNo,
                    Forenoon = entity.Forenoon,
                    Afternoon = entity.Afternoon,
                    SessionDate = entity.SessionDate
                };
            }
            else
            {
                dto = new DoctorTelemedicineDto();
            }

            ViewBag.TelemedicineList = _db.DoctorsTelemedicine.OrderByDescending(t => t.Id).ToList();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoctorInTelemedicine(DoctorTelemedicineDto model)
        {
            ViewBag.Title = model.Id > 0 ? "Edit Doctor In Telemedicine" : "Doctors in Telemedicine";

            if (!ModelState.IsValid)
            {
                ViewBag.TelemedicineList = _db.DoctorsTelemedicine.OrderByDescending(t => t.Id).ToList();
                return View(model);
            }

            if (model.Id > 0)
            {
                var entity = _db.DoctorsTelemedicine.Find(model.Id);
                if (entity == null)
                {
                    TempData["Error"] = "Telemedicine record not found.";
                    return RedirectToAction("DoctorInTelemedicine", new { id = (int?)null });
                }

                entity.DoctorName = model.DoctorName;
                entity.Specialist = model.Specialist;
                entity.RoomNo = model.RoomNo;
                entity.Forenoon = model.Forenoon;
                entity.Afternoon = model.Afternoon;
                entity.SessionDate = model.SessionDate;
                _db.SaveChanges();

                TempData["Success"] = "Telemedicine record updated successfully.";
            }
            else
            {
                var entity = new DoctorTelemedicine
                {
                    DoctorName = model.DoctorName,
                    Specialist = model.Specialist,
                    RoomNo = model.RoomNo,
                    Forenoon = model.Forenoon,
                    Afternoon = model.Afternoon,
                    SessionDate = model.SessionDate
                };
                _db.DoctorsTelemedicine.Add(entity);
                _db.SaveChanges();

                TempData["Success"] = "Telemedicine doctor added successfully.";
            }

            return RedirectToAction("DoctorInTelemedicine", new { id = (int?)null });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDoctorInTelemedicine(int id)
        {
            var entity = _db.DoctorsTelemedicine.Find(id);
            if (entity == null)
            {
                TempData["Error"] = "Telemedicine record not found.";
                return RedirectToAction("DoctorInTelemedicine", new { id = (int?)null });
            }

            try
            {
                _db.DoctorsTelemedicine.Remove(entity);
                _db.SaveChanges();
                TempData["Success"] = "Telemedicine record deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Cannot delete this record — it may be referenced elsewhere.";
            }

            return RedirectToAction("DoctorInTelemedicine", new { id = (int?)null });
        }

        [HttpGet]
        public ActionResult VisitingDoctor(int? id)
        {
            ViewBag.Title = id.HasValue ? "Edit Visiting Doctor" : "Visiting Doctors";

            VisitingDoctorDto dto;

            if (id.HasValue)
            {
                var entity = _db.VisitingDoctors.Find(id.Value);
                if (entity == null)
                {
                    TempData["Error"] = "Visiting doctor record not found.";
                    return RedirectToAction("VisitingDoctor", new { id = (int?)null });
                }

                dto = new VisitingDoctorDto
                {
                    Id = entity.Id,
                    DoctorName = entity.DoctorName,
                    Specialist = entity.Specialist,
                    RoomNo = entity.RoomNo,
                    Forenoon = entity.Forenoon,
                    Afternoon = entity.Afternoon,
                    VisitDate = entity.VisitDate
                };
            }
            else
            {
                dto = new VisitingDoctorDto();
            }

            ViewBag.VisitingList = _db.VisitingDoctors.OrderByDescending(v => v.Id).ToList();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VisitingDoctor(VisitingDoctorDto model)
        {
            ViewBag.Title = model.Id > 0 ? "Edit Visiting Doctor" : "Visiting Doctors";

            if (!ModelState.IsValid)
            {
                ViewBag.VisitingList = _db.VisitingDoctors.OrderByDescending(v => v.Id).ToList();
                return View(model);
            }

            if (model.Id > 0)
            {
                var entity = _db.VisitingDoctors.Find(model.Id);
                if (entity == null)
                {
                    TempData["Error"] = "Visiting doctor record not found.";
                    return RedirectToAction("VisitingDoctor", new { id = (int?)null });
                }

                entity.DoctorName = model.DoctorName;
                entity.Specialist = model.Specialist;
                entity.RoomNo = model.RoomNo;
                entity.Forenoon = model.Forenoon;
                entity.Afternoon = model.Afternoon;
                entity.VisitDate = model.VisitDate;
                _db.SaveChanges();

                TempData["Success"] = "Visiting doctor updated successfully.";
            }
            else
            {
                var entity = new VisitingDoctor
                {
                    DoctorName = model.DoctorName,
                    Specialist = model.Specialist,
                    RoomNo = model.RoomNo,
                    Forenoon = model.Forenoon,
                    Afternoon = model.Afternoon,
                    VisitDate = model.VisitDate
                };
                _db.VisitingDoctors.Add(entity);
                _db.SaveChanges();

                TempData["Success"] = "Visiting doctor added successfully.";
            }

            return RedirectToAction("VisitingDoctor", new { id = (int?)null });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteVisitingDoctor(int id)
        {
            var entity = _db.VisitingDoctors.Find(id);
            if (entity == null)
            {
                TempData["Error"] = "Visiting doctor record not found.";
                return RedirectToAction("VisitingDoctor", new { id = (int?)null });
            }

            try
            {
                _db.VisitingDoctors.Remove(entity);
                _db.SaveChanges();
                TempData["Success"] = "Visiting doctor deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Cannot delete this record — it may be referenced elsewhere.";
            }

            return RedirectToAction("VisitingDoctor", new { id = (int?)null });
        }

        [HttpGet]
        public ActionResult DoctorInShift(int? id)
        {
            ViewBag.Title = id.HasValue ? "Edit Doctor In Shift" : "Add Doctor In Shift";

            DoctorInShiftDto dto;

            if (id.HasValue)
            {
                var entity = _db.DoctorsInShift.Find(id.Value);
                if (entity == null)
                {
                    TempData["Error"] = "Shift record not found.";
                    return RedirectToAction("DoctorInShift", new { id = (int?)null });
                }

                dto = new DoctorInShiftDto
                {
                    Id = entity.Id,
                    DoctorId = entity.DoctorId,
                    Shift = entity.Shift,
                    ShiftDate = entity.ShiftDate
                };
            }
            else
            {
                dto = new DoctorInShiftDto();
            }

            dto.DoctorList = new SelectList(_db.Doctors.OrderBy(d => d.Name).ToList(), "Id", "Name", dto.DoctorId);

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
            ViewBag.Title = model.Id > 0 ? "Edit Doctor In Shift" : "Add Doctor In Shift";

            if (!ModelState.IsValid)
            {
                model.DoctorList = new SelectList(_db.Doctors.OrderBy(d => d.Name).ToList(), "Id", "Name", model.DoctorId);
                ViewBag.ShiftList = _db.DoctorsInShift.Include("Doctor").OrderByDescending(s => s.Id).ToList();
                return View(model);
            }

            if (model.Id > 0)
            {
                var entity = _db.DoctorsInShift.Find(model.Id);
                if (entity == null)
                {
                    TempData["Error"] = "Shift record not found.";
                    return RedirectToAction("DoctorInShift", new { id = (int?)null });
                }

                entity.DoctorId = model.DoctorId;
                entity.Shift = model.Shift;
                entity.ShiftDate = model.ShiftDate;
                _db.SaveChanges();

                TempData["Success"] = "Shift record updated successfully.";
            }
            else
            {
                var entity = new DoctorInShift
                {
                    DoctorId = model.DoctorId,
                    Shift = model.Shift,
                    ShiftDate = model.ShiftDate
                };
                _db.DoctorsInShift.Add(entity);
                _db.SaveChanges();

                TempData["Success"] = "Doctor added to shift successfully.";
            }

            return RedirectToAction("DoctorInShift", new { id = (int?)null });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDoctorInShift(int id)
        {
            var entity = _db.DoctorsInShift.Find(id);
            if (entity == null)
            {
                TempData["Error"] = "Shift record not found.";
                return RedirectToAction("DoctorInShift", new { id = (int?)null });
            }

            try
            {
                _db.DoctorsInShift.Remove(entity);
                _db.SaveChanges();
                TempData["Success"] = "Shift record deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Cannot delete this record — it may be referenced elsewhere.";
            }

            return RedirectToAction("DoctorInShift", new { id = (int?)null });
        }

        [HttpGet]
        public ActionResult DoctorsOnLeave(int? id)
        {
            ViewBag.Title = id.HasValue ? "Edit Doctor On Leave" : "Doctors on Leave";

            DoctorOnLeaveDto dto;

            if (id.HasValue)
            {
                var entity = _db.DoctorsOnLeave.Find(id.Value);
                if (entity == null)
                {
                    TempData["Error"] = "Leave record not found.";
                    return RedirectToAction("DoctorsOnLeave", new { id = (int?)null });
                }

                dto = new DoctorOnLeaveDto
                {
                    Id = entity.Id,
                    DoctorId = entity.DoctorId,
                    FromDate = entity.FromDate,
                    ToDate = entity.ToDate
                };
            }
            else
            {
                dto = new DoctorOnLeaveDto { FromDate = DateTime.Today, ToDate = DateTime.Today };
            }

            dto.DoctorList = new SelectList(_db.Doctors.OrderBy(d => d.Name).ToList(), "Id", "Name", dto.DoctorId);
            ViewBag.LeaveList = _db.DoctorsOnLeave.Include("Doctor").OrderByDescending(l => l.FromDate).ToList();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoctorsOnLeave(DoctorOnLeaveDto model)
        {
            ViewBag.Title = model.Id > 0 ? "Edit Doctor On Leave" : "Doctors on Leave";

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

            if (model.Id > 0)
            {
                var entity = _db.DoctorsOnLeave.Find(model.Id);
                if (entity == null)
                {
                    TempData["Error"] = "Leave record not found.";
                    return RedirectToAction("DoctorsOnLeave", new { id = (int?)null });
                }

                entity.DoctorId = model.DoctorId;
                entity.FromDate = model.FromDate;
                entity.ToDate = model.ToDate;
                _db.SaveChanges();

                TempData["Success"] = "Leave record updated successfully.";
            }
            else
            {
                var entity = new DoctorOnLeave
                {
                    DoctorId = model.DoctorId,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate
                };
                _db.DoctorsOnLeave.Add(entity);
                _db.SaveChanges();

                TempData["Success"] = "Leave recorded successfully.";
            }

            return RedirectToAction("DoctorsOnLeave", new { id = (int?)null });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDoctorsOnLeave(int id)
        {
            var entity = _db.DoctorsOnLeave.Find(id);
            if (entity == null)
            {
                TempData["Error"] = "Leave record not found.";
                return RedirectToAction("DoctorsOnLeave", new { id = (int?)null });
            }

            try
            {
                _db.DoctorsOnLeave.Remove(entity);
                _db.SaveChanges();
                TempData["Success"] = "Leave record deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Cannot delete this record — it may be referenced elsewhere.";
            }

            return RedirectToAction("DoctorsOnLeave", new { id = (int?)null });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}