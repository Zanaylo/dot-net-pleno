using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StallosDotnetPleno.Application.Interfaces;
using StallosDotnetPleno.Domain.Entities;
using StallosDotnetPleno.Domain.ViewModels;
using StallosDotnetPleno.Infrastructure.Context;

namespace StallosDotnetPleno.Api.Controllers;
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
    public async Task<IActionResult> CreatePessoa(PostPessoaView postPessoaView)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _pessoaService.CreatePessoaAsync(postPessoaView);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return CreatedAtAction(nameof(GetPessoa), new { id = result.Data }, null);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePessoa(int id, PostPessoaView postPessoaView)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _pessoaService.UpdatePessoaAsync(id, postPessoaView);
        if (!result.Success)
        {
            return NotFound(result.ErrorMessage);
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePessoa(int id)
    {
        var result = await _pessoaService.DeletePessoaAsync(id);
        if (!result.Success)
        {
            return NotFound(result.ErrorMessage);
        }

        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PessoaView>> GetPessoa(int id)
    {
        var result = await _pessoaService.GetPessoaAsync(id);
        if (!result.Success)
        {
            return NotFound(result.ErrorMessage);
        }

        return Ok(result.Data);
    }

    [HttpGet]
    public async Task<ActionResult<List<PessoaView>>> GetPessoas()
    {
        var result = await _pessoaService.GetPessoasAsync();
        return Ok(result.Data);
    }
}