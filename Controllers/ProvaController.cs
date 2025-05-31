using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using SobrasaApi.Dtos;
using SobrasaApi.Models;
using SobrasaApi.Services;

[ApiController]
[Route("api/[controller]")]
public class ProvaController : BaseController<ProvaService>
{
    public ProvaController(ProvaService service) : base(service) { }

    [HttpGet]
    public ActionResult<List<Prova>> Get()
    {
        return Ok(_service.Listar());
    }

    [HttpPost]
    public IActionResult Post([FromBody] ProvaDTO provaDto)
    {
        var prova = new Prova
        {
            Nome = provaDto.Nome,
            Tipo = provaDto.Tipo,
            Modalidade = provaDto.Modalidade,
            TempoOuColocacao = provaDto.TempoOuColocacao,
            Genero = provaDto.Genero,
            CategoriaEtaria = provaDto.CategoriaEtaria
        };
        _service.Adicionar(prova);
        return Created("", prova);
    }
}