namespace Hospital.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDateFieldsToVisitingTelemedicineShift : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DoctorInShifts", "ShiftDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.DoctorTelemedicines", "SessionDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.VisitingDoctors", "VisitDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VisitingDoctors", "VisitDate");
            DropColumn("dbo.DoctorTelemedicines", "SessionDate");
            DropColumn("dbo.DoctorInShifts", "ShiftDate");
        }
    }
}
