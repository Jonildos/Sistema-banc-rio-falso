using BCrypt.Net;
using Microsoft.Extensions.FileProviders;
using Sistema_banc_rio_falso.Data;
using Sistema_banc_rio_falso.Models;
using Sistema_banc_rio_falso.Services; // Apenas uma vez aqui no topo

var builder = WebApplication.CreateBuilder(args);

// Adiciona os controllers, o contexto do banco e a camada de serviços (Service Pattern)
builder.Services.AddControllers();
builder.Services.AddDbContext<BancoDbContext>();
builder.Services.AddScoped<ContaService>(); // 👈 REGISTRO DO SERVICE QUE ESTAVA FALTANDO

// Configuração de CORS
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
        string senhaHash = BCrypt.Net.BCrypt.HashPassword("admin123");
        dbContext.Administradores.Add(new Administrador("admin@bancofalso.com", "000.000.000-00", senhaHash));
        dbContext.SaveChanges();
    }
}

// 🌐 Serve os arquivos estáticos da pasta PlataformaWeb na raiz do site
var arquivosPlataformaWeb = new PhysicalFileProvider(
    Path.Combine(builder.Environment.ContentRootPath, "PlataformaWeb"));
app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = arquivosPlataformaWeb });
app.UseStaticFiles(new StaticFileOptions { FileProvider = arquivosPlataformaWeb });

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();