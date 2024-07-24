using StallosDotnetPleno.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StallosDotnetPleno.Application.Services.Interfaces
{
    public interface IPessoaService
    {
        Task<bool> AddPessoaAsync(Pessoa pessoa);



    }
}
