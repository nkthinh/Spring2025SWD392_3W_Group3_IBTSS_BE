using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBTSS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class FixRelationof_Transaction_Ticket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Tickets_TicketId",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "TicketId",
                table: "Transactions",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_TicketId",
                table: "Transactions",
                newName: "IX_Transactions_CustomerId");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Transactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "Tickets",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TransactionId",
                table: "Tickets",
                column: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Transactions_TransactionId",
                table: "Tickets",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "TransactionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Customers_CustomerId",
                table: "Transactions",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Transactions_TransactionId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Customers_CustomerId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_TransactionId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Transactions",
                newName: "TicketId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_CustomerId",
                table: "Transactions",
                newName: "IX_Transactions_TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Tickets_TicketId",
                table: "Transactions",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "TicketId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
