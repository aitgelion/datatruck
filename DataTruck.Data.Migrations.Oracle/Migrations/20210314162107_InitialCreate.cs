using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataTruck.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "datatruck");

            migrationBuilder.CreateTable(
                name: "Files",
                schema: "datatruck",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "1, 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Hash = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files",
                schema: "datatruck");
        }
    }
}
