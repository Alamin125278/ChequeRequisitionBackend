using Carter;
using ChequeRequisiontService.Core.Dto.Ftp;
using ChequeRequisiontService.DbContexts;
using ChequeRequisiontService.Extensions;
using ChequeRequisiontService.Infrastructure.SignalRrepo;
using ChequeRequisiontService.MIddlewares;
using EProcurementService.Extensions;
using FluentValidation;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddSignalR()
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddCarter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    // Add this for Bearer token support
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token.\n\nExample: Bearer abc123xyz"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<CRDBContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration.GetConnectionString("CRDB")!);
});
builder.Services.Configure<List<FtpSetting>>(builder.Configuration.GetSection("FtpServers"));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("CRDB")!);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        //builder.WithOrigins([
        //    "http://localhost:5173"
        //]).AllowAnyMethod()     
        //.AllowAnyHeader()
        //.AllowCredentials();

        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddHttpContextAccessor();
builder.AddAppAuthetication();
builder.Services.AddAuthorization();

builder.Services.AddDependency();
var app = builder.Build();
// Use the CORS policy
app.UseCors("AllowAll");

app.MapHub<AppHub>("/cheque-requisition", options =>
{
    options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
});

app.UseMiddleware<AuthenticationMiddleware>();


// Configure the HTTP request pipeline.
app.MapCarter();

app.MapGet("/", () => "Cheque Requisition Service is running!");

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler(options => { });

app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();                          
    app.UseSwaggerUI();                        
}

app.UseStaticFiles();

app.Run();

