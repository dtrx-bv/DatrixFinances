using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatrixFinances.API.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "APIActivityLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateTimeOffset = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IPv4 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IPv6 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OriginPlatform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OriginCompany = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignedInCompanyUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusCode = table.Column<int>(type: "int", nullable: false),
                    StatusCodeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HttpMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ControllerMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ElapsedMilliseconds = table.Column<double>(type: "float", nullable: false),
                    Flag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentPayload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentPayloadContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceivedPayload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceivedPayloadContentType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APIActivityLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientSecret = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VendorApiKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bearer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Scope = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAuthorized = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "APIActivityLogs");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
