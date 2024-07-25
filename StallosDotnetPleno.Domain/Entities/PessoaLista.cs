using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StallosDotnetPleno.Domain.Entities;

[Table("TB_PESSOA_LISTA")]
public class PessoaLista
{
    public int Id { get; set; }
    public int IdPessoa { get; set; }
    public string Lista { get; set; }
    public Pessoa Pessoa { get; set; }
}