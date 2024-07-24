using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StallosDotnetPleno.Domain.Entities;

public class TipoPessoa
{
    public int Id { get; set; }
    public string Tipo { get; set; } // PF for Pessoa Física and PJ for Pessoa Jurídica
}