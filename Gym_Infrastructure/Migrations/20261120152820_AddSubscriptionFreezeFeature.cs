using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gym_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriptionFreezeFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FreezeEndsAt",
                table: "Subscriptions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FreezeReason",
                table: "Subscriptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FrozenAt",
                table: "Subscriptions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalFrozenDays",
                table: "Subscriptions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FreezeEndsAt",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "FreezeReason",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "FrozenAt",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "TotalFrozenDays",
                table: "Subscriptions");
        }
    }
}
