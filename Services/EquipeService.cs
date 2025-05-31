using System; // Necessário para DBNull.Value
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

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
        // Corrigido para usar estado_id e pais_id [cite: 5]
        command.CommandText = @"
            INSERT INTO Equipe (nome, tipo, estado_id, pais_id)
            VALUES ($nome, $tipo, $estado_id, $pais_id)";
        command.Parameters.AddWithValue("$nome", equipe.Nome);
        command.Parameters.AddWithValue("$tipo", equipe.Tipo);
        // Usar DBNull.Value para valores nulos de int?
        command.Parameters.AddWithValue("$estado_id", (object?)equipe.EstadoId ?? DBNull.Value);
        command.Parameters.AddWithValue("$pais_id", (object?)equipe.PaisId ?? DBNull.Value);
        command.ExecuteNonQuery();
    }

    public List<Equipe> Listar()
    {
        var equipes = new List<Equipe>();
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        // Corrigido para selecionar estado_id e pais_id [cite: 5]
        command.CommandText = "SELECT id, nome, tipo, estado_id, pais_id FROM Equipe";
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            equipes.Add(new Equipe
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")), // Usar GetOrdinal para robustez
                Nome = reader.GetString(reader.GetOrdinal("nome")),
                Tipo = reader.GetString(reader.GetOrdinal("tipo")),
                // Usar GetOrdinal e IsDBNull para campos que podem ser nulos
                EstadoId = reader.IsDBNull(reader.GetOrdinal("estado_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("estado_id")),
                PaisId = reader.IsDBNull(reader.GetOrdinal("pais_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("pais_id"))
            });
        }
        return equipes;
    }

    public void Deletar(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Equipe WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }

    public void Atualizar(int id, EquipeUpdateDto dto)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        // Corrigido para atualizar estado_id e pais_id [cite: 5]
        command.CommandText = @"
            UPDATE Equipe
            SET nome = COALESCE($nome, nome),
                tipo = COALESCE($tipo, tipo),
                estado_id = COALESCE($estado_id, estado_id),
                pais_id = COALESCE($pais_id, pais_id)
            WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);
        command.Parameters.AddWithValue("$nome", (object?)dto.Nome ?? DBNull.Value);
        command.Parameters.AddWithValue("$tipo", (object?)dto.Tipo ?? DBNull.Value);
        command.Parameters.AddWithValue("$estado_id", (object?)dto.EstadoId ?? DBNull.Value); // Corrigido
        command.Parameters.AddWithValue("$pais_id", (object?)dto.PaisId ?? DBNull.Value);       // Corrigido
        command.ExecuteNonQuery();
    }
}