using StallosDotnetPleno.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StallosDotnetPleno.Domain.ViewModels;

public class EnderecoView
{
    [Required(ErrorMessage = "Campo Cep é requerido")]
    public string Cep { get; set; } = null!;
    [Required(ErrorMessage = "Campo nome Logradouro é requerido")]
    public string Logradouro { get; set; } = null!;
    [Required(ErrorMessage = "Campo nome Número é requerido")]
    public string Numero { get; set; } = null!;

    [Required(ErrorMessage = "Campo nome Bairro é requerido")]
    public string Bairro { get; set; } = null!;

    [Required(ErrorMessage = "Campo nome Cidade é requerido")]
    public string Cidade { get; set; } = null!;

    [Required(ErrorMessage = "Campo nome Uf é requerido")]
    [StringLength(2, ErrorMessage = "Informe apenas as iniciais.")]
    public string Uf { get; set; } = null!;
}
