using StallosDotnetPleno.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StallosDotnetPleno.Domain.ViewModels;

public class PessoaView
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string TipoPessoa { get; set; } = null!;
    public string Documento { get; set; } = null!;
    public List<EnderecoView> Enderecos { get; set; } = new List<EnderecoView>();
    public List<string> Lista { get; set; } = new List<string>();

}
