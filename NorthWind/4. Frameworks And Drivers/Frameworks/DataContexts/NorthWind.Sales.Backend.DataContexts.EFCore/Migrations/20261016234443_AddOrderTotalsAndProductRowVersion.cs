using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NorthWind.Sales.Backend.DataContexts.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderTotalsAndProductRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Products",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            // SQL Server requires dropping the foreign key before
            // altering the columns it references.
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "Orders",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nchar(5)",
                oldFixedLength: true,
                oldMaxLength: 5);

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "Orders",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Tax",
                table: "Orders",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "Orders",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            // SQL Server requires dropping the primary key before
            // altering the column that conforms it.
            migrationBuilder.DropPrimaryKey(
                name: "PK_Customers",
                table: "Customers");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Customers",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nchar(5)",
                oldFixedLength: true,
                oldMaxLength: 5);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customers",
                table: "Customers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Subtotal",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Tax",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "Orders",
                type: "nchar(5)",
                fixedLength: true,
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(13)",
                oldMaxLength: 13);

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customers",
                table: "Customers");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Customers",
                type: "nchar(5)",
                fixedLength: true,
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(13)",
                oldMaxLength: 13);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customers",
                table: "Customers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
