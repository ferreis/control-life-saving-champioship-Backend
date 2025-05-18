var builder = WebApplication.CreateBuilder(args);

// Necessário para habilitar controllers (como AtletaController)
builder.Services.AddControllers();

var app = builder.Build();

// Mapeia as rotas definidas nos controllers
app.MapControllers();

// Inicializa o banco, cria tabelas e insere dados se necessário
DatabaseInitializer.Initialize("Database/sobrasa_banco_de_dados.db");

app.Run();
