var builder = WebApplication.CreateBuilder(args);

// 1. A INSTRUÇÃO
builder.Services.AddControllers();

// CORREÇÃO AQUI: Mandamos o builder construir o app
var app = builder.Build();

// 2. O ROTEAMENTO
app.MapControllers();

// 3. LIGA O MOTOR
app.Run();