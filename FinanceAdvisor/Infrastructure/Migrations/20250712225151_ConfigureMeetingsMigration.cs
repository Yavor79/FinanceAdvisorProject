using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceAdvisor.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureMeetingsMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_Advisors_AdvisorId",
                table: "Meetings");

            migrationBuilder.AlterColumn<Guid>(
                name: "AdvisorId",
                table: "Meetings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "Meetings",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_ClientId",
                table: "Meetings",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_Advisors_AdvisorId",
                table: "Meetings",
                column: "AdvisorId",
                principalTable: "Advisors",
                principalColumn: "AdvisorId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_AspNetUsers_ClientId",
                table: "Meetings",
                column: "ClientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_Advisors_AdvisorId",
                table: "Meetings");

            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_AspNetUsers_ClientId",
                table: "Meetings");

            migrationBuilder.DropIndex(
                name: "IX_Meetings_ClientId",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Meetings");

            migrationBuilder.AlterColumn<Guid>(
                name: "AdvisorId",
                table: "Meetings",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_Advisors_AdvisorId",
                table: "Meetings",
                column: "AdvisorId",
                principalTable: "Advisors",
                principalColumn: "AdvisorId");
        }
    }
}
