using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StallosDotnetPleno.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTablesNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PessoaEnderecos_Enderecos_IdEndereco",
                table: "PessoaEnderecos");

            migrationBuilder.DropForeignKey(
                name: "FK_PessoaEnderecos_Pessoas_IdPessoa",
                table: "PessoaEnderecos");

            migrationBuilder.DropForeignKey(
                name: "FK_PessoaListas_Pessoas_IdPessoa",
                table: "PessoaListas");

            migrationBuilder.DropForeignKey(
                name: "FK_Pessoas_TipoPessoas_IdTipoPessoa",
                table: "Pessoas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TipoPessoas",
                table: "TipoPessoas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pessoas",
                table: "Pessoas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PessoaListas",
                table: "PessoaListas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PessoaEnderecos",
                table: "PessoaEnderecos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Enderecos",
                table: "Enderecos");

            migrationBuilder.RenameTable(
                name: "TipoPessoas",
                newName: "TB_TIPO_PESSOA");

            migrationBuilder.RenameTable(
                name: "Pessoas",
                newName: "TB_PESSOA");

            migrationBuilder.RenameTable(
                name: "PessoaListas",
                newName: "TB_PESSOA_LISTA");

            migrationBuilder.RenameTable(
                name: "PessoaEnderecos",
                newName: "TB_PESSOA_ENDERECO");

            migrationBuilder.RenameTable(
                name: "Enderecos",
                newName: "TB_ENDERECO");

            migrationBuilder.RenameIndex(
                name: "IX_Pessoas_IdTipoPessoa",
                table: "TB_PESSOA",
                newName: "IX_TB_PESSOA_IdTipoPessoa");

            migrationBuilder.RenameIndex(
                name: "IX_PessoaListas_IdPessoa",
                table: "TB_PESSOA_LISTA",
                newName: "IX_TB_PESSOA_LISTA_IdPessoa");

            migrationBuilder.RenameIndex(
                name: "IX_PessoaEnderecos_IdEndereco",
                table: "TB_PESSOA_ENDERECO",
                newName: "IX_TB_PESSOA_ENDERECO_IdEndereco");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TB_TIPO_PESSOA",
                table: "TB_TIPO_PESSOA",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TB_PESSOA",
                table: "TB_PESSOA",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TB_PESSOA_LISTA",
                table: "TB_PESSOA_LISTA",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TB_PESSOA_ENDERECO",
                table: "TB_PESSOA_ENDERECO",
                columns: new[] { "IdPessoa", "IdEndereco" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TB_ENDERECO",
                table: "TB_ENDERECO",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TB_PESSOA_TB_TIPO_PESSOA_IdTipoPessoa",
                table: "TB_PESSOA",
                column: "IdTipoPessoa",
                principalTable: "TB_TIPO_PESSOA",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TB_PESSOA_ENDERECO_TB_ENDERECO_IdEndereco",
                table: "TB_PESSOA_ENDERECO",
                column: "IdEndereco",
                principalTable: "TB_ENDERECO",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TB_PESSOA_ENDERECO_TB_PESSOA_IdPessoa",
                table: "TB_PESSOA_ENDERECO",
                column: "IdPessoa",
                principalTable: "TB_PESSOA",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TB_PESSOA_LISTA_TB_PESSOA_IdPessoa",
                table: "TB_PESSOA_LISTA",
                column: "IdPessoa",
                principalTable: "TB_PESSOA",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TB_PESSOA_TB_TIPO_PESSOA_IdTipoPessoa",
                table: "TB_PESSOA");

            migrationBuilder.DropForeignKey(
                name: "FK_TB_PESSOA_ENDERECO_TB_ENDERECO_IdEndereco",
                table: "TB_PESSOA_ENDERECO");

            migrationBuilder.DropForeignKey(
                name: "FK_TB_PESSOA_ENDERECO_TB_PESSOA_IdPessoa",
                table: "TB_PESSOA_ENDERECO");

            migrationBuilder.DropForeignKey(
                name: "FK_TB_PESSOA_LISTA_TB_PESSOA_IdPessoa",
                table: "TB_PESSOA_LISTA");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TB_TIPO_PESSOA",
                table: "TB_TIPO_PESSOA");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TB_PESSOA_LISTA",
                table: "TB_PESSOA_LISTA");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TB_PESSOA_ENDERECO",
                table: "TB_PESSOA_ENDERECO");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TB_PESSOA",
                table: "TB_PESSOA");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TB_ENDERECO",
                table: "TB_ENDERECO");

            migrationBuilder.RenameTable(
                name: "TB_TIPO_PESSOA",
                newName: "TipoPessoas");

            migrationBuilder.RenameTable(
                name: "TB_PESSOA_LISTA",
                newName: "PessoaListas");

            migrationBuilder.RenameTable(
                name: "TB_PESSOA_ENDERECO",
                newName: "PessoaEnderecos");

            migrationBuilder.RenameTable(
                name: "TB_PESSOA",
                newName: "Pessoas");

            migrationBuilder.RenameTable(
                name: "TB_ENDERECO",
                newName: "Enderecos");

            migrationBuilder.RenameIndex(
                name: "IX_TB_PESSOA_LISTA_IdPessoa",
                table: "PessoaListas",
                newName: "IX_PessoaListas_IdPessoa");

            migrationBuilder.RenameIndex(
                name: "IX_TB_PESSOA_ENDERECO_IdEndereco",
                table: "PessoaEnderecos",
                newName: "IX_PessoaEnderecos_IdEndereco");

            migrationBuilder.RenameIndex(
                name: "IX_TB_PESSOA_IdTipoPessoa",
                table: "Pessoas",
                newName: "IX_Pessoas_IdTipoPessoa");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TipoPessoas",
                table: "TipoPessoas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PessoaListas",
                table: "PessoaListas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PessoaEnderecos",
                table: "PessoaEnderecos",
                columns: new[] { "IdPessoa", "IdEndereco" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pessoas",
                table: "Pessoas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Enderecos",
                table: "Enderecos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PessoaEnderecos_Enderecos_IdEndereco",
                table: "PessoaEnderecos",
                column: "IdEndereco",
                principalTable: "Enderecos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PessoaEnderecos_Pessoas_IdPessoa",
                table: "PessoaEnderecos",
                column: "IdPessoa",
                principalTable: "Pessoas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PessoaListas_Pessoas_IdPessoa",
                table: "PessoaListas",
                column: "IdPessoa",
                principalTable: "Pessoas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pessoas_TipoPessoas_IdTipoPessoa",
                table: "Pessoas",
                column: "IdTipoPessoa",
                principalTable: "TipoPessoas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
