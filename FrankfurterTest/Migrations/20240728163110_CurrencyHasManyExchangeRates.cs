using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FrankfurterTest.Migrations
{
    /// <inheritdoc />
    public partial class CurrencyHasManyExchangeRates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Symbol = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExchangesRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaseCurrencyId = table.Column<int>(type: "int", nullable: false),
                    TargetCurrencyId = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangesRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangesRates_Currencies_BaseCurrencyId",
                        column: x => x.BaseCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExchangesRates_Currencies_TargetCurrencyId",
                        column: x => x.TargetCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExchangesRates_BaseCurrencyId",
                table: "ExchangesRates",
                column: "BaseCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangesRates_TargetCurrencyId",
                table: "ExchangesRates",
                column: "TargetCurrencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExchangesRates");

            migrationBuilder.DropTable(
                name: "Currencies");
        }
    }
}
