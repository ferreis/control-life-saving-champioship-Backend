namespace SobrasaApi.Models
{
    public class Pontuacao
    {
        public int Id { get; set; }
        public int ParticipacaoProvaId { get; set; } // FK
        public int Pontos { get; set; }
    }
}