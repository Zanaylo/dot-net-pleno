using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StallosDotnetPleno.Domain.ViewModels;

public class LoginView
{
    [StringLength(255)]
    public string Username { get; set; } = null!;
    [StringLength(255)]
    public string Password { get; set; } = null!;
}
