public class AtletaEquipeDto
{
    public int? Id { get; set; }
    public int? AtletaId { get; set; }
    public int? EquipeId { get; set; }
    public int? AnoCompeticao { get; set; }
}
public class AtletaComEquipeDto
{
    public int AtletaId { get; set; }
    public string Nome { get; set; }
    public string Cpf { get; set; }
    public string Genero { get; set; }
    public DateTime DataNascimento { get; set; }
    public int PaisId { get; set; }
    public string? Nacionalidade { get; set; }
    public string? EquipeNome { get; set; }
    public string? EquipeTipo { get; set; }
    public string? EquipeEstado { get; set; }
    public string? EquipeNacionalidade { get; set; }
    public int? AnoCompeticao { get; set; }
}
