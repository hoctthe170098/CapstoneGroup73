using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyFlow.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Lan19MigrationSuaThuocTnhChinhSach : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_ChinhSach_PhanTramGiam",
                table: "ChinhSach",
                sql: "[PhanTramGiam] > 0 AND [PhanTramGiam] <= 0.1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_ChinhSach_PhanTramGiam",
                table: "ChinhSach");
        }
    }
}
