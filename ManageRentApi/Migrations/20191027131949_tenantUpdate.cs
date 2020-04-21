using Microsoft.EntityFrameworkCore.Migrations;

namespace ManageRentApi.Migrations
{
    public partial class tenantUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Houses_Users_OwnerId",
                table: "Houses");

            migrationBuilder.DropIndex(
                name: "IX_Houses_OwnerId",
                table: "Houses");

            migrationBuilder.DropColumn(
                name: "MyProperty",
                table: "Tenants");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Tenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Tenants",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "Houses",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Tenants");

            migrationBuilder.AddColumn<int>(
                name: "MyProperty",
                table: "Tenants",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "Houses",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_Houses_OwnerId",
                table: "Houses",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Houses_Users_OwnerId",
                table: "Houses",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
