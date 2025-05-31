using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using SobrasaApi.Dtos;
using SobrasaApi.Models;
using SobrasaApi.Services;

[ApiController]
[Route("api/[controller]")]
public class PontuacaoController : BaseController<PontuacaoService>
{
    public PontuacaoController(PontuacaoService service) : base(service) { }

    [HttpGet]
    public ActionResult<List<Pontuacao>> Get()
    {
        return Ok(_service.Listar());
    }

    [HttpPost]
    public IActionResult Post([FromBody] PontuacaoDTO pontuacaoDto)
    {
        var pontuacao = new Pontuacao
        {
            ParticipacaoProvaId = pontuacaoDto.ParticipacaoProvaId,
            Pontos = pontuacaoDto.Pontos
        };
        _service.Adicionar(pontuacao);
        return Created("", pontuacao);
    }
}