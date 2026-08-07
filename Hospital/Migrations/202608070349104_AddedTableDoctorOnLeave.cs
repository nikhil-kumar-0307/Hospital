namespace Hospital.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTableDoctorOnLeave : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DoctorOnLeaves",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DoctorId = c.Int(nullable: false),
                        FromDate = c.DateTime(nullable: false),
                        ToDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Doctors", t => t.DoctorId, cascadeDelete: true)
                .Index(t => t.DoctorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DoctorOnLeaves", "DoctorId", "dbo.Doctors");
            DropIndex("dbo.DoctorOnLeaves", new[] { "DoctorId" });
            DropTable("dbo.DoctorOnLeaves");
        }
    }
}
