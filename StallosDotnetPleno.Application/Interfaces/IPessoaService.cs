using StallosDotnetPleno.Application.ResultObject;
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
    Task<OperationResult<int>> CreatePessoaAsync(PostPessoaView postPessoaView);
    Task<OperationResult<PessoaView>> GetPessoaAsync(int id);
    Task<OperationResult<List<PessoaView>>> GetPessoasAsync();
    Task<OperationResult> UpdatePessoaAsync(int id, PostPessoaView postPessoaView);
    Task<OperationResult> DeletePessoaAsync(int id);
}