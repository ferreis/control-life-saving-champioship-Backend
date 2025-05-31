using System;

namespace SobrasaApi.Models
{
    public class ParticipacaoProva
    {
        public int Id { get; set; }
        public int AtletaId { get; set; } // FK
        public int EquipeId { get; set; } // FK
        public int ProvaId { get; set; } // FK
        public int CompeticaoId { get; set; } // FK
        public TimeSpan? Tempo { get; set; } // NUMERIC no DB, TimeSpan em C#
        public int Colocacao { get; set; }
    }
}