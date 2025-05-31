using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using SobrasaApi.Models;

namespace SobrasaApi.Services
{
    public class EquipeService
    {
        private readonly string _connectionString;

        public EquipeService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Adicionar(Equipe equipe)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Equipe (nome, tipo, estado, nacionalidade)
                VALUES ($nome, $tipo, $estado, $nacionalidade)";
            
            command.Parameters.AddWithValue("$nome", equipe.Nome);
            command.Parameters.AddWithValue("$tipo", equipe.Tipo);
            command.Parameters.AddWithValue("$estado", equipe.Estado);
            command.Parameters.AddWithValue("$nacionalidade", equipe.Nacionalidade);
            
            command.ExecuteNonQuery();
        }

        public List<Equipe> Listar()
        {
            var lista = new List<Equipe>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT id, nome, tipo, estado, nacionalidade FROM Equipe";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Equipe
                {
                    Id = reader.GetInt32(0),
                    Nome = reader.GetString(1),
                    Tipo = reader.GetString(2),
                    Estado = reader.GetString(3),
                    Nacionalidade = reader.GetString(4)
                });
            }
            return lista;
        }
    }
}