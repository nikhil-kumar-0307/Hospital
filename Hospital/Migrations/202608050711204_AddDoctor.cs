namespace Hospital.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDoctor : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Doctors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Specialist = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ScheduledDoctors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DoctorId = c.Int(nullable: false),
                        ScheduledDate = c.DateTime(nullable: false),
                        RoomNo = c.String(nullable: false, maxLength: 20),
                        Forenoon = c.Boolean(nullable: false),
                        Afternoon = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Doctors", t => t.DoctorId, cascadeDelete: true)
                .Index(t => t.DoctorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ScheduledDoctors", "DoctorId", "dbo.Doctors");
            DropIndex("dbo.ScheduledDoctors", new[] { "DoctorId" });
            DropTable("dbo.ScheduledDoctors");
            DropTable("dbo.Doctors");
        }
    }
}
