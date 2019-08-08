namespace ArenaOIA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Submission", "Username", c => c.String());
            AddColumn("dbo.Submission", "ContestId", c => c.Int(nullable: false));
            AddColumn("dbo.Submission", "ProblemId", c => c.Int(nullable: false));
            AddColumn("dbo.Submission", "OIAJSubmissionId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Submission", "OIAJSubmissionId");
            DropColumn("dbo.Submission", "ProblemId");
            DropColumn("dbo.Submission", "ContestId");
            DropColumn("dbo.Submission", "Username");
        }
    }
}
