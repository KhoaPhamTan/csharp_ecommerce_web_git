using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetStoreAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePetTypeName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PetTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Insect");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PetTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Insert");
        }
    }
}
