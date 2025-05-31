using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using SobrasaApi.Dtos;
using SobrasaApi.Models;
using SobrasaApi.Services;

[ApiController]
[Route("api/[controller]")]
public class AtletaEquipeController : BaseController<AtletaEquipeService>
{
    public AtletaEquipeController(AtletaEquipeService service) : base(service) { }

    [HttpGet]
    public ActionResult<List<AtletaEquipe>> Get()
    {
        return Ok(_service.Listar());
    }

    [HttpPost]
    public IActionResult Post([FromBody] AtletaEquipeDTO atletaEquipeDto)
    {
        var atletaEquipe = new AtletaEquipe
        {
            AtletaId = atletaEquipeDto.AtletaId,
            EquipeId = atletaEquipeDto.EquipeId,
            AnoCompeticao = atletaEquipeDto.AnoCompeticao
        };
        _service.Adicionar(atletaEquipe);
        return Created("", atletaEquipe);
    }
}