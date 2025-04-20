using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBTSS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class RenameIsDeleteToIsBooked : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDelete",
                table: "Seats",
                newName: "IsBooked");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsBooked",
                table: "Seats",
                newName: "IsDelete");
        }
    }
}
