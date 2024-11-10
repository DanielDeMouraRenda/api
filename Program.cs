using Api.Data;
using Api.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Api.Model;

var builder = WebApplication.CreateBuilder(args);

// Configuração da string de conexão
builder.Services.AddDbContext<UsuarioDbcontext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adicionar o repositório de usuários
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

// Configuração do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração da autenticação JWT
var jwtKey = builder.Configuration["Jwt:Key"]; // Chave secreta deve ser configurada no appsettings.json
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Adicionar serviços de controle
builder.Services.AddControllers();

var app = builder.Build();

// Configuração do middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Mapeamento de endpoints
app.MapGet("/api/usuario", async (IUsuarioRepository usuarioRepository) =>
{
    var usuarios = await usuarioRepository.BuscaUsuario();
    return usuarios.Any() ? Results.Ok(usuarios) : Results.NoContent();
});

app.MapGet("/api/usuario/{id}", async (IUsuarioRepository usuarioRepository, int id) =>
{
    var usuario = await usuarioRepository.BuscaUsuario(id);
    return usuario != null ? Results.Ok(usuario) : Results.NotFound("Usuário não encontrado");
});

app.MapPost("/api/usuario", async (IUsuarioRepository usuarioRepository, Usuario usuario) =>
{
    usuarioRepository.AdicionarUsuario(usuario);
    return await usuarioRepository.SaveChangesAsync()
        ? Results.Ok("Usuário Adicionado com sucesso")
        : Results.BadRequest("Erro ao salvar Usuário");
});

app.MapPut("/api/usuario/{id}", async (IUsuarioRepository usuarioRepository, int id, Usuario usuario) =>
{
    var usuarioBanco = await usuarioRepository.BuscaUsuario(id);
    if (usuarioBanco == null) return Results.NotFound("Usuário não encontrado");

    usuarioBanco.Nome = usuario.Nome ?? usuarioBanco.Nome;
    usuarioBanco.Saldo = usuario.Saldo == 0 ? 0 : usuario.Saldo;

    usuarioRepository.AtualizarUsuario(usuarioBanco);
    return await usuarioRepository.SaveChangesAsync()
        ? Results.Ok("Usuário atualizado com sucesso")
        : Results.BadRequest("Erro ao atualizar Usuário");
});

app.MapDelete("/api/usuario/{id}", async (IUsuarioRepository usuarioRepository, int id) =>
{
    var usuarioBanco = await usuarioRepository.BuscaUsuario(id);
    if (usuarioBanco == null) return Results.NotFound("Usuário não encontrado");

    usuarioRepository.DeletaUsuario(usuarioBanco);
    return await usuarioRepository.SaveChangesAsync()
        ? Results.Ok("Usuário deletado com sucesso")
        : Results.BadRequest("Erro ao deletar Usuário");
});

app.Run();
