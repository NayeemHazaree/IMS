using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMS.DataAccess.Migrations
{
    public partial class updateSalesTb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Invoice",
                table: "Sales",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Invoice",
                table: "Sales");
        }
    }
}
