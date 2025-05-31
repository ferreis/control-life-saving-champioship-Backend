using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

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
        return Created($"api/atleta/{atleta.Id}", atleta);
    }

    [HttpPatch]
    public IActionResult Atualizar([FromBody] AtletaUpdateDto atleta)
    {
        var existente = _service.ObterPorId(atleta.Id);
        if (existente == null)
            return NotFound();

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

    [HttpPost("vincular")]
    public IActionResult Vincular([FromBody] AtletaEquipeDto dto)
    {
        try
        {
            if (dto.AtletaId == null || dto.EquipeId == null || dto.AnoCompeticao == null)
                return BadRequest("Todos os campos devem ser preenchidos.");

            var vinculo = new AtletaEquipe
            {
                AtletaId = dto.AtletaId.Value,
                EquipeId = dto.EquipeId.Value,
                AnoCompeticao = dto.AnoCompeticao.Value
            };

            _service.VincularAtletaEquipe(vinculo);
            return Ok("Atleta vinculado à equipe com sucesso.");
        }
        catch (Exception ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpGet("com-equipes")]
    public IActionResult GetComEquipe()
    {
        var lista = _service.ListarComEquipes();
        return Ok(lista);
    }

}
