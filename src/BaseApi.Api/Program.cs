using System.Text;
using System.Text.Json.Serialization;
using BaseApi.Application;
using BaseApi.Infra;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

// Carregar variáveis de ambiente do arquivo .env
// Procura o .env na raiz do projeto (2 níveis acima de src/BaseApi.Api)
var currentDir = Directory.GetCurrentDirectory();
var envPath = Path.Combine(currentDir, "..", "..", ".env");

// Se não encontrar, tenta no diretório atual (para casos de publicação)
if (!File.Exists(envPath))
{
    envPath = Path.Combine(currentDir, ".env");
}

if (File.Exists(envPath))
{
    Env.Load(envPath);
    Console.WriteLine($"✅ Arquivo .env carregado: {Path.GetFullPath(envPath)}");
}
else
{
    Console.WriteLine($"⚠️  Arquivo .env não encontrado. Usando variáveis de ambiente do sistema.");
}

var builder = WebApplication.CreateBuilder(args);

// Adicionar variáveis de ambiente ao Configuration (incluindo as do .env)
builder.Configuration.AddEnvironmentVariables();

// Debug: mostrar connection string sendo usada
var connString = builder.Configuration.GetConnectionString("DefaultConnection");
if (connString != null)
{
    var preview = connString.Length > 50 ? connString.Substring(0, 50) + "..." : connString;
    Console.WriteLine($"🔍 Connection String: {preview}");
}
else
{
    Console.WriteLine("❌ Connection String não encontrada!");
}

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Permitir serialização de enums como strings (ex: "Admin" em vez de 0)
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Base API",
        Version = "v1",
        Description = "Template base para API com Clean Architecture e JWT Authentication"
    });

    // Configurar autenticação JWT no Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add Application services
builder.Services.AddApplication();

// Add Infrastructure (DbContext, Repositories, etc.)
builder.Services.AddInfrastructure(builder.Configuration);

// Configure JWT Authentication (opcional durante setup inicial)
var jwtSecret = builder.Configuration["Jwt:Secret"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (!string.IsNullOrEmpty(jwtSecret) && !string.IsNullOrEmpty(jwtIssuer) && !string.IsNullOrEmpty(jwtAudience))
{
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero
        };
    });
}
else
{
    Console.WriteLine("⚠️  JWT não configurado. Configure as variáveis JWT__SECRET, JWT__ISSUER e JWT__AUDIENCE no .env");
}

// CORS configuration for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

// Executar migrations e seed do banco de dados
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<BaseApi.Infra.Data.ApplicationDbContext>();
        var configuration = services.GetRequiredService<IConfiguration>();
        var logger = services.GetRequiredService<ILogger<BaseApi.Infra.Data.DatabaseSeeder>>();

        var seeder = new BaseApi.Infra.Data.DatabaseSeeder(context, configuration, logger);
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro ao executar migrations e seed do banco de dados");
        throw;
    }
}

// Configure Swagger (sempre ativo para desenvolvimento)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Base API v1");
    options.RoutePrefix = string.Empty; // Swagger na raiz (/)
});

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
