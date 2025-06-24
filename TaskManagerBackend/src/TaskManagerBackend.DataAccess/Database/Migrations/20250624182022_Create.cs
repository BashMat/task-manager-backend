using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagerBackend.DataAccess.Database.Migrations
{
    /// <inheritdoc />
    public partial class Create : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(256)", maxLength: 256, nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("User_PK", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrackingLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("TrackingLog_PK", x => x.Id);
                    table.ForeignKey(
                        name: "TrackingLog_CreatedBy_FK",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "TrackingLog_UpdatedBy_FK",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TrackingLogEntryStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    TrackingLogId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("TrackingLogEntryStatus_PK", x => x.Id);
                    table.ForeignKey(
                        name: "TrackingLogEntryStatus_CreatedBy_FK",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "TrackingLogEntryStatus_TrackingLogId_FK",
                        column: x => x.TrackingLogId,
                        principalTable: "TrackingLog",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "TrackingLogEntryStatus_UpdatedBy_FK",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TrackingLogEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    TrackingLogId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: true),
                    OrderIndex = table.Column<decimal>(type: "decimal(19,2)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("TrackingLogEntry_PK", x => x.Id);
                    table.ForeignKey(
                        name: "TrackingLogEntry_CreatedBy_FK",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "TrackingLogEntry_StatusId_FK",
                        column: x => x.StatusId,
                        principalTable: "TrackingLogEntryStatus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "TrackingLogEntry_TrackingLogId_FK",
                        column: x => x.TrackingLogId,
                        principalTable: "TrackingLog",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "TrackingLogEntry_UpdatedBy_FK",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrackingLog_CreatedBy",
                table: "TrackingLog",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TrackingLog_UpdatedBy",
                table: "TrackingLog",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TrackingLogEntry_CreatedBy",
                table: "TrackingLogEntry",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TrackingLogEntry_StatusId",
                table: "TrackingLogEntry",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackingLogEntry_TrackingLogId",
                table: "TrackingLogEntry",
                column: "TrackingLogId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackingLogEntry_UpdatedBy",
                table: "TrackingLogEntry",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TrackingLogEntryStatus_CreatedBy",
                table: "TrackingLogEntryStatus",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TrackingLogEntryStatus_TrackingLogId",
                table: "TrackingLogEntryStatus",
                column: "TrackingLogId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackingLogEntryStatus_UpdatedBy",
                table: "TrackingLogEntryStatus",
                column: "UpdatedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackingLogEntry");

            migrationBuilder.DropTable(
                name: "TrackingLogEntryStatus");

            migrationBuilder.DropTable(
                name: "TrackingLog");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
