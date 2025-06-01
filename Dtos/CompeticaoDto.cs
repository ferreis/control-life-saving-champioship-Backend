public class CompeticaoDTO
{
  public string Nome { get; set; } = string.Empty;
  public DateTime? DataInicio { get; set; }
  public DateTime? DataFim { get; set; }
  public string? Local { get; set; } // Mantém Local
  public int Ano { get; set; }

}