using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmentsService.Migrations
{
    /// <inheritdoc />
    public partial class NewMigrationV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "appointments",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "appointments",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "appointments",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "appointments",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "CustomerName",
                table: "appointments",
                newName: "customer_name");

            migrationBuilder.RenameColumn(
                name: "CustomerEmail",
                table: "appointments",
                newName: "customer_email");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "appointments",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "AppointmentDate",
                table: "appointments",
                newName: "appointment_date");

            migrationBuilder.AddColumn<Guid>(
                name: "appointment_id",
                table: "appointments",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "appointment_id",
                table: "appointments");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "appointments",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "appointments",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "appointments",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "appointments",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "customer_name",
                table: "appointments",
                newName: "CustomerName");

            migrationBuilder.RenameColumn(
                name: "customer_email",
                table: "appointments",
                newName: "CustomerEmail");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "appointments",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "appointment_date",
                table: "appointments",
                newName: "AppointmentDate");
        }
    }
}
