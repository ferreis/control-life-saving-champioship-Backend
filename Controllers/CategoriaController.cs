using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using SobrasaApi.Dtos;
using SobrasaApi.Models;
using SobrasaApi.Services;

[ApiController]
[Route("api/[controller]")]
public class CategoriaController : BaseController<CategoriaService>
{
    public CategoriaController(CategoriaService service) : base(service) { }

    [HttpGet]
    public ActionResult<List<Categoria>> Get()
    {
        return Ok(_service.Listar());
    }

    [HttpPost]
    public IActionResult Post([FromBody] CategoriaDTO categoriaDto)
    {
        var categoria = new Categoria
        {
            Descricao = categoriaDto.Descricao,
            IdadeMin = categoriaDto.IdadeMin,
            IdadeMax = categoriaDto.IdadeMax
        };
        _service.Adicionar(categoria);
        return Created("", categoria);
    }
}