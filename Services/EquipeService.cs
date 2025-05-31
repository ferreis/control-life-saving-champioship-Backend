using Microsoft.Data.Sqlite;
using System.Collections.Generic;

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
        var equipes = new List<Equipe>();
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT id, nome, tipo, estado, nacionalidade FROM Equipe";
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            equipes.Add(new Equipe
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Tipo = reader.GetString(2),
                Estado = reader.IsDBNull(3) ? null : reader.GetString(3),
                Nacionalidade = reader.GetString(4)
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

    public void AtualizarParcial(int id, EquipeUpdateDto dto)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Equipe
            SET nome = COALESCE($nome, nome),
                tipo = COALESCE($tipo, tipo),
                estado = COALESCE($estado, estado),
                nacionalidade = COALESCE($nacionalidade, nacionalidade)
            WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);
        command.Parameters.AddWithValue("$nome", (object?)dto.Nome ?? DBNull.Value);
        command.Parameters.AddWithValue("$tipo", (object?)dto.Tipo ?? DBNull.Value);
        command.Parameters.AddWithValue("$estado", (object?)dto.Estado ?? DBNull.Value);
        command.Parameters.AddWithValue("$nacionalidade", (object?)dto.Nacionalidade ?? DBNull.Value);
        command.ExecuteNonQuery();
    }
}
