using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAllTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    MobileNumber = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    PatientName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PatientSex = table.Column<string>(type: "char(1)", maxLength: 1, nullable: false),
                    PatientBirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PatientSize = table.Column<long>(type: "bigint", nullable: true),
                    PatientWeight = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                    table.CheckConstraint("CK_Patient_PatientSex", "PatientSex IN ('M', 'F')");
                    table.CheckConstraint("CK_Patient_PatientSize", "PatientSize IS NULL OR PatientSize >= 0");
                    table.CheckConstraint("CK_Patient_PatientWeight", "PatientWeight IS NULL OR PatientWeight >= 0");
                    table.ForeignKey(
                        name: "FK_Patients_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Studies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    StudyInstanceUID = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    StudyDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StudyDescription = table.Column<string>(type: "varchar(256)", maxLength: 64, nullable: true),
                    StudyTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    StudyId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Studies", x => x.Id);
                    table.CheckConstraint("CK_Study_StudyId", "StudyId IS NULL OR StudyId >= 0");
                    table.ForeignKey(
                        name: "FK_Studies_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Series",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudyId = table.Column<int>(type: "int", nullable: false),
                    SeriesInstanceUID = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    SeriesNumber = table.Column<long>(type: "bigint", nullable: false),
                    Modality = table.Column<string>(type: "varchar(32)", maxLength: 16, nullable: false),
                    SeriesDescription = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true),
                    SeriesTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Series", x => x.Id);
                    table.CheckConstraint("CK_Series_SeriesNumber", "SeriesNumber >= 0");
                    table.ForeignKey(
                        name: "FK_Series_Studies_StudyId",
                        column: x => x.StudyId,
                        principalTable: "Studies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeriesId = table.Column<int>(type: "int", nullable: false),
                    SOPInstanceUID = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    InstanceNumber = table.Column<long>(type: "bigint", nullable: false),
                    Rows = table.Column<long>(type: "bigint", nullable: false),
                    Columns = table.Column<long>(type: "bigint", nullable: false),
                    BitsAllocated = table.Column<long>(type: "bigint", nullable: false),
                    SOPClassUID = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    AcquisitionDate = table.Column<DateOnly>(type: "date", nullable: true),
                    AcquisitionTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                    table.CheckConstraint("CK_Image_BitsAllocated", "BitsAllocated >= 0");
                    table.CheckConstraint("CK_Image_Columns", "Columns >= 0");
                    table.CheckConstraint("CK_Image_InstanceNumber", "InstanceNumber >= 0");
                    table.CheckConstraint("CK_Image_Rows", "Rows >= 0");
                    table.ForeignKey(
                        name: "FK_Images_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Images_SeriesId",
                table: "Images",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_PatientId",
                table: "Patients",
                column: "PatientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_PersonId",
                table: "Patients",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_Email",
                table: "Persons",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_MobileNumber",
                table: "Persons",
                column: "MobileNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Series_StudyId",
                table: "Series",
                column: "StudyId");

            migrationBuilder.CreateIndex(
                name: "IX_Studies_PatientId",
                table: "Studies",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Series");

            migrationBuilder.DropTable(
                name: "Studies");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "Persons");
        }
    }
}
