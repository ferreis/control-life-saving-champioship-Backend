using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using SobrasaApi.Dtos;


[ApiController]
[Route("api/[controller]")]
public class AtletaController : BaseController<AtletaService>
{

    [HttpGet]
    public ActionResult<List<Atleta>> Get()
    {
        return Ok(_service.Listar());
    }

    [HttpPost]
    public IActionResult Post([FromBody] Atleta atleta)
    {
        _service.Adicionar(atleta);
        return Created("", atleta);
    }
    
    [HttpPatch]
    public IActionResult Atualizar([FromBody] AtletaUpdateDto atleta)
    {
        var existente = _service.ObterPorId(atleta.Id);
        if (existente == null)
            return NotFound();

        // Atualiza apenas os campos enviados
        existente.Nome = atleta.Nome ?? existente.Nome;
        existente.Cpf = atleta.Cpf ?? existente.Cpf;
        existente.Genero = atleta.Genero ?? existente.Genero;
        existente.DataNascimento = atleta.DataNascimento ?? existente.DataNascimento;
        existente.Nacionalidade = atleta.Nacionalidade ?? existente.Nacionalidade;

        _service.Atualizar(existente);
        return Ok(existente);
    }
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var atleta = _service.ObterPorId(id);
        if (atleta == null)
            return NotFound();

        _service.Deletar(atleta.Id);
        return NoContent();
    }

}
