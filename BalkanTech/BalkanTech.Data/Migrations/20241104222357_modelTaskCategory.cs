using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BalkanTech.Data.Migrations
{
    /// <inheritdoc />
    public partial class modelTaskCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TaskCategoryId",
                table: "MaintananceTasks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "TaskCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskCategory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaintananceTasks_TaskCategoryId",
                table: "MaintananceTasks",
                column: "TaskCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintananceTasks_TaskCategory_TaskCategoryId",
                table: "MaintananceTasks",
                column: "TaskCategoryId",
                principalTable: "TaskCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintananceTasks_TaskCategory_TaskCategoryId",
                table: "MaintananceTasks");

            migrationBuilder.DropTable(
                name: "TaskCategory");

            migrationBuilder.DropIndex(
                name: "IX_MaintananceTasks_TaskCategoryId",
                table: "MaintananceTasks");

            migrationBuilder.DropColumn(
                name: "TaskCategoryId",
                table: "MaintananceTasks");
        }
    }
}
