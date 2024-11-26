using Api.Data;
using Api.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Api.Model;
using Api.Controllers;

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

// Mapeamento de controladores
app.MapControllers();

app.Run();