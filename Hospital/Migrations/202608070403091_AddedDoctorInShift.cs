namespace Hospital.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDoctorInShift : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DoctorInShifts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DoctorId = c.Int(nullable: false),
                        Shift = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Doctors", t => t.DoctorId, cascadeDelete: true)
                .Index(t => t.DoctorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DoctorInShifts", "DoctorId", "dbo.Doctors");
            DropIndex("dbo.DoctorInShifts", new[] { "DoctorId" });
            DropTable("dbo.DoctorInShifts");
        }
    }
}
