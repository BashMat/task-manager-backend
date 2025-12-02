using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagerBackend.DataAccess.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddEventTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityType = table.Column<int>(type: "int", nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    EntityVersion = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    DispatchedByUserId = table.Column<int>(type: "int", nullable: false),
                    DispatchedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CausationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Event_PK", x => x.Id);
                    table.ForeignKey(
                        name: "Event_CausationId_FK",
                        column: x => x.CausationId,
                        principalTable: "Event",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "Event_CorrelationId_FK",
                        column: x => x.CorrelationId,
                        principalTable: "Event",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "Event_DispatchedByUserId_FK",
                        column: x => x.DispatchedByUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Event_CausationId",
                table: "Event",
                column: "CausationId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_CorrelationId",
                table: "Event",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_DispatchedByUserId",
                table: "Event",
                column: "DispatchedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_EntityType_EntityId_EntityVersion",
                table: "Event",
                columns: new[] { "EntityType", "EntityId", "EntityVersion" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Event");
        }
    }
}
