using StallosDotnetPleno.Domain.Entities;
using StallosDotnetPleno.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StallosDotnetPleno.Application.Interfaces;

public interface IPessoaService
{
    Task<PessoaView> GetPessoaAsync(int id);
    Task<List<PessoaView>> GetPessoasAsync();
    Task<int> CreatePessoaAsync(PostPessoaView postPessoaView);
    Task UpdatePessoaAsync(int id, PostPessoaView postPessoaView);
    Task DeletePessoaAsync(int id);
}