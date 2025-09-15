using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dicomAPIs.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DicomRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PatientName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StudyInstanceUID = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SeriesInstanceUID = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SOPInstanceUID = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Modality = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StudyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SavedFilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DicomRecords", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DicomRecords");
        }
    }
}
