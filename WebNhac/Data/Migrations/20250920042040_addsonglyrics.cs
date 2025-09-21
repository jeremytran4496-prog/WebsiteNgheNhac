using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebNhac.Data.Migrations
{
    /// <inheritdoc />
    public partial class addsonglyrics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Lyrics",
                table: "Songs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lyrics",
                table: "Songs");
        }
    }
}
