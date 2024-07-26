using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StallosDotnetPleno.Domain.Entities;

[Table("TB_USER")]
public class User
{
    public int Id { get; set; }
    [StringLength(255)]
    public string ClientId { get; set; }
    [StringLength(255)]
    public string ClientSecret { get; set; }
    [StringLength(255)]
    public string RosterId { get; set; }
    [StringLength(255)]
    public string RosterSecret { get; set; }
    [StringLength(255)]
    public string RosterXApi { get; set; }
}
