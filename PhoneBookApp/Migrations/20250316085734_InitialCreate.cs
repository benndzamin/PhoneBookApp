﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PhoneBookApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "date", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contacts_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Contacts_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Bosna i Hercegovina" },
                    { 2, "Croatia" },
                    { 3, "Serbia" },
                    { 4, "Austria" },
                    { 5, "Germany" }
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "CountryId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Sarajevo" },
                    { 2, 1, "Mostar" },
                    { 3, 1, "Tuzla" },
                    { 4, 1, "Zenica" },
                    { 5, 2, "Zagreb" },
                    { 6, 2, "Split" },
                    { 7, 2, "Dubrovnik" },
                    { 8, 2, "Rijeka" },
                    { 9, 3, "Novi Sad" },
                    { 10, 3, "Beograd" },
                    { 11, 3, "Niš" },
                    { 12, 3, "Kragujevac" },
                    { 13, 4, "Wien" },
                    { 14, 4, "Graz" },
                    { 15, 4, "Linz" },
                    { 16, 4, "Salzburg" },
                    { 17, 5, "Berlin" },
                    { 18, 5, "Hamburg" },
                    { 19, 5, "München" },
                    { 20, 5, "Köln" }
                });

            migrationBuilder.InsertData(
                table: "Contacts",
                columns: new[] { "Id", "BirthDate", "CityId", "CountryId", "Email", "FirstName", "Gender", "LastName", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, new DateTime(1985, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, "johndoe@example.com", "John", 1, "Doe", "123/456-789" },
                    { 2, new DateTime(1990, 10, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 2, "janedoe@example.com", "Jane", 0, "Doe", "987/654-321" },
                    { 3, new DateTime(1980, 7, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 10, 3, "marksmith@example.com", "Mark", 1, "Smith", "555/555-555" },
                    { 4, new DateTime(1985, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 16, 4, "ibrahim@example.com", "Ibrahim", 1, "Hasić", "123/456-789" },
                    { 5, new DateTime(1990, 10, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 1, "amira@example.com", "Amira", 0, "Džanić", "987/654-321" },
                    { 6, new DateTime(1980, 7, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 2, "hasan@example.com", "Hasan", 1, "Selimović", "555/555-555" },
                    { 7, new DateTime(1992, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 14, 4, "elma@example.com", "Elma", 0, "Begović", "321/654-987" },
                    { 8, new DateTime(1989, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 20, 5, "mirza@example.com", "Mirza", 1, "Kovačević", "123/321-456" },
                    { 9, new DateTime(1995, 11, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 13, 4, "maja@example.com", "Maja", 0, "Avdagić", "654/987-321" },
                    { 10, new DateTime(1987, 4, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 2, "nedim@example.com", "Nedim", 1, "Salkanović", "987/123-654" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountryId",
                table: "Cities",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_CityId",
                table: "Contacts",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_CountryId",
                table: "Contacts",
                column: "CountryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
