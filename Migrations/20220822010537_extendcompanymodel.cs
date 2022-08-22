using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JATS.Migrations
{
    public partial class extendcompanymodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "CompanyBG",
                table: "Companies",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyBGFileContentType",
                table: "Companies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyBGFileName",
                table: "Companies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Companies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Companies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SocialMediaFacebook",
                table: "Companies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SocialMediaInstagram",
                table: "Companies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SocialMediaTwitter",
                table: "Companies",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyBG",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "CompanyBGFileContentType",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "CompanyBGFileName",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "SocialMediaFacebook",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "SocialMediaInstagram",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "SocialMediaTwitter",
                table: "Companies");
        }
    }
}
