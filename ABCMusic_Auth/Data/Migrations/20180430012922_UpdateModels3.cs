using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ABCMusic_Auth.Data.Migrations
{
    public partial class UpdateModels3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CreateUserViewModel",
                columns: table => new
                {
                    UserName = table.Column<string>(nullable: false),
                    Age = table.Column<byte>(nullable: false),
                    ConfirmPassword = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    Gender = table.Column<string>(maxLength: 10, nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    Password = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreateUserViewModel", x => x.UserName);
                });

            migrationBuilder.CreateTable(
                name: "EditUserViewModel",
                columns: table => new
                {
                    UserName = table.Column<string>(nullable: false),
                    Age = table.Column<byte>(nullable: false),
                    ConfirmPassword = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    Gender = table.Column<string>(maxLength: 10, nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    Password = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EditUserViewModel", x => x.UserName);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreateUserViewModel");

            migrationBuilder.DropTable(
                name: "EditUserViewModel");
        }
    }
}
