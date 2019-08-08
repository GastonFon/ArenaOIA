namespace ArenaOIA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Submission", "ProblemName", c => c.String());
            AlterColumn("dbo.Submission", "OIAJSubmissionId", c => c.String());
            DropColumn("dbo.Submission", "ProblemId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Submission", "ProblemId", c => c.Int(nullable: false));
            AlterColumn("dbo.Submission", "OIAJSubmissionId", c => c.Int(nullable: false));
            DropColumn("dbo.Submission", "ProblemName");
        }
    }
}
