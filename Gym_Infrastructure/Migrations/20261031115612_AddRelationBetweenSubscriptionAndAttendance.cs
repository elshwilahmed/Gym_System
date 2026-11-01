using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gym_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationBetweenSubscriptionAndAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AttendanceId",
                table: "Subscriptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionId",
                table: "Attendances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_SubscriptionId",
                table: "Attendances",
                column: "SubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Subscriptions_SubscriptionId",
                table: "Attendances",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Subscriptions_SubscriptionId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_SubscriptionId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "AttendanceId",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "SubscriptionId",
                table: "Attendances");
        }
    }
}
