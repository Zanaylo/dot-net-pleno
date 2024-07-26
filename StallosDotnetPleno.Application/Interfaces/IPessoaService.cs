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
    Task<OperationResult> CreatePessoaAsync(PostPessoaView postPessoaView);
    Task<OperationResult> GetPessoaAsync(int id);
    Task<OperationResult> GetPessoasAsync(int pageNumber, int pageSize);
    Task<OperationResult> UpdatePessoaAsync(int id, PostPessoaView postPessoaView);
    Task<OperationResult> DeletePessoaAsync(int id);
}