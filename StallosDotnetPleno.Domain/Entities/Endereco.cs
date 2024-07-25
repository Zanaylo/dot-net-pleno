using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StallosDotnetPleno.Domain.Entities;

[Table("TB_ENDERECO")]
public class Endereco
{
    public int Id { get; set; }
    public string Cep { get; set; }
    public string Logradouro { get; set; }
    public string Numero { get; set; }
    public string Bairro { get; set; }
    public string Cidade { get; set; }
    public string Uf { get; set; }
    public List<PessoaEndereco> PessoaEnderecos { get; set; } = new List<PessoaEndereco>();
}