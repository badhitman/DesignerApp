using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext028_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WorkScheduleBaseModelDB_StartPart_EndPart",
                table: "WorkScheduleBaseModelDB",
                columns: new[] { "StartPart", "EndPart" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkScheduleBaseModelDB_StartPart_EndPart",
                table: "WorkScheduleBaseModelDB");
        }
    }
}
