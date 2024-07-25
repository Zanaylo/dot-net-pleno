using StallosDotnetPleno.Domain.Entities;
using StallosDotnetPleno.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StallosDotnetPleno.Domain.ViewModels;

public class PostPessoaView
{
    [Required(ErrorMessage = "Campo nome é requerido")]
    public string Nome { get; set; } = null!;

    [Required(ErrorMessage = "Campo TipoPessoa é requerido")]
    [RegularExpression(@"^(PF|PJ|pf|pj)$", ErrorMessage = "Apenas tipo PF ou PJ")]
    public string TipoPessoa { get; set; } = null!;

    [Required(ErrorMessage = "Campo Documento é requerido")]
    [StringLength(18, MinimumLength = 11, ErrorMessage = "Informe todos os números do CPF ou CNPJ")]
    public string Documento { get; set; } = null!;

    [RequiredList]
    public List<EnderecoView> Enderecos { get; set; } = new List<EnderecoView>();
}