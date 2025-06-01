// Services/CompeticaoService.cs
using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Data; // Para IDataReader

public class CompeticaoService
{
    private readonly string _connectionString;

    public CompeticaoService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public Competicao ObterPorId(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT id, nome, data_inicio, data_fim, local, ano, pais_id, estado_id, cidade_id FROM Competicao WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Competicao
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Nome = reader.GetString(reader.GetOrdinal("nome")),
                DataInicio = DateTime.Parse(reader.GetString(reader.GetOrdinal("data_inicio"))),
                DataFim = DateTime.Parse(reader.GetString(reader.GetOrdinal("data_fim"))),
                Local = reader.GetString(reader.GetOrdinal("local")),
                Ano = reader.GetInt32(reader.GetOrdinal("ano")),
                PaisId = reader.GetInt32(reader.GetOrdinal("pais_id")),
                EstadoId = reader.GetInt32(reader.GetOrdinal("estado_id")),
                CidadeId = reader.GetInt32(reader.GetOrdinal("cidade_id"))
            };
        }
        return null;
    }

    public List<Competicao> Listar()
    {
        var competicoes = new List<Competicao>();
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT id, nome, data_inicio, data_fim, local, ano, pais_id, estado_id, cidade_id FROM Competicao";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            competicoes.Add(new Competicao
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Nome = reader.GetString(reader.GetOrdinal("nome")),
                DataInicio = DateTime.Parse(reader.GetString(reader.GetOrdinal("data_inicio"))),
                DataFim = DateTime.Parse(reader.GetString(reader.GetOrdinal("data_fim"))),
                Local = reader.GetString(reader.GetOrdinal("local")),
                Ano = reader.GetInt32(reader.GetOrdinal("ano")),
                PaisId = reader.GetInt32(reader.GetOrdinal("pais_id")),
                EstadoId = reader.GetInt32(reader.GetOrdinal("estado_id")),
                CidadeId = reader.GetInt32(reader.GetOrdinal("cidade_id"))
            });
        }
        return competicoes;
    }

    public int Adicionar(CompeticaoDto dto)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Competicao (nome, data_inicio, data_fim, local, ano, pais_id, estado_id, cidade_id)
            VALUES ($nome, $dataInicio, $dataFim, $local, $ano, $paisId, $estadoId, $cidadeId)";
        command.Parameters.AddWithValue("$nome", dto.Nome);
        command.Parameters.AddWithValue("$dataInicio", dto.DataInicio.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$dataFim", dto.DataFim.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$local", dto.Local);
        command.Parameters.AddWithValue("$ano", dto.Ano);
        command.Parameters.AddWithValue("$paisId", dto.PaisId);
        command.Parameters.AddWithValue("$estadoId", dto.EstadoId);
        command.Parameters.AddWithValue("$cidadeId", dto.CidadeId);
        command.ExecuteNonQuery();

        // Obter o ID da última linha inserida usando SQL
        command.CommandText = "SELECT last_insert_rowid()";
        var newId = (long)command.ExecuteScalar();
        return (int)newId;
    }

    public void Atualizar(CompeticaoUpdateDto dto)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Competicao
            SET nome = COALESCE($nome, nome),
                data_inicio = COALESCE($dataInicio, data_inicio),
                data_fim = COALESCE($dataFim, data_fim),
                local = COALESCE($local, local),
                ano = COALESCE($ano, ano),
                pais_id = COALESCE($paisId, pais_id),
                estado_id = COALESCE($estadoId, estado_id),
                cidade_id = COALESCE($cidadeId, cidade_id)
            WHERE id = $id";
        command.Parameters.AddWithValue("$id", dto.Id);
        command.Parameters.AddWithValue("$nome", (object?)dto.Nome ?? DBNull.Value);
        command.Parameters.AddWithValue("$dataInicio", (object?)dto.DataInicio?.ToString("yyyy-MM-dd") ?? DBNull.Value);
        command.Parameters.AddWithValue("$dataFim", (object?)dto.DataFim?.ToString("yyyy-MM-dd") ?? DBNull.Value);
        command.Parameters.AddWithValue("$local", (object?)dto.Local ?? DBNull.Value);
        command.Parameters.AddWithValue("$ano", (object?)dto.Ano ?? DBNull.Value);
        command.Parameters.AddWithValue("$paisId", (object?)dto.PaisId ?? DBNull.Value);
        command.Parameters.AddWithValue("$estadoId", (object?)dto.EstadoId ?? DBNull.Value);
        command.Parameters.AddWithValue("$cidadeId", (object?)dto.CidadeId ?? DBNull.Value);
        command.ExecuteNonQuery();
    }

    public void Deletar(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Competicao WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }
}