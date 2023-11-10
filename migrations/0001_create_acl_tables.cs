using FluentMigrator;

namespace AclManager.Migrations
{
    [Migration(1)]
    public class CreateAclTables : Migration
    {
        public override void Up()
        {
            Create.Table("aces")
                  .WithColumn("ace_id").AsInt32().NotNullable().PrimaryKey().Identity()
                  .WithColumn("principal").AsString(128).NotNullable()
                  .WithColumn("object").AsString(128).NotNullable()
                  .WithColumn("allow_type").AsString(5).NotNullable();

            Create.Table("principals")
                  .WithColumn("principal_id").AsInt32().NotNullable().PrimaryKey().Identity()
                  .WithColumn("child").AsString(128).NotNullable()
                  .WithColumn("parent").AsString(128).NotNullable();
        }

        public override void Down()
        {
            Delete.Table("aces");
            Delete.Table("principals");
        }
    }
}
