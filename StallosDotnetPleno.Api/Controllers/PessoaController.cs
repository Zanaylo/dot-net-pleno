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

        var id = await _pessoaService.CreatePessoaAsync(postPessoaView);
        return CreatedAtAction(nameof(GetPessoa), new { id }, null);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePessoa(int id, PostPessoaView postPessoaView)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {

            await _pessoaService.UpdatePessoaAsync(id, postPessoaView);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePessoa(int id)
    {
        try
        {
            await _pessoaService.DeletePessoaAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PessoaView>> GetPessoa(int id)
    {
        try
        {
            var pessoa = await _pessoaService.GetPessoaAsync(id);
            return Ok(pessoa);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<PessoaView>>> GetPessoas()
    {
        var pessoas = await _pessoaService.GetPessoasAsync();
        return Ok(pessoas);
    }

}