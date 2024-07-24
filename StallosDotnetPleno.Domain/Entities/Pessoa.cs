using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StallosDotnetPleno.Domain.Entities;

public class Pessoa
{
    public int Id { get; set; }
    public int IdTipoPessoa { get; set; }
    public string Nome { get; set; }
    public string Documento { get; set; }
    public TipoPessoa TipoPessoa { get; set; }
    public List<PessoaEndereco> PessoaEnderecos { get; set; } = new List<PessoaEndereco>();
    public List<PessoaLista> PessoaListas { get; set; } = new List<PessoaLista>();
}