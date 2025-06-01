// Models/Prova.cs
using System;

public class Prova
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Tipo { get; set; } // Ex: "Praia", "Piscina"
    public string Modalidade { get; set; } // Ex: "Individual", "Dupla", "Revezamento"
    public string TempoOuColocacao { get; set; } // Ex: "Tempo", "Colocacao"
    public string Genero { get; set; } // Ex: "M", "F", "unissex"
    public string CategoriaEtaria { get; set; } // Ex: "Adulto", "Junior", "Master"
}