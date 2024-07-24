using Microsoft.AspNetCore.Mvc;
using StallosDotnetPleno.Application.Services.Interfaces;
using StallosDotnetPleno.Domain.Entities;
using StallosDotnetPleno.Infrastructure.Context;

namespace StallosDotnetPleno.Api.Controllers;

[ApiController]
[Route("pessoa")]
public class PessoaController : ControllerBase
{
    private readonly IPessoaService _pessoaService;

    public PessoaController(IPessoaService pessoaService)
    {
        _pessoaService = pessoaService;
    }

    [HttpPost]
    public async Task<IActionResult> AddPessoa([FromBody] Pessoa pessoa)
    {
        try
        {
            await _pessoaService.AddPessoaAsync(pessoa);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}