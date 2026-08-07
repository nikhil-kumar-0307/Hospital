namespace Hospital.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedVisitingDoctor : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VisitingDoctors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DoctorName = c.String(nullable: false, maxLength: 150),
                        Specialist = c.String(nullable: false, maxLength: 100),
                        RoomNo = c.String(nullable: false, maxLength: 20),
                        Forenoon = c.Boolean(nullable: false),
                        Afternoon = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.VisitingDoctors");
        }
    }
}
