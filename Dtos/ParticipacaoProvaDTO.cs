namespace SobrasaApi.Dtos
{
    public class ParticipacaoProvaDTO
    {
        public int AtletaId { get; set; }
        public int EquipeId { get; set; }
        public int ProvaId { get; set; }
        public int CompeticaoId { get; set; }
        public string? Tempo { get; set; } // String para input (ex: "00:30:45")
        public int Colocacao { get; set; }
    }
}