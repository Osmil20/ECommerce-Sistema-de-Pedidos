using ECommerce.API.DTOs;
using ECommerce.API.Interfaces;
using ECommerce.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _service;
    private readonly IHistoricoService _historico;

    public ProdutosController(IProdutoService service, IHistoricoService historico)
    {
        _service = service;
        _historico = historico;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProdutoResponseDto>>> Get()
    {
        return Ok(await _service.ListarAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProdutoResponseDto>> Get(int id)
    {
        var produto = await _service.ObterPorIdAsync(id);
        return produto is null ? NotFound() : Ok(produto);
    }

    [HttpPost]
    public async Task<ActionResult<ProdutoResponseDto>> Post([FromBody] ProdutoCreateDto dto)
    {
        var criado = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = criado.Id }, criado);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProdutoResponseDto>> Put(int id, [FromBody] ProdutoUpdateDto dto)
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
        var historico = await _historico.ListarPorEntidadeAsync(TipoEntidade.Produto, id);
        return Ok(historico);
    }
}
