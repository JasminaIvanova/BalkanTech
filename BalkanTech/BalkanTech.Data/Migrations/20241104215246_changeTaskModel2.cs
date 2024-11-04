using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BalkanTech.Data.Migrations
{
    /// <inheritdoc />
    public partial class changeTaskModel2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedDate",
                table: "MaintananceTasks",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedDate",
                table: "MaintananceTasks");
        }
    }
}
