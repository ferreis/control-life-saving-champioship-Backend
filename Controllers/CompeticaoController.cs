using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class CompeticaoController : BaseController<CompeticaoService>
{
    [HttpGet]
    public ActionResult<List<Competicao>> Get()
    {
        return Ok(_service.Listar());
    }

    [HttpPost]
    // Agora o POST recebe um CompeticaoDTO
    public IActionResult Post([FromBody] CompeticaoDTO competicaoDto)
    {
        // Mapeia o DTO para o modelo de domínio antes de passar para o serviço
        var competicao = new Competicao
        {
            Nome = competicaoDto.Nome,
            DataInicio = competicaoDto.DataInicio,
            DataFim = competicaoDto.DataFim,
            Local = competicaoDto.Local,
            Ano = competicaoDto.Ano
            // REMOVIDO: PaisId, EstadoId, CidadeId do mapeamento
        };

        _service.Adicionar(competicao);
        // Retorna 201 Created. O Location vazio ('') é comum para APIs simples
        // Em APIs mais complexas, seria a URL do novo recurso criado.
        return Created("", competicao);
    }
    
}