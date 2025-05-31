using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using SobrasaApi.Dtos;
using SobrasaApi.Models;
using SobrasaApi.Services;

[ApiController]
[Route("api/[controller]")]
public class ParticipacaoProvaController : BaseController<ParticipacaoProvaService>
{
    public ParticipacaoProvaController(ParticipacaoProvaService service) : base(service) { }

    [HttpGet]
    public ActionResult<List<ParticipacaoProva>> Get()
    {
        return Ok(_service.Listar());
    }

    [HttpPost]
    public IActionResult Post([FromBody] ParticipacaoProvaDTO participacaoDto)
    {
        var participacao = new ParticipacaoProva
        {
            AtletaId = participacaoDto.AtletaId,
            EquipeId = participacaoDto.EquipeId,
            ProvaId = participacaoDto.ProvaId,
            CompeticaoId = participacaoDto.CompeticaoId,
            // Converte string "HH:mm:ss" para TimeSpan?
            Tempo = string.IsNullOrEmpty(participacaoDto.Tempo) ? (TimeSpan?)null : TimeSpan.Parse(participacaoDto.Tempo),
            Colocacao = participacaoDto.Colocacao
        };
        _service.Adicionar(participacao);
        return Created("", participacao);
    }
}