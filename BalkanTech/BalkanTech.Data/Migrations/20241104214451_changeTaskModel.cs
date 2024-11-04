using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BalkanTech.Data.Migrations
{
    /// <inheritdoc />
    public partial class changeTaskModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "MaintananceTasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "MaintananceTasks");
        }
    }
}
