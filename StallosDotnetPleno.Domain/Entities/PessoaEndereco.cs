using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StallosDotnetPleno.Domain.Entities;

public class PessoaEndereco
{
    public int IdPessoa { get; set; }
    public int IdEndereco { get; set; }
    public Pessoa Pessoa { get; set; }
    public Endereco Endereco { get; set; }
}