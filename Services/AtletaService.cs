using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

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
        command.CommandText =
            "SELECT id, nome, cpf, genero, data_nascimento, pais_id FROM Atleta WHERE id = $id";
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
                PaisId = reader.GetInt32(reader.GetOrdinal("pais_id")),
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
        command.CommandText =
            "SELECT id, nome, cpf, genero, data_nascimento, pais_id FROM Atleta";

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
                PaisId = reader.GetInt32(reader.GetOrdinal("pais_id")),
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
            INSERT INTO Atleta (nome, cpf, genero, data_nascimento, pais_id)
            VALUES ($nome, $cpf, $genero, $data_nascimento, $pais_id)";
        command.Parameters.AddWithValue("$nome", atleta.Nome);
        command.Parameters.AddWithValue("$cpf", atleta.Cpf);
        command.Parameters.AddWithValue("$genero", atleta.Genero);
        command.Parameters.AddWithValue("$data_nascimento", atleta.DataNascimento.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$pais_id", atleta.PaisId);
        command.ExecuteNonQuery();
    }

    public void Atualizar(Atleta atleta)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Atleta
            SET nome = $nome, cpf = $cpf, genero = $genero, data_nascimento = $data_nascimento, pais_id = $pais_id
            WHERE id = $id";
        command.Parameters.AddWithValue("$id", atleta.Id);
        command.Parameters.AddWithValue("$nome", atleta.Nome);
        command.Parameters.AddWithValue("$cpf", atleta.Cpf);
        command.Parameters.AddWithValue("$genero", atleta.Genero);
        command.Parameters.AddWithValue("$data_nascimento", atleta.DataNascimento.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$pais_id", atleta.PaisId);
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

    public void VincularAtletaEquipe(AtletaEquipe atletaEquipe)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
        INSERT INTO Atleta_Equipe (atleta_id, equipe_id, ano_competicao)
        VALUES ($atletaId, $equipeId, $ano)";
        command.Parameters.AddWithValue("$atletaId", atletaEquipe.AtletaId);
        command.Parameters.AddWithValue("$equipeId", atletaEquipe.EquipeId);
        command.Parameters.AddWithValue("$ano", atletaEquipe.AnoCompeticao);
        command.ExecuteNonQuery();

        connection.Close();
    }

    public List<AtletaComEquipeDto> ListarComEquipes()
    {
        var lista = new List<AtletaComEquipeDto>();

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT
                a.id AS atleta_id,
                a.nome,
                a.cpf,
                a.genero,
                a.data_nascimento,
                p.nacionalidade AS pais_nacionalidade, -- Alias corrigido para a nacionalidade do país do atleta [cite: 1, 4]
                e.nome AS equipe_nome,
                e.tipo AS equipe_tipo,
                s.nome AS equipe_estado_nome, -- Adicionado JOIN para obter o nome do estado da equipe [cite: 5, 2]
                pe.nacionalidade AS equipe_nacionalidade_nome, -- Adicionado JOIN para obter a nacionalidade do país da equipe [cite: 5, 1]
                ae.ano_competicao
            FROM Atleta a
            INNER JOIN Pais p ON a.pais_id = p.id
            LEFT JOIN Atleta_Equipe ae ON a.id = ae.atleta_id
            LEFT JOIN Equipe e ON ae.equipe_id = e.id
            LEFT JOIN Estado s ON e.estado_id = s.id -- JOIN para a tabela Estado [cite: 5, 2]
            LEFT JOIN Pais pe ON e.pais_id = pe.id; -- JOIN para a tabela Pais (para o país da equipe) [cite: 5, 1]
";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(new AtletaComEquipeDto
            {
                AtletaId = reader.GetInt32(reader.GetOrdinal("atleta_id")),
                Nome = reader.GetString(reader.GetOrdinal("nome")),
                Cpf = reader.GetString(reader.GetOrdinal("cpf")),
                Genero = reader.GetString(reader.GetOrdinal("genero")),
                DataNascimento = DateTime.Parse(reader.GetString(reader.GetOrdinal("data_nascimento"))),
                Nacionalidade = reader.GetString(reader.GetOrdinal("pais_nacionalidade")),

                EquipeNome = reader.IsDBNull(reader.GetOrdinal("equipe_nome")) ? null : reader.GetString(reader.GetOrdinal("equipe_nome")),
                EquipeTipo = reader.IsDBNull(reader.GetOrdinal("equipe_tipo")) ? null : reader.GetString(reader.GetOrdinal("equipe_tipo")),
                EquipeEstado = reader.IsDBNull(reader.GetOrdinal("equipe_estado_nome")) ? null : reader.GetString(reader.GetOrdinal("equipe_estado_nome")), // Corrigido para usar o novo alias [cite: 2]
                EquipeNacionalidade = reader.IsDBNull(reader.GetOrdinal("equipe_nacionalidade_nome")) ? null : reader.GetString(reader.GetOrdinal("equipe_nacionalidade_nome")), // Corrigido para usar o novo alias [cite: 1]
                AnoCompeticao = reader.IsDBNull(reader.GetOrdinal("ano_competicao")) ? null : reader.GetInt32(reader.GetOrdinal("ano_competicao"))
            });
        }

        return lista;
    }
}