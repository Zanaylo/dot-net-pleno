using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StallosDotnetPleno.Domain.Entities;

[Table("TB_ENDERECO")]
public class Endereco
{
    public int Id { get; set; }
    [StringLength(8)]
    public string Cep { get; set; }
    [StringLength(255)]
    public string Logradouro { get; set; }
    [StringLength(8)]
    public string Numero { get; set; }
    [StringLength(255)]
    public string Bairro { get; set; }
    [StringLength(255)]
    public string Cidade { get; set; }
    [StringLength(2, ErrorMessage = "Informe apenas as Inciais")]
    public string Uf { get; set; }
    public List<PessoaEndereco> PessoaEnderecos { get; set; } = new List<PessoaEndereco>();
}