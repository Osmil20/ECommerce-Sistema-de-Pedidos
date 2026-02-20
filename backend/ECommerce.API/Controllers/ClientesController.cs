using ECommerce.API.DTOs;
using ECommerce.API.Interfaces;
using ECommerce.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _service;
    private readonly IHistoricoService _historico;

    public ClientesController(IClienteService service, IHistoricoService historico)
    {
        _service = service;
        _historico = historico;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClienteResponseDto>>> Get()
    {
        var clientes = await _service.ListarAsync();
        return Ok(clientes);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClienteResponseDto>> Get(int id)
    {
        var cliente = await _service.ObterPorIdAsync(id);
        return cliente is null ? NotFound() : Ok(cliente);
    }

    [HttpPost]
    public async Task<ActionResult<ClienteResponseDto>> Post([FromBody] ClienteCreateDto dto)
    {
        var criado = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = criado.Id }, criado);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ClienteResponseDto>> Put(int id, [FromBody] ClienteUpdateDto dto)
    {
        var atualizado = await _service.AtualizarAsync(id, dto);
        return atualizado is null ? NotFound() : Ok(atualizado);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var removido = await _service.RemoverAsync(id);
        return removido ? NoContent() : NotFound();
    }

    [HttpGet("{id:int}/historico")]
    public async Task<ActionResult<IEnumerable<HistoricoResponseDto>>> GetHistorico(int id)
    {
        var historico = await _historico.ListarPorEntidadeAsync(TipoEntidade.Cliente, id);
        return Ok(historico);
    }
}
