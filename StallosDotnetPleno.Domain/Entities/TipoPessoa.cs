using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StallosDotnetPleno.Domain.Entities;

[Table("TB_TIPO_PESSOA")]
public class TipoPessoa
{
    public int Id { get; set; }
    [StringLength(2)]
    public string Tipo { get; set; } // PF | PJ
}