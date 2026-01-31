using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankaApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialGuidMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Islemler_Hesaplar_HesapId",
                table: "Islemler");

            migrationBuilder.DropIndex(
                name: "IX_Islemler_HesapId",
                table: "Islemler");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Kullanicilar",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "HesapNo",
                table: "Hesaplar",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "KullaniciId",
                table: "Hesaplar",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HesapNo",
                table: "Hesaplar");

            migrationBuilder.DropColumn(
                name: "KullaniciId",
                table: "Hesaplar");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Kullanicilar",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.CreateIndex(
                name: "IX_Islemler_HesapId",
                table: "Islemler",
                column: "HesapId");

            migrationBuilder.AddForeignKey(
                name: "FK_Islemler_Hesaplar_HesapId",
                table: "Islemler",
                column: "HesapId",
                principalTable: "Hesaplar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
