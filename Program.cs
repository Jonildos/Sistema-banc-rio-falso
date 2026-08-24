var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// 1. A REGRA DO CORS: Ensinamos o cérebro a criar uma política de passe livre
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTudo", policy =>
    {
        policy.AllowAnyOrigin()  // Permite qualquer site (no mundo real, colocaríamos só o endereço do FotoPontoCom)
              .AllowAnyHeader()  // Permite qualquer tipo de dado oculto
              .AllowAnyMethod(); // Permite POST, GET, PUT, etc.
    });
});

var app = builder.Build();

// 2. LIGA O CORS: Avisamos o servidor para usar a regra ANTES de abrir as portas
app.UseCors("PermitirTudo");

app.MapControllers();
app.Run();