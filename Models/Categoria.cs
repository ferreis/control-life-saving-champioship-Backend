namespace SobrasaApi.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public int IdadeMin { get; set; }
        public int IdadeMax { get; set; }
    }
}