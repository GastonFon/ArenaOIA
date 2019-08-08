namespace ArenaOIA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ContestViewModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        Inicio = c.DateTime(nullable: false),
                        Fin = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Problem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        Puntaje = c.Int(nullable: false),
                        ContestViewModel_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ContestViewModel", t => t.ContestViewModel_Id)
                .Index(t => t.ContestViewModel_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Problem", "ContestViewModel_Id", "dbo.ContestViewModel");
            DropIndex("dbo.Problem", new[] { "ContestViewModel_Id" });
            DropTable("dbo.Problem");
            DropTable("dbo.ContestViewModel");
        }
    }
}
