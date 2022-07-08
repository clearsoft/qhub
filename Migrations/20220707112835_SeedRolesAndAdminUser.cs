using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QHub.Migrations
{
    public partial class SeedRolesAndAdminUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                                    INSERT INTO [AspNetUsers] ([Id],[FirstName],[LastName],[UserName],[NormalizedUserName],[Email],[NormalizedEmail],[EmailConfirmed],[PasswordHash],[SecurityStamp],[ConcurrencyStamp],[PhoneNumber],[PhoneNumberConfirmed],[TwoFactorEnabled],[LockoutEnd],[LockoutEnabled],[AccessFailedCount]) VALUES ('ee1a0184-542b-443d-a781-08d620813a70','Admin','User','qhub_admin@clearsoft.com.au','QHUB_ADMIN@CLEARSOFT.COM.AU','qhub_admin@clearsoft.com.au','QHUB_ADMIN@CLEARSOFT.COM.AU',1,'AQAAAAEAACcQAAAAEAiS3s2SM9Ux4vYPOmPIbUto61kaBCe5h9Pvhpuxwcgod7qQx2v76QtWH3x+jbHfaQ==','JFNWKCNUJOQ53FV5RMRFOLTDAPTMAW6F','8e581845-0a74-4c8d-93fd-633cde48307f',NULL,0,0,NULL,1,0);

                                    INSERT INTO [AspNetRoles] ([Id],[Name],[NormalizedName],[ConcurrencyStamp]) VALUES ('91ce2a9b-2871-4ff7-8708-50c4fecf4d13','Teacher','TEACHER','6bc04f6e-8a29-45e7-97ed-d8ce96d0caaa');
                                    INSERT INTO [AspNetRoles] ([Id],[Name],[NormalizedName],[ConcurrencyStamp]) VALUES ('fb42f484-11a9-48c8-bff2-91b4d6a5057a','Admin','ADMIN','71b46a9b-11b4-4945-b3d1-da3a4a64eb39');
                                    INSERT INTO [AspNetRoles] ([Id],[Name],[NormalizedName],[ConcurrencyStamp]) VALUES ('fcd9f7f2-e9c8-46d2-b3f3-f1e3ee59a036','Student','STUDENT','381b75da-272c-4389-9cdb-653108c75ba1');

                                    INSERT INTO [AspNetUserRoles] ([UserId],[RoleId]) VALUES ('ee1a0184-542b-443d-a781-08d620813a70','fb42f484-11a9-48c8-bff2-91b4d6a5057a');
                                    ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
