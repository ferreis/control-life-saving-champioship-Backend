// Models/Competicao.cs
using System;

public class Competicao
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public DateTime DataInicio { get; set; } // DATE no DB
    public DateTime DataFim { get; set; }    // DATE no DB
    public string Local { get; set; }
    public int Ano { get; set; }
    public int PaisId { get; set; }
    public int EstadoId { get; set; }
    public int CidadeId { get; set; }

}