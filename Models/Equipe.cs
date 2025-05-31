public class Equipe
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Tipo { get; set; } // 'Estadual', 'Nacional', 'Internacional', 'Clube', 'Força Armada' [cite: 5]
    public int? EstadoId { get; set; } // Pode ser nulo [cite: 5]
    public int? PaisId { get; set; }   // Pode ser nulo [cite: 5]
}