using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankaApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialGuidMigration3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HesapId",
                table: "Islemler",
                newName: "KullaniciId");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Islemler",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "IslemTuru",
                table: "Islemler",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IslemTuru",
                table: "Islemler");

            migrationBuilder.RenameColumn(
                name: "KullaniciId",
                table: "Islemler",
                newName: "HesapId");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Islemler",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);
        }
    }
}
