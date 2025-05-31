using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EquipeController : BaseController<EquipeService>
{
    [HttpGet]
    public IActionResult Get() => Ok(_service.Listar());

    [HttpPost]
    public IActionResult Post([FromBody] EquipeDto dto)
    {
        var equipe = new Equipe
        {
            Nome = dto.Nome,
            Tipo = dto.Tipo,
            Estado = dto.Estado,
            Nacionalidade = dto.Nacionalidade
        };
        _service.Adicionar(equipe);
        return Created("", equipe);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _service.Deletar(id);
        return NoContent();
    }

    [HttpPatch("{id}")]
    public IActionResult Patch(int id, [FromBody] EquipeDto dto)
    {
        _service.AtualizarParcial(id, dto);
        return NoContent();
    }
}
