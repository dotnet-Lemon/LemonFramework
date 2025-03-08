using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LemonFramework.Core.Migrations
{
    /// <inheritdoc />
    public partial class init_25030801 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "用户Guid"),
                    UserNaame = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "用户名"),
                    RealName = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "真实名称"),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "联系方式"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "创建时间"),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "修改时间"),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: true, comment: "是否启用"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true, comment: "是否删除")
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
                name: "Users");
        }
    }
}
