using System.Data.Entity;
using Hospital.Models;

namespace Hospital.Data
{
    public class HospitalDbContext : DbContext
    {
        public HospitalDbContext() : base("HospitalDbContext")
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<ScheduledDoctor> ScheduledDoctors { get; set; }
        public DbSet<DoctorOnLeave> DoctorsOnLeave { get; set; }
        public DbSet<DoctorInShift> DoctorsInShift { get; set; }
        public DbSet<DoctorTelemedicine> DoctorsTelemedicine { get; set; }
        public DbSet<VisitingDoctor> VisitingDoctors { get; set; }
    }
}