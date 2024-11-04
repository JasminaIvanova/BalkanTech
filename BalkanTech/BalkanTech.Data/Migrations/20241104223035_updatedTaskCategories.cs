using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BalkanTech.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatedTaskCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintananceTasks_TaskCategory_TaskCategoryId",
                table: "MaintananceTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskCategory",
                table: "TaskCategory");

            migrationBuilder.RenameTable(
                name: "TaskCategory",
                newName: "TaskCategories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskCategories",
                table: "TaskCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintananceTasks_TaskCategories_TaskCategoryId",
                table: "MaintananceTasks",
                column: "TaskCategoryId",
                principalTable: "TaskCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintananceTasks_TaskCategories_TaskCategoryId",
                table: "MaintananceTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskCategories",
                table: "TaskCategories");

            migrationBuilder.RenameTable(
                name: "TaskCategories",
                newName: "TaskCategory");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskCategory",
                table: "TaskCategory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintananceTasks_TaskCategory_TaskCategoryId",
                table: "MaintananceTasks",
                column: "TaskCategoryId",
                principalTable: "TaskCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
