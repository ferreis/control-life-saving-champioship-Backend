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

        // ** DEFINIÇÕES DE TABELAS (CONFORME SUA ESPECIFICAÇÃO MAIS RECENTE) **
        command.CommandText = @"
            -- Tabela de países (com nacionalidade)
            CREATE TABLE IF NOT EXISTS Pais (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                nome TEXT NOT NULL,
                nacionalidade TEXT NOT NULL
            );

            -- Tabela de estados
            CREATE TABLE IF NOT EXISTS Estado (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                nome TEXT NOT NULL,
                sigla TEXT NOT NULL,
                pais_id INTEGER NOT NULL,
                FOREIGN KEY (pais_id) REFERENCES Pais(id)
            );

            -- Tabela de cidades
            CREATE TABLE IF NOT EXISTS Cidade (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                nome TEXT NOT NULL,
                estado_id INTEGER NOT NULL,
                FOREIGN KEY (estado_id) REFERENCES Estado(id)
            );

            -- Atleta vinculado a um país
            CREATE TABLE IF NOT EXISTS Atleta (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                nome TEXT NOT NULL,
                cpf TEXT NOT NULL UNIQUE,
                genero TEXT CHECK (genero IN ('M', 'F')),
                data_nascimento DATE NOT NULL,
                pais_id INTEGER NOT NULL,
                FOREIGN KEY (pais_id) REFERENCES Pais(id)
            );

            -- Equipe vinculada a estado e país
            CREATE TABLE IF NOT EXISTS Equipe (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                nome TEXT NOT NULL,
                tipo TEXT CHECK (tipo IN ('Estadual', 'Nacional', 'Internacional', 'Clube', 'Força Armada')),
                estado_id INTEGER,
                pais_id INTEGER,
                FOREIGN KEY (estado_id) REFERENCES Estado(id),
                FOREIGN KEY (pais_id) REFERENCES Pais(id)
            );

            -- Associação de atleta com equipe e ano de competição
            CREATE TABLE IF NOT EXISTS Atleta_Equipe (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                atleta_id INTEGER NOT NULL,
                equipe_id INTEGER NOT NULL,
                ano_competicao INTEGER NOT NULL,
                FOREIGN KEY (atleta_id) REFERENCES Atleta(id),
                FOREIGN KEY (equipe_id) REFERENCES Equipe(id)
            );

            -- Competição com localização completa (ATUALIZADA)
            CREATE TABLE IF NOT EXISTS Competicao (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                nome TEXT NOT NULL,
                data_inicio DATE,
                data_fim DATE,
                ano INTEGER,
                pais_id INTEGER,
                estado_id INTEGER,
                cidade_id INTEGER,
                FOREIGN KEY (pais_id) REFERENCES Pais(id),
                FOREIGN KEY (estado_id) REFERENCES Estado(id),
                FOREIGN KEY (cidade_id) REFERENCES Cidade(id)
            );

            -- Prova definida com regras
            CREATE TABLE IF NOT EXISTS Prova (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                nome TEXT NOT NULL,
                tipo TEXT CHECK (tipo IN ('Praia', 'Piscina')),
                modalidade TEXT CHECK (modalidade IN ('Individual', 'Dupla', 'Revezamento')),
                tempo_ou_colocacao TEXT CHECK (tempo_ou_colocacao IN ('Tempo', 'Colocacao')),
                genero TEXT CHECK (genero IN ('M', 'F', 'unissex')),
                categoria_etaria TEXT
            );

            -- Participação de atleta na prova
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

            -- Pontuação da participação
            CREATE TABLE IF NOT EXISTS Pontuacao (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                participacao_prova_id INTEGER,
                pontos INTEGER,
                FOREIGN KEY (participacao_prova_id) REFERENCES Participacao_Prova(id)
            );

            -- Faixa etária da categoria
            CREATE TABLE IF NOT EXISTS Categoria (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                descricao TEXT,
                idade_min INTEGER,
                idade_max INTEGER
            );
            ";
        command.ExecuteNonQuery();

        // ** INSERÇÃO DE DADOS INICIAIS (PELO MENOS DUAS INFORMAÇÕES POR TABELA) **
        if (novo || TabelaVazia(connection, "Pais")) // Verifica se a tabela Pais está vazia para evitar duplicidade de seeds
        {
            Console.WriteLine("Inserindo dados iniciais...");

            var insert = connection.CreateCommand();
            insert.CommandText = @"
                -- 1. Inserir Países
                INSERT INTO Pais (nome, nacionalidade) VALUES ('Brasil', 'Brasileiro');
                INSERT INTO Pais (nome, nacionalidade) VALUES ('Estados Unidos', 'Americano');

                -- 2. Inserir Estados (vinculados aos Países)
                INSERT INTO Estado (nome, sigla, pais_id) VALUES ('Santa Catarina', 'SC', (SELECT id FROM Pais WHERE nome = 'Brasil'));
                INSERT INTO Estado (nome, sigla, pais_id) VALUES ('São Paulo', 'SP', (SELECT id FROM Pais WHERE nome = 'Brasil'));
                INSERT INTO Estado (nome, sigla, pais_id) VALUES ('Florida', 'FL', (SELECT id FROM Pais WHERE nome = 'Estados Unidos'));

                -- 3. Inserir Cidades (vinculadas aos Estados)
                INSERT INTO Cidade (nome, estado_id) VALUES ('Itajaí', (SELECT id FROM Estado WHERE sigla = 'SC'));
                INSERT INTO Cidade (nome, estado_id) VALUES ('Florianópolis', (SELECT id FROM Estado WHERE sigla = 'SC'));
                INSERT INTO Cidade (nome, estado_id) VALUES ('São Paulo', (SELECT id FROM Estado WHERE sigla = 'SP'));
                INSERT INTO Cidade (nome, estado_id) VALUES ('Miami', (SELECT id FROM Estado WHERE sigla = 'FL'));

                -- 4. Inserir Atletas (vinculados aos Países)
                INSERT INTO Atleta (nome, cpf, genero, data_nascimento, pais_id) VALUES ('João Silva', '12345678900', 'M', '2000-01-01', (SELECT id FROM Pais WHERE nome = 'Brasil'));
                INSERT INTO Atleta (nome, cpf, genero, data_nascimento, pais_id) VALUES ('Maria Souza', '98765432100', 'F', '2002-06-15', (SELECT id FROM Pais WHERE nome = 'Brasil'));
                INSERT INTO Atleta (nome, cpf, genero, data_nascimento, pais_id) VALUES ('Peter Johnson', '11223344550', 'M', '1998-03-20', (SELECT id FROM Pais WHERE nome = 'Estados Unidos'));

                -- 5. Inserir Equipes (vinculadas a Estados e Países)
                INSERT INTO Equipe (nome, tipo, estado_id, pais_id) VALUES ('Equipe Alpha SC', 'Estadual', (SELECT id FROM Estado WHERE sigla = 'SC'), (SELECT id FROM Pais WHERE nome = 'Brasil'));
                INSERT INTO Equipe (nome, tipo, estado_id, pais_id) VALUES ('Equipe Beta SP', 'Estadual', (SELECT id FROM Estado WHERE sigla = 'SP'), (SELECT id FROM Pais WHERE nome = 'Brasil'));
                INSERT INTO Equipe (nome, tipo, estado_id, pais_id) VALUES ('Team USA', 'Nacional', NULL, (SELECT id FROM Pais WHERE nome = 'Estados Unidos')); -- Exemplo de equipe nacional sem estado específico

                -- 6. Inserir Atleta_Equipe (vinculando atletas e equipes)
                INSERT INTO Atleta_Equipe (atleta_id, equipe_id, ano_competicao) VALUES ((SELECT id FROM Atleta WHERE cpf = '12345678900'), (SELECT id FROM Equipe WHERE nome = 'Equipe Alpha SC'), 2023);
                INSERT INTO Atleta_Equipe (atleta_id, equipe_id, ano_competicao) VALUES ((SELECT id FROM Atleta WHERE cpf = '98765432100'), (SELECT id FROM Equipe WHERE nome = 'Equipe Alpha SC'), 2023);
                INSERT INTO Atleta_Equipe (atleta_id, equipe_id, ano_competicao) VALUES ((SELECT id FROM Atleta WHERE cpf = '11223344550'), (SELECT id FROM Equipe WHERE nome = 'Team USA'), 2024);


                -- 7. Inserir Competições (com localização completa)
                INSERT INTO Competicao (nome, data_inicio, data_fim, ano, pais_id, estado_id, cidade_id) VALUES (
                    'Sobrasa Rescue 2023', '2023-11-08', '2023-11-11', 2023,
                    (SELECT id FROM Pais WHERE nome = 'Brasil'),
                    (SELECT id FROM Estado WHERE sigla = 'SC'),
                    (SELECT id FROM Cidade WHERE nome = 'Itajaí')
                );
                INSERT INTO Competicao (nome, data_inicio, data_fim, ano, pais_id, estado_id, cidade_id) VALUES (
                    'Nacional de Salvamento Aquático 2024', '2024-03-10', '2024-03-12', 2024,
                    (SELECT id FROM Pais WHERE nome = 'Brasil'),
                    (SELECT id FROM Estado WHERE sigla = 'SP'),
                    (SELECT id FROM Cidade WHERE nome = 'São Paulo')
                );
                INSERT INTO Competicao (nome, data_inicio, data_fim, ano, pais_id, estado_id, cidade_id) VALUES (
                    'World Lifesaving Championships 2024', '2024-09-01', '2024-09-07', 2024,
                    (SELECT id FROM Pais WHERE nome = 'Estados Unidos'),
                    (SELECT id FROM Estado WHERE sigla = 'FL'),
                    (SELECT id FROM Cidade WHERE nome = 'Miami')
                );


                -- 8. Inserir Provas
                INSERT INTO Prova (nome, tipo, modalidade, tempo_ou_colocacao, genero, categoria_etaria) VALUES ('Rescue Tube Race', 'Praia', 'Individual', 'Tempo', 'unissex', 'A 18 a 24 anos');
                INSERT INTO Prova (nome, tipo, modalidade, tempo_ou_colocacao, genero, categoria_etaria) VALUES ('200m Obstáculos', 'Piscina', 'Individual', 'Tempo', 'M', 'Senior');
                INSERT INTO Prova (nome, tipo, modalidade, tempo_ou_colocacao, genero, categoria_etaria) VALUES ('Line Throw', 'Praia', 'Dupla', 'Tempo', 'unissex', 'Todas');


                -- 9. Inserir Categorias
                INSERT INTO Categoria (descricao, idade_min, idade_max) VALUES ('A 18 a 24 anos', 18, 24);
                INSERT INTO Categoria (descricao, idade_min, idade_max) VALUES ('B 25 a 29 anos', 25, 29);
                INSERT INTO Categoria (descricao, idade_min, idade_max) VALUES ('Senior', 30, 99);


                -- 10. Inserir Participacao_Prova (Exemplo simples, ajuste conforme a lógica de seed desejada)
                INSERT INTO Participacao_Prova (atleta_id, equipe_id, prova_id, competicao_id, tempo, colocacao) VALUES (
                    (SELECT id FROM Atleta WHERE cpf = '12345678900'),
                    (SELECT id FROM Equipe WHERE nome = 'Equipe Alpha SC'),
                    (SELECT id FROM Prova WHERE nome = 'Rescue Tube Race'),
                    (SELECT id FROM Competicao WHERE nome = 'Sobrasa Rescue 2023'),
                    120.5,
                    3
                );
                INSERT INTO Participacao_Prova (atleta_id, equipe_id, prova_id, competicao_id, tempo, colocacao) VALUES (
                    (SELECT id FROM Atleta WHERE cpf = '98765432100'),
                    (SELECT id FROM Equipe WHERE nome = 'Equipe Alpha SC'),
                    (SELECT id FROM Prova WHERE nome = '200m Obstáculos'),
                    (SELECT id FROM Competicao WHERE nome = 'Sobrasa Rescue 2023'),
                    150.0,
                    1
                );
                INSERT INTO Participacao_Prova (atleta_id, equipe_id, prova_id, competicao_id, tempo, colocacao) VALUES (
                    (SELECT id FROM Atleta WHERE cpf = '11223344550'),
                    (SELECT id FROM Equipe WHERE nome = 'Team USA'),
                    (SELECT id FROM Prova WHERE nome = 'Line Throw'),
                    (SELECT id FROM Competicao WHERE nome = 'World Lifesaving Championships 2024'),
                    NULL, -- Colocação pode ser mais relevante aqui
                    2
                );


                -- 11. Inserir Pontuação (vinculada à Participacao_Prova)
                INSERT INTO Pontuacao (participacao_prova_id, pontos) VALUES ((SELECT id FROM Participacao_Prova WHERE atleta_id = (SELECT id FROM Atleta WHERE cpf = '12345678900') AND competicao_id = (SELECT id FROM Competicao WHERE nome = 'Sobrasa Rescue 2023')), 10);
                INSERT INTO Pontuacao (participacao_prova_id, pontos) VALUES ((SELECT id FROM Participacao_Prova WHERE atleta_id = (SELECT id FROM Atleta WHERE cpf = '98765432100') AND competicao_id = (SELECT id FROM Competicao WHERE nome = 'Sobrasa Rescue 2023')), 15);
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