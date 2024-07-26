using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StallosDotnetPleno.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_USER",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ClientSecret = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RosterId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RosterSecret = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RosterXApi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_USER", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_USER");
        }
    }
}
