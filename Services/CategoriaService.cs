using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using SobrasaApi.Models;

namespace SobrasaApi.Services
{
    public class CategoriaService
    {
        private readonly string _connectionString;

        public CategoriaService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Adicionar(Categoria categoria)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Categoria (descricao, idade_min, idade_max)
                VALUES ($descricao, $idade_min, $idade_max)";
            
            command.Parameters.AddWithValue("$descricao", categoria.Descricao);
            command.Parameters.AddWithValue("$idade_min", categoria.IdadeMin);
            command.Parameters.AddWithValue("$idade_max", categoria.IdadeMax);
            
            command.ExecuteNonQuery();
        }

        public List<Categoria> Listar()
        {
            var lista = new List<Categoria>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT id, descricao, idade_min, idade_max FROM Categoria";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Categoria
                {
                    Id = reader.GetInt32(0),
                    Descricao = reader.GetString(1),
                    IdadeMin = reader.GetInt32(2),
                    IdadeMax = reader.GetInt32(3)
                });
            }
            return lista;
        }
    }
}