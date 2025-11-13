using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarryJBriggs.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelWithDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "AboutPages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "AboutPages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
