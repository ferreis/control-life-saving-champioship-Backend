using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using SobrasaApi.Models;

namespace SobrasaApi.Services
{
    public class ParticipacaoProvaService
    {
        private readonly string _connectionString;

        public ParticipacaoProvaService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Adicionar(ParticipacaoProva participacao)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            // Inserindo todos os campos da sua tabela
            command.CommandText = @"
                INSERT INTO Participacao_Prova (atleta_id, equipe_id, prova_id, competicao_id, tempo, colocacao)
                VALUES ($atleta_id, $equipe_id, $prova_id, $competicao_id, $tempo, $colocacao)";
            
            command.Parameters.AddWithValue("$atleta_id", participacao.AtletaId);
            command.Parameters.AddWithValue("$equipe_id", participacao.EquipeId);
            command.Parameters.AddWithValue("$prova_id", participacao.ProvaId);
            command.Parameters.AddWithValue("$competicao_id", participacao.CompeticaoId);
            // TimeSpan é armazenado como TEXT "HH:mm:ss" ou string vazia se nulo
            command.Parameters.AddWithValue("$tempo", participacao.Tempo?.ToString(@"hh\:mm\:ss") ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("$colocacao", participacao.Colocacao);
            
            command.ExecuteNonQuery();
        }

        public List<ParticipacaoProva> Listar()
        {
            var lista = new List<ParticipacaoProva>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT id, atleta_id, equipe_id, prova_id, competicao_id, tempo, colocacao FROM Participacao_Prova";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new ParticipacaoProva
                {
                    Id = reader.GetInt32(0),
                    AtletaId = reader.GetInt32(1),
                    EquipeId = reader.GetInt32(2),
                    ProvaId = reader.GetInt32(3),
                    CompeticaoId = reader.GetInt32(4),
                    Tempo = reader.IsDBNull(5) ? null : TimeSpan.Parse(reader.GetString(5)),
                    Colocacao = reader.GetInt32(6)
                });
            }
            return lista;
        }
    }
}