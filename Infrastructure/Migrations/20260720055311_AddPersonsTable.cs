using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM bookings;");
            migrationBuilder.Sql("DELETE FROM events;");

            migrationBuilder.AddColumn<Guid>(
                name: "PersonId",
                table: "bookings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "persons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Login = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_persons", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_bookings_PersonId",
                table: "bookings",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_persons_Login",
                table: "persons",
                column: "Login",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_bookings_persons_PersonId",
                table: "bookings",
                column: "PersonId",
                principalTable: "persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bookings_persons_PersonId",
                table: "bookings");

            migrationBuilder.DropTable(
                name: "persons");

            migrationBuilder.DropIndex(
                name: "IX_bookings_PersonId",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "bookings");
        }
    }
}
