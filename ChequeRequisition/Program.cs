using Carter;
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
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CRDBContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration.GetConnectionString("CRDB")!);
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("CRDB")!);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.WithOrigins([
            "http://localhost:5173"
        ]).AllowAnyMethod()     // Allow any HTTP method (GET, POST, etc.)
        .AllowAnyHeader()
        .AllowCredentials();    // Allow any header
    });
});

builder.Services.AddHttpContextAccessor();
builder.AddAppAuthetication();
builder.Services.AddAuthorization();

builder.Services.AddDependency();
var app = builder.Build();
// Use the CORS policy
app.UseCors("AllowAll");

app.MapHub<AppHub>("/e-procurement-hub", options =>
{
    options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
});

// Configure the HTTP request pipeline.
app.MapCarter();

app.MapGet("/", () => "Cheque Requisition Service is running!");



app.UseExceptionHandler(options => { });

app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();                          // Serve the Swagger JSON
    app.UseSwaggerUI();                        // Serve the Swagger UI
}

app.Run();

