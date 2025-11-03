using Microsoft.EntityFrameworkCore;
using ApiPostgresEf.Migrations;
using ProjetoLogin.Data;
using ProjetoLogin.Models;

var builder = WebApplication.CreateBuilder(args);

// 1) CORS: defina UMA política
const string CorsDev = "CorsDev";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CorsDev, policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",   // Angular dev (HTTP)
                "https://localhost:4200"   // se você rodar Angular em HTTPS
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
            // .AllowCredentials(); // use somente se for mandar cookies/credenciais
    });
});

// 2) DbContext etc.
builder.Services.AddDbContext<AppDb>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Pg")));

var app = builder.Build();

// 3) (opcional) HTTPS redirection
app.UseHttpsRedirection();

// 4) **ATIVAR CORS** (antes dos endpoints)
app.UseCors(CorsDev);

// 5) seus endpoints
app.MapGet("/usuarios", async (AppDb db) =>
    await db.Usuarios.AsNoTracking().ToListAsync());

app.MapPost("/usuarios", async (AppDb db, Usuario usuario) =>
{
    // TODO: gerar hash seguro aqui, não gravar senha em claro
    db.Usuarios.Add(usuario);
    await db.SaveChangesAsync();
    return Results.Created($"/usuarios/{usuario.Id}", usuario);
});

app.Run();
