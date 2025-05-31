var builder = WebApplication.CreateBuilder(args);

// Habilita CORS para aceitar qualquer origem, método e header
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Habilita controllers
builder.Services.AddControllers();

var app = builder.Build();

// Ativa o uso de CORS
app.UseCors();

// Mapeia as rotas dos controllers
app.MapControllers();

// Inicializa o banco de dados SQLite
DatabaseInitializer.Initialize("Database/sobrasa_banco_de_dados.db");

app.Run();
