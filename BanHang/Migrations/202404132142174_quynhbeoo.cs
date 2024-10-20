namespace BanHang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class quynhbeoo : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FeedBack", "IDProduct", "dbo.Product");
            DropForeignKey("dbo.FeedBack", "UserID", "dbo.User");
            DropForeignKey("dbo.ChatMessages", "Receiver_UserID", "dbo.User");
            DropForeignKey("dbo.ChatMessages", "Sender_UserID", "dbo.User");
            DropIndex("dbo.ChatMessages", new[] { "Receiver_UserID" });
            DropIndex("dbo.ChatMessages", new[] { "Sender_UserID" });
            DropIndex("dbo.FeedBack", new[] { "UserID" });
            DropIndex("dbo.FeedBack", new[] { "IDProduct" });
            DropTable("dbo.ChatMessages");
            DropTable("dbo.FeedBack");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.FeedBack",
                c => new
                    {
                        FeedBackID = c.Int(nullable: false),
                        UserID = c.Int(),
                        IDProduct = c.Int(),
                        CreatedBy = c.String(maxLength: 50),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 50),
                        ModifiedDate = c.DateTime(),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.FeedBackID);
            
            CreateTable(
                "dbo.ChatMessages",
                c => new
                    {
                        ChatMessageID = c.Int(nullable: false, identity: true),
                        SenderID = c.Int(nullable: false),
                        ReceiverID = c.Int(nullable: false),
                        MessageContent = c.String(),
                        SentAt = c.DateTime(nullable: false),
                        Receiver_UserID = c.Int(),
                        Sender_UserID = c.Int(),
                    })
                .PrimaryKey(t => t.ChatMessageID);
            
            CreateIndex("dbo.FeedBack", "IDProduct");
            CreateIndex("dbo.FeedBack", "UserID");
            CreateIndex("dbo.ChatMessages", "Sender_UserID");
            CreateIndex("dbo.ChatMessages", "Receiver_UserID");
            AddForeignKey("dbo.ChatMessages", "Sender_UserID", "dbo.User", "UserID");
            AddForeignKey("dbo.ChatMessages", "Receiver_UserID", "dbo.User", "UserID");
            AddForeignKey("dbo.FeedBack", "UserID", "dbo.User", "UserID");
            AddForeignKey("dbo.FeedBack", "IDProduct", "dbo.Product", "IDProduct");
        }
    }
}
