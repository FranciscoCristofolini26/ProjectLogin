using Microsoft.EntityFrameworkCore;
using ApiPostgresEf.Migrations;
using ProjetoLogin.Data;
using ProjetoLogin.Models;
using BCrypt.Net;

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

app.MapPost("/auth/login", async (AppDb db, LoginDto body) =>
{
    // ‘login’ pode ser userName OU email
    var user = await db.Usuarios
        .FirstOrDefaultAsync(u => u.UserName == body.Login || u.Email == body.Login);

    // Se não encontrou, erro
    if (user is null)
        return Results.Unauthorized();

    // Verifica hash (padrão)
    var senhaOk = false;
    try
    {
        // se você já salvou hash com BCrypt:
        senhaOk = BCrypt.Net.BCrypt.Verify(body.Senha, user.SenhaHash);
    }
    catch
    {
        // fallback provisório: se você ainda grava “em claro” (apenas para testes)
        senhaOk = user.SenhaHash == body.Senha;
    }

    if (!senhaOk)
        return Results.Unauthorized();

    // OK — (opcional: gere JWT aqui)
    return Results.Ok(new
    {
        ok = true,
        user = new { user.Id, user.UserName, user.Email }
    });
});

app.Run();
public record LoginDto(string Login, string Senha);


