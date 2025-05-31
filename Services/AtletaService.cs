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
    public AtletaComEquipeDto? ObterPorId(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        // Inclui JOIN com a tabela Pais para obter a nacionalidade
        command.CommandText =
            @"SELECT a.id AS atleta_id, a.nome, a.cpf, a.genero, a.data_nascimento, a.pais_id,
                     p.nacionalidade AS pais_nacionalidade
              FROM Atleta a
              INNER JOIN Pais p ON a.pais_id = p.id
              WHERE a.id = $id";
        command.Parameters.AddWithValue("$id", id);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new AtletaComEquipeDto
            {
                AtletaId = reader.GetInt32(reader.GetOrdinal("atleta_id")),
                Nome = reader.GetString(reader.GetOrdinal("nome")),
                Cpf = reader.GetString(reader.GetOrdinal("cpf")),
                Genero = reader.GetString(reader.GetOrdinal("genero")),
                DataNascimento = DateTime.Parse(reader.GetString(reader.GetOrdinal("data_nascimento"))),
                PaisId = reader.GetInt32(reader.GetOrdinal("pais_id")), // Preenchendo o PaisId
                Nacionalidade = reader.GetString(reader.GetOrdinal("pais_nacionalidade")) // Atribuindo a nacionalidade à propriedade PaisNome
            };
        }

        return null;
    }

    // Convertido para retornar List<AtletaComEquipeDto> e incluir a nacionalidade do país
    public List<AtletaComEquipeDto> Listar()
    {
        var atletas = new List<AtletaComEquipeDto>();

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        // Inclui JOIN com a tabela Pais para obter a nacionalidade
        command.CommandText =
            @"SELECT a.id AS atleta_id, a.nome, a.cpf, a.genero, a.data_nascimento, a.pais_id,
                     p.nacionalidade AS pais_nacionalidade
              FROM Atleta a
              INNER JOIN Pais p ON a.pais_id = p.id";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            atletas.Add(new AtletaComEquipeDto
            {
                AtletaId = reader.GetInt32(reader.GetOrdinal("atleta_id")),
                Nome = reader.GetString(reader.GetOrdinal("nome")),
                Cpf = reader.GetString(reader.GetOrdinal("cpf")),
                Genero = reader.GetString(reader.GetOrdinal("genero")),
                DataNascimento = DateTime.Parse(reader.GetString(reader.GetOrdinal("data_nascimento"))),
                PaisId = reader.GetInt32(reader.GetOrdinal("pais_id")), // Preenchendo o PaisId
                Nacionalidade = reader.GetString(reader.GetOrdinal("pais_nacionalidade")) // Atribuindo a nacionalidade à propriedade PaisNome
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
                a.pais_id,
                p.nacionalidade AS pais_nacionalidade,
                e.nome AS equipe_nome,
                e.tipo AS equipe_tipo,
                s.nome AS equipe_estado_nome,
                pe.nacionalidade AS equipe_nacionalidade_nome,
                ae.ano_competicao
            FROM Atleta a
            INNER JOIN Pais p ON a.pais_id = p.id
            LEFT JOIN Atleta_Equipe ae ON a.id = ae.atleta_id
            LEFT JOIN Equipe e ON ae.equipe_id = e.id
            LEFT JOIN Estado s ON e.estado_id = s.id
            LEFT JOIN Pais pe ON e.pais_id = pe.id;
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
                PaisId = reader.GetInt32(reader.GetOrdinal("pais_id")),
                Nacionalidade = reader.GetString(reader.GetOrdinal("pais_nacionalidade")),

                EquipeNome = reader.IsDBNull(reader.GetOrdinal("equipe_nome")) ? null : reader.GetString(reader.GetOrdinal("equipe_nome")),
                EquipeTipo = reader.IsDBNull(reader.GetOrdinal("equipe_tipo")) ? null : reader.GetString(reader.GetOrdinal("equipe_tipo")),
                EquipeEstado = reader.IsDBNull(reader.GetOrdinal("equipe_estado_nome")) ? null : reader.GetString(reader.GetOrdinal("equipe_estado_nome")),
                EquipeNacionalidade = reader.IsDBNull(reader.GetOrdinal("equipe_nacionalidade_nome")) ? null : reader.GetString(reader.GetOrdinal("equipe_nacionalidade_nome")),
                AnoCompeticao = reader.IsDBNull(reader.GetOrdinal("ano_competicao")) ? null : reader.GetInt32(reader.GetOrdinal("ano_competicao"))
            });
        }

        return lista;
    }
}