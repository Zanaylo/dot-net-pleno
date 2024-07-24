using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StallosDotnetPleno.Domain.Entities;

public class PessoaLista
{
    public int Id { get; set; }
    public int IdPessoa { get; set; }
    public string Lista { get; set; }
    public Pessoa Pessoa { get; set; }
}