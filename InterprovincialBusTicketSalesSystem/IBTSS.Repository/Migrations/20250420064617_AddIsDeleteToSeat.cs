using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBTSS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeleteToSeat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Seats",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Seats");
        }
    }
}
