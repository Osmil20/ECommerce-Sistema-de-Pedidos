using ECommerce.API.DTOs;
using ECommerce.API.Interfaces;
using ECommerce.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _service;
    private readonly IHistoricoService _historico;

    public PedidosController(IPedidoService service, IHistoricoService historico)
    {
        _service = service;
        _historico = historico;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PedidoResponseDto>>> Get([FromQuery] StatusPedido? status = null)
    {
        return Ok(await _service.ListarAsync(status));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PedidoResponseDto>> Get(int id)
    {
        var pedido = await _service.ObterPorIdAsync(id);
        return pedido is null ? NotFound() : Ok(pedido);
    }

    [HttpPost]
    public async Task<ActionResult<PedidoResponseDto>> Post([FromBody] PedidoCreateDto dto)
    {
        try
        {
            var criado = await _service.CriarAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = criado.Id }, criado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPatch("{id:int}/pagar")]
    public async Task<ActionResult<PedidoResponseDto>> Pagar(int id)
    {
        try
        {
            var pedido = await _service.PagarAsync(id);
            return pedido is null ? NotFound() : Ok(pedido);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPatch("{id:int}/cancelar")]
    public async Task<ActionResult<PedidoResponseDto>> Cancelar(int id)
    {
        try
        {
            var pedido = await _service.CancelarAsync(id);
            return pedido is null ? NotFound() : Ok(pedido);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("{id:int}/historico")]
    public async Task<ActionResult<IEnumerable<HistoricoResponseDto>>> GetHistorico(int id)
    {
        var historico = await _historico.ListarPorEntidadeAsync(TipoEntidade.Pedido, id);
        return Ok(historico);
    }
}
