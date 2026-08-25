using Sistema_banc_rio_falso.Data;
using Sistema_banc_rio_falso.Models; 

var builder = WebApplication.CreateBuilder(args);

// Adiciona os controllers e o contexto do banco
builder.Services.AddControllers();
builder.Services.AddDbContext<BancoDbContext>();

// Configuração de CORS se necessário...
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// GARANTE QUE O BANCO DE DADOS SQLite É CRIADO NA INICIALIZAÇÃO
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BancoDbContext>();
    dbContext.Database.EnsureCreated();
    
    // Cria um Admin padrão se não existir nenhum cadastrado no sistema
    if (!dbContext.Administradores.Any())
    {
        dbContext.Administradores.Add(new Administrador("admin@bancofalso.com", "000.000.000-00", "admin123"));
        dbContext.SaveChanges();
    }
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();