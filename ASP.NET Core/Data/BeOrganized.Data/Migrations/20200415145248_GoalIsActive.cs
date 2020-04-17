﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace BeOrganized.Data.Migrations
{
    public partial class GoalIsActive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Goals",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Goals");
        }
    }
}
