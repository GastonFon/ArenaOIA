namespace ArenaOIA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v6 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Problem", "ContestViewModel_Id", "dbo.ContestViewModel");
            DropIndex("dbo.Problem", new[] { "ContestViewModel_Id" });
            DropTable("dbo.ContestViewModel");
            DropTable("dbo.Problem");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Problem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        Puntaje = c.Int(nullable: false),
                        ContestViewModel_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
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
            
            CreateIndex("dbo.Problem", "ContestViewModel_Id");
            AddForeignKey("dbo.Problem", "ContestViewModel_Id", "dbo.ContestViewModel", "Id");
        }
    }
}
