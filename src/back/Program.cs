using System.Reflection;
using CashWiseAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using CashWiseAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Configure(context.Configuration.GetSection("Kestrel"));
});

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var useInMemory = builder.Environment.IsEnvironment("Testing") ||
                  string.Equals(builder.Configuration["Database:Provider"], "InMemory", StringComparison.OrdinalIgnoreCase);

if (useInMemory)
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("CashWiseTests"));
}
else
{
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("ConnectionStrings:DefaultConnection must be configured.");
    }

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36))));
}

builder.Services.AddScoped<DatabaseService>();
builder.Services.AddScoped<TransacaoService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<MetaFinanceiraService>();
builder.Services.AddScoped<DashboardService>();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:8080" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AppCors", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Financial Management API",
        Version = "v1",
        Description = "API for managing financial transactions and user data",
        Contact = new OpenApiContact
        {
            Name = "API Support",
            Email = "support@example.com"
        }
    });

    c.CustomSchemaIds(type => type.FullName);

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the ******",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

if (!builder.Environment.IsEnvironment("Testing") && string.IsNullOrWhiteSpace(builder.Configuration["Jwt:Key"]))
{
    throw new InvalidOperationException("Jwt:Key must be configured outside test environments.");
}

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Financial Management API V1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "Financial Management API Documentation";
    c.DefaultModelsExpandDepth(2);
});

app.UseHttpsRedirection();
app.UseCors("AppCors");
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;
