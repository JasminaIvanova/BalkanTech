using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BalkanTech.Data.Migrations
{
    /// <inheritdoc />
    public partial class manyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaintananceTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2500)", maxLength: 2500, nullable: false),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintananceTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintananceTasks_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssignedTechniciansTasks",
                columns: table => new
                {
                    AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaintananceTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignedTechniciansTasks", x => new { x.AppUserId, x.MaintananceTaskId });
                    table.ForeignKey(
                        name: "FK_AssignedTechniciansTasks_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssignedTechniciansTasks_MaintananceTasks_MaintananceTaskId",
                        column: x => x.MaintananceTaskId,
                        principalTable: "MaintananceTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignedTechniciansTasks_MaintananceTaskId",
                table: "AssignedTechniciansTasks",
                column: "MaintananceTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintananceTasks_RoomId",
                table: "MaintananceTasks",
                column: "RoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignedTechniciansTasks");

            migrationBuilder.DropTable(
                name: "MaintananceTasks");
        }
    }
}
