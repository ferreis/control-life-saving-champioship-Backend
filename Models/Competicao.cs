using System;

 public class Competicao
    {
      public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string? Local { get; set; } // Mantém Local
        // REMOVIDO: PaisId, EstadoId, CidadeId
        public int Ano { get; set; }
    }