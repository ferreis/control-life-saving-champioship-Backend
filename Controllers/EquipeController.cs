using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using SobrasaApi.Dtos;
using SobrasaApi.Models;
using SobrasaApi.Services;

[ApiController]
[Route("api/[controller]")]
public class EquipeController : BaseController<EquipeService>
{
    public EquipeController(EquipeService service) : base(service) { }

    [HttpGet]
    public ActionResult<List<Equipe>> Get()
    {
        return Ok(_service.Listar());
    }

    [HttpPost]
    public IActionResult Post([FromBody] EquipeDTO equipeDto)
    {
        var equipe = new Equipe
        {
            Nome = equipeDto.Nome,
            Tipo = equipeDto.Tipo,
            Estado = equipeDto.Estado,
            Nacionalidade = equipeDto.Nacionalidade
        };
        _service.Adicionar(equipe);
        return Created("", equipe);
    }
}