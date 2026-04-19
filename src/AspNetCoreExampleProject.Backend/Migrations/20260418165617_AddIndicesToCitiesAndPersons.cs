using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNetCoreExampleProject.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddIndicesToCitiesAndPersons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(name: "IX_Persons_FirstName", table: "Persons", column: "FirstName");

            migrationBuilder.CreateIndex(name: "IX_Cities_Name", table: "Cities", column: "Name", unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_Persons_FirstName", table: "Persons");

            migrationBuilder.DropIndex(name: "IX_Cities_Name", table: "Cities");
        }
    }
}
