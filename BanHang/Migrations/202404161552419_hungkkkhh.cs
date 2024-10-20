namespace BanHang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hungkkkhh : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Category", "Title", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.News", "Title", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.Posts", "Title", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.Contact", "Phone", c => c.String(maxLength: 10));
            AlterColumn("dbo.Product", "Name", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Product", "Name", c => c.String(maxLength: 50));
            AlterColumn("dbo.Contact", "Phone", c => c.String(maxLength: 15));
            AlterColumn("dbo.Posts", "Title", c => c.String(maxLength: 150));
            AlterColumn("dbo.News", "Title", c => c.String(maxLength: 150));
            AlterColumn("dbo.Category", "Title", c => c.String(maxLength: 50));
        }
    }
}
