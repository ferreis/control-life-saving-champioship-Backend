using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using SobrasaApi.Models;

namespace SobrasaApi.Services
{
    public class ProvaService
    {
        private readonly string _connectionString;

        public ProvaService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Adicionar(Prova prova)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            // Inserindo todos os campos da sua tabela
            command.CommandText = @"
                INSERT INTO Prova (nome, tipo, modalidade, tempo_ou_colocacao, genero, categoria_etaria)
                VALUES ($nome, $tipo, $modalidade, $tempo_ou_colocacao, $genero, $categoria_etaria)";
            
            command.Parameters.AddWithValue("$nome", prova.Nome);
            command.Parameters.AddWithValue("$tipo", prova.Tipo);
            command.Parameters.AddWithValue("$modalidade", prova.Modalidade);
            command.Parameters.AddWithValue("$tempo_ou_colocacao", prova.TempoOuColocacao);
            command.Parameters.AddWithValue("$genero", prova.Genero);
            command.Parameters.AddWithValue("$categoria_etaria", prova.CategoriaEtaria);
            
            command.ExecuteNonQuery();
        }

        public List<Prova> Listar()
        {
            var lista = new List<Prova>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT id, nome, tipo, modalidade, tempo_ou_colocacao, genero, categoria_etaria FROM Prova";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Prova
                {
                    Id = reader.GetInt32(0),
                    Nome = reader.GetString(1),
                    Tipo = reader.GetString(2),
                    Modalidade = reader.GetString(3),
                    TempoOuColocacao = reader.GetString(4),
                    Genero = reader.GetString(5),
                    CategoriaEtaria = reader.GetString(6)
                });
            }
            return lista;
        }
    }
}