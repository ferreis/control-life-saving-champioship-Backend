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

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Equipe (nome, tipo, estado, nacionalidade)
            VALUES ($nome, $tipo, $estado, $nacionalidade)";
        cmd.Parameters.AddWithValue("$nome", equipe.Nome);
        cmd.Parameters.AddWithValue("$tipo", equipe.Tipo);
        cmd.Parameters.AddWithValue("$estado", equipe.Estado);
        cmd.Parameters.AddWithValue("$nacionalidade", equipe.Nacionalidade);
        cmd.ExecuteNonQuery();
        // Recupera o ID da última linha inserida
        using (var lastIdCmd = connection.CreateCommand())
        {
            lastIdCmd.CommandText = "SELECT last_insert_rowid()";
            equipe.Id = Convert.ToInt32(lastIdCmd.ExecuteScalar());
        }
    }

    public List<Equipe> Listar()
    {
        var equipes = new List<Equipe>();
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT id, nome, tipo, estado, nacionalidade FROM Equipe";
        using var reader = cmd.ExecuteReader();
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

        var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM Equipe WHERE id = $id";
        cmd.Parameters.AddWithValue("$id", id);
        cmd.ExecuteNonQuery();
    }

    public void AtualizarParcial(int id, EquipeDto dto)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            UPDATE Equipe
            SET nome = COALESCE($nome, nome),
                tipo = COALESCE($tipo, tipo),
                estado = COALESCE($estado, estado),
                nacionalidade = COALESCE($nacionalidade, nacionalidade)
            WHERE id = $id";
        cmd.Parameters.AddWithValue("$id", id);
        cmd.Parameters.AddWithValue("$nome", (object?)dto.Nome ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$tipo", (object?)dto.Tipo ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$estado", (object?)dto.Estado ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$nacionalidade", (object?)dto.Nacionalidade ?? DBNull.Value);
        cmd.ExecuteNonQuery();
    }
}
