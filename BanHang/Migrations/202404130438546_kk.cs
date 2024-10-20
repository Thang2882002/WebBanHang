namespace BanHang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class kk : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Contact",
                c => new
                    {
                        IDContact = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Email = c.String(maxLength: 150),
                        Phone = c.String(maxLength: 15),
                        Message = c.String(maxLength: 4000),
                        CreatedBy = c.String(maxLength: 50),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 50),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.IDContact);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Contact");
        }
    }
}
