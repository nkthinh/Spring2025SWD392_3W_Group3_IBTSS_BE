using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBTSS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Improvelogic_in_LocationRoute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "StopDuration",
                table: "LocationRoutes",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StopOrder",
                table: "LocationRoutes",
                type: "int",
                nullable: true,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StopDuration",
                table: "LocationRoutes");

            migrationBuilder.DropColumn(
                name: "StopOrder",
                table: "LocationRoutes");
        }
    }
}
