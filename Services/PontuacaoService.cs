using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using SobrasaApi.Models;

namespace SobrasaApi.Services
{
    public class PontuacaoService
    {
        private readonly string _connectionString;

        public PontuacaoService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Adicionar(Pontuacao pontuacao)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Pontuacao (participacao_prova_id, pontos)
                VALUES ($participacao_prova_id, $pontos)";
            
            command.Parameters.AddWithValue("$participacao_prova_id", pontuacao.ParticipacaoProvaId);
            command.Parameters.AddWithValue("$pontos", pontuacao.Pontos);
            
            command.ExecuteNonQuery();
        }

        public List<Pontuacao> Listar()
        {
            var lista = new List<Pontuacao>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT id, participacao_prova_id, pontos FROM Pontuacao";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Pontuacao
                {
                    Id = reader.GetInt32(0),
                    ParticipacaoProvaId = reader.GetInt32(1),
                    Pontos = reader.GetInt32(2)
                });
            }
            return lista;
        }
    }
}