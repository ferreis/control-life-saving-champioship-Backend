using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

public class CompeticaoService
{
    private readonly string _connectionString;

    public CompeticaoService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Adicionar(Competicao competicao)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Competicao (nome, data_inicio, data_fim, local, ano)
                VALUES ($nome, $data_inicio, $data_fim, $local, $ano)";
        
       command.Parameters.AddWithValue("$nome", competicao.Nome);
            command.Parameters.AddWithValue("$data_inicio", competicao.DataInicio?.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("$data_fim", competicao.DataFim?.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("$local", competicao.Local);
            command.Parameters.AddWithValue("$ano", competicao.Ano); 
            // REMOVIDO: Parâmetros para PaisId, EstadoId, CidadeId
        
        command.ExecuteNonQuery();
    }

    public List<Competicao> Listar()
    {
        var lista = new List<Competicao>();
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        // CORREÇÃO/MELHORIA: Removi 'local' do SELECT para corresponder ao modelo e evitar erros de índice.
        // Se 'local' for necessário, adicione ao seu modelo e mapeie.
            cmd.CommandText = "SELECT Id, Nome, DataInicio, DataFim, Local, Ano FROM Competicao";

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(new Competicao
            {
                  Id = reader.GetInt32(0),
                    Nome = reader.GetString(1),
                    DataInicio = reader.IsDBNull(2) ? null : DateTime.Parse(reader.GetString(2)),
                    DataFim = reader.IsDBNull(3) ? null : DateTime.Parse(reader.GetString(3)),
                    Local = reader.IsDBNull(4) ? null : reader.GetString(4), // Local é o índice 4
                    Ano = reader.GetInt32(5) // Ano é o índice 5
                    // REMOVIDO: Leitura de PaisId, EstadoId, CidadeId
            });
        }
        return lista;
    }
}