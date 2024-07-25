using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StallosDotnetPleno.Domain.Entities;

[Table("TB_PESSOA_ENDERECO")]
public class PessoaEndereco
{
    public int IdPessoa { get; set; }
    public int IdEndereco { get; set; }
    public Pessoa Pessoa { get; set; }
    public Endereco Endereco { get; set; }
}