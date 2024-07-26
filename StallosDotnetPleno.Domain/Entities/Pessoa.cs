using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StallosDotnetPleno.Domain.Entities;

[Table("TB_PESSOA")]
public class Pessoa
{
    public int Id { get; set; }
    public int IdTipoPessoa { get; set; }
    [StringLength(255)]
    public string Nome { get; set; }
    [StringLength(14)]
    public string Documento { get; set; }
    public TipoPessoa TipoPessoa { get; set; }
    public List<PessoaEndereco> PessoaEnderecos { get; set; } = new List<PessoaEndereco>();
    public List<PessoaLista> PessoaListas { get; set; } = new List<PessoaLista>();
}