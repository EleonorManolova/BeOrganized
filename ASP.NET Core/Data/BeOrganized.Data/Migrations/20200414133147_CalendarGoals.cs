namespace BeOrganized.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class CalendarGoals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Habits_Calendars_CalendarId",
                table: "Habits");

            migrationBuilder.DropIndex(
                name: "IX_Habits_CalendarId",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "CalendarId",
                table: "Habits");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CalendarId",
                table: "Habits",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Habits_CalendarId",
                table: "Habits",
                column: "CalendarId");

            migrationBuilder.AddForeignKey(
                name: "FK_Habits_Calendars_CalendarId",
                table: "Habits",
                column: "CalendarId",
                principalTable: "Calendars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
