using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using SobrasaApi.Models;

namespace SobrasaApi.Services
{
    public class AtletaEquipeService
    {
        private readonly string _connectionString;

        public AtletaEquipeService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Adicionar(AtletaEquipe atletaEquipe)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Atleta_Equipe (atleta_id, equipe_id, ano_competicao)
                VALUES ($atleta_id, $equipe_id, $ano_competicao)";
            
            command.Parameters.AddWithValue("$atleta_id", atletaEquipe.AtletaId);
            command.Parameters.AddWithValue("$equipe_id", atletaEquipe.EquipeId);
            command.Parameters.AddWithValue("$ano_competicao", atletaEquipe.AnoCompeticao);
            
            command.ExecuteNonQuery();
        }

        public List<AtletaEquipe> Listar()
        {
            var lista = new List<AtletaEquipe>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT id, atleta_id, equipe_id, ano_competicao FROM Atleta_Equipe";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new AtletaEquipe
                {
                    Id = reader.GetInt32(0),
                    AtletaId = reader.GetInt32(1),
                    EquipeId = reader.GetInt32(2),
                    AnoCompeticao = reader.GetInt32(3)
                });
            }
            return lista;
        }
    }
}