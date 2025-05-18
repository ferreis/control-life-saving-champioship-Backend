using Microsoft.Data.Sqlite;
using System;
using System.IO;

public static class DatabaseInitializer
{
    public static void Initialize(string dbPath)
    {
        bool novo = !File.Exists(dbPath);
        var connectionString = $"Data Source={dbPath}";

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();

        command.CommandText = @"
CREATE TABLE IF NOT EXISTS Atleta (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    nome TEXT NOT NULL,
    cpf TEXT NOT NULL UNIQUE,
    genero TEXT CHECK (genero IN ('M', 'F')),
    data_nascimento DATE NOT NULL,
    nacionalidade TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS Equipe (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    nome TEXT NOT NULL,
    tipo TEXT CHECK (tipo IN ('Estadual', 'Nacional', 'Internacional', 'Clube', 'Força Armada')),
    estado TEXT,
    nacionalidade TEXT
);
CREATE TABLE IF NOT EXISTS Atleta_Equipe (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    atleta_id INTEGER,
    equipe_id INTEGER,
    ano_competicao INTEGER,
    FOREIGN KEY (atleta_id) REFERENCES Atleta(id),
    FOREIGN KEY (equipe_id) REFERENCES Equipe(id)
);
CREATE TABLE IF NOT EXISTS Competicao (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    nome TEXT NOT NULL,
    data_inicio DATE,
    data_fim DATE,
    local TEXT,
    ano INTEGER
);
CREATE TABLE IF NOT EXISTS Prova (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    nome TEXT NOT NULL,
    tipo TEXT CHECK (tipo IN ('Praia', 'Piscina')),
    modalidade TEXT CHECK (modalidade IN ('Individual', 'Dupla', 'Revezamento')),
    tempo_ou_colocacao TEXT CHECK (tempo_ou_colocacao IN ('Tempo', 'Colocacao')),
    genero TEXT CHECK (genero IN ('M', 'F', 'unissex')),
    categoria_etaria TEXT
);
CREATE TABLE IF NOT EXISTS Participacao_Prova (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    atleta_id INTEGER,
    equipe_id INTEGER,
    prova_id INTEGER,
    competicao_id INTEGER,
    tempo NUMERIC,
    colocacao INTEGER,
    FOREIGN KEY (atleta_id) REFERENCES Atleta(id),
    FOREIGN KEY (equipe_id) REFERENCES Equipe(id),
    FOREIGN KEY (prova_id) REFERENCES Prova(id),
    FOREIGN KEY (competicao_id) REFERENCES Competicao(id)
);
CREATE TABLE IF NOT EXISTS Pontuacao (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    participacao_prova_id INTEGER,
    pontos INTEGER,
    FOREIGN KEY (participacao_prova_id) REFERENCES Participacao_Prova(id)
);
CREATE TABLE IF NOT EXISTS Categoria (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    descricao TEXT,
    idade_min INTEGER,
    idade_max INTEGER
);
";
        command.ExecuteNonQuery();

        // Inserir seed se Atleta estiver vazio
        if (novo || TabelaVazia(connection, "Atleta"))
        {
            Console.WriteLine("Inserindo dados iniciais...");

            var insert = connection.CreateCommand();
            insert.CommandText = @"
INSERT INTO Atleta (nome, cpf, genero, data_nascimento, nacionalidade)
VALUES
('João Silva', '12345678900', 'M', '2000-01-01', 'Brasil'),
('Maria Souza', '98765432100', 'F', '2002-06-15', 'Brasil');

INSERT INTO Equipe (nome, tipo, estado, nacionalidade)
VALUES ('Equipe Alpha', 'Estadual', 'SC', 'Brasil');

INSERT INTO Competicao (nome, data_inicio, data_fim, local, ano)
VALUES ('Sobrasa Rescue 2023', '2023-11-08', '2023-11-11', 'Itapema', 2023);

INSERT INTO Categoria (descricao, idade_min, idade_max)
VALUES ('A 18 a 24 anos', 18, 24);
";
            insert.ExecuteNonQuery();
        }
    }

    private static bool TabelaVazia(SqliteConnection conn, string tabela)
    {
        var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT COUNT(*) FROM {tabela}";
        var count = Convert.ToInt32(cmd.ExecuteScalar());
        return count == 0;
    }
}
