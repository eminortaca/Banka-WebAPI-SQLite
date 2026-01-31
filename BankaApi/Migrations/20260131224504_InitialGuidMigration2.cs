using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankaApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialGuidMigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KullaniciAdi",
                table: "Kullanicilar",
                newName: "Soyad");

            migrationBuilder.AddColumn<string>(
                name: "Ad",
                table: "Kullanicilar",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ad",
                table: "Kullanicilar");

            migrationBuilder.RenameColumn(
                name: "Soyad",
                table: "Kullanicilar",
                newName: "KullaniciAdi");
        }
    }
}
