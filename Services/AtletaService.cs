using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

public class AtletaService
{
    private readonly string _connectionString;

    public AtletaService(string connectionString)
    {
        _connectionString = connectionString;
    }
    public Atleta ObterPorId(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT id, nome, cpf, genero, data_nascimento, nacionalidade FROM Atleta WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Atleta
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Nome = reader.GetString(reader.GetOrdinal("nome")),
                Cpf = reader.GetString(reader.GetOrdinal("cpf")),
                Genero = reader.GetString(reader.GetOrdinal("genero")),
                DataNascimento = DateTime.Parse(reader.GetString(reader.GetOrdinal("data_nascimento"))),
                Nacionalidade = reader.GetString(reader.GetOrdinal("nacionalidade"))
            };
        }

        return null;
    }
    public List<Atleta> Listar()
    {
        var atletas = new List<Atleta>();

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT id, nome, cpf, genero, data_nascimento, nacionalidade FROM Atleta";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            atletas.Add(new Atleta
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Nome = reader.GetString(reader.GetOrdinal("nome")),
                Cpf = reader.GetString(reader.GetOrdinal("cpf")),
                Genero = reader.GetString(reader.GetOrdinal("genero")),
                DataNascimento = DateTime.Parse(reader.GetString(reader.GetOrdinal("data_nascimento"))),
                Nacionalidade = reader.GetString(reader.GetOrdinal("nacionalidade"))
            });
        }

        return atletas;
    }

    public void Adicionar(Atleta atleta)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Atleta (nome, cpf, genero, data_nascimento, nacionalidade)
            VALUES ($nome, $cpf, $genero, $data_nascimento, $nacionalidade)";
        command.Parameters.AddWithValue("$nome", atleta.Nome);
        command.Parameters.AddWithValue("$cpf", atleta.Cpf);
        command.Parameters.AddWithValue("$genero", atleta.Genero);
        command.Parameters.AddWithValue("$data_nascimento", atleta.DataNascimento.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$nacionalidade", atleta.Nacionalidade);
        command.ExecuteNonQuery();
    }
    public void Atualizar(Atleta atleta)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Atleta
            SET nome = $nome, cpf = $cpf, genero = $genero, data_nascimento = $data_nascimento, nacionalidade = $nacionalidade
            WHERE id = $id";
        command.Parameters.AddWithValue("$id", atleta.Id);
        command.Parameters.AddWithValue("$nome", atleta.Nome);
        command.Parameters.AddWithValue("$cpf", atleta.Cpf);
        command.Parameters.AddWithValue("$genero", atleta.Genero);
        command.Parameters.AddWithValue("$data_nascimento", atleta.DataNascimento.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$nacionalidade", atleta.Nacionalidade);
        command.ExecuteNonQuery();
    }
    public void Deletar(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Atleta WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }
}
