namespace SobrasaApi.Models
{
    public class Prova
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty; // Tipo da Prova (ex: Individual, Revezamento)
        public string Modalidade { get; set; } = string.Empty; // Modalidade (ex: Natação, Corridas)
        public string TempoOuColocacao { get; set; } = string.Empty; // Se a prova é por tempo ou colocação direta (ex: "tempo", "colocacao")
        public string Genero { get; set; } = string.Empty; // Gênero da prova (ex: "Masculino", "Feminino", "Misto")
        public string CategoriaEtaria { get; set; } = string.Empty; // Categoria etária (ex: "18-24", "25-29")
    }
}