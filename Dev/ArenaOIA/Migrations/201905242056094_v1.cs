namespace ArenaOIA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v1 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Contest");
            AlterColumn("dbo.Contest", "Id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Contest", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Contest");
            AlterColumn("dbo.Contest", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Contest", "Id");
        }
    }
}
