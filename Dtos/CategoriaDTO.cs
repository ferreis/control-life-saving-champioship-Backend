namespace SobrasaApi.Dtos
{
    public class CategoriaDTO
    {
        public string Descricao { get; set; } = string.Empty;
        public int IdadeMin { get; set; }
        public int IdadeMax { get; set; }
    }
}