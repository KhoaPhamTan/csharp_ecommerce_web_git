using Microsoft.EntityFrameworkCore.Migrations;

public partial class UpdateUserIdInCartItems : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "UserId",
            table: "CartItems",
            type: "nvarchar(450)",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<string>(
            name: "Id",
            table: "Users",
            type: "nvarchar(450)",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<int>(
            name: "UserId",
            table: "CartItems",
            type: "int",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(450)");

        migrationBuilder.AlterColumn<int>(
            name: "Id",
            table: "Users",
            type: "int",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(450)");
    }
}