namespace BanHang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class thangl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Product", "PriceOrigin", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Order", "Shipping", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Order", "Shipping");
            DropColumn("dbo.Product", "PriceOrigin");
        }
    }
}
