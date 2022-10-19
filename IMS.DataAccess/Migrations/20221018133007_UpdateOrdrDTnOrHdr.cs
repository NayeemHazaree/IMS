using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMS.DataAccess.Migrations
{
    public partial class UpdateOrdrDTnOrHdr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Responsible_User",
                table: "OrderDetails");

            migrationBuilder.AddColumn<string>(
                name: "Responsible_User",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Responsible_User",
                table: "OrderHeaders");

            migrationBuilder.AddColumn<string>(
                name: "Responsible_User",
                table: "OrderDetails",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
