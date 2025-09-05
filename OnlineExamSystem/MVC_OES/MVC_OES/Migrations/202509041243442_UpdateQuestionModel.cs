namespace MVC_OES.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateQuestionModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Questions", "QuestionType", c => c.String(nullable: false));
            AlterColumn("dbo.Questions", "Subject", c => c.String(nullable: false));
            AlterColumn("dbo.Questions", "QuestionText", c => c.String(nullable: false));
            AlterColumn("dbo.Questions", "CorrectAnswer", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Questions", "CorrectAnswer", c => c.String());
            AlterColumn("dbo.Questions", "QuestionText", c => c.String());
            AlterColumn("dbo.Questions", "Subject", c => c.String());
            DropColumn("dbo.Questions", "QuestionType");
        }
    }
}
