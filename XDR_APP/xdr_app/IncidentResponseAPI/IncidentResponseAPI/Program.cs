using DotNetEnv;
using IncidentResponseAPI.Helpers;
using IncidentResponseAPI.Models;
using IncidentResponseAPI.Orchestrators;
using IncidentResponseAPI.Repositories.Interfaces;
using IncidentResponseAPI.Repositories.Implementations;
using IncidentResponseAPI.Services;
using IncidentResponseAPI.Services.Implementations;
using IncidentResponseAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebApplication = Microsoft.AspNetCore.Builder.WebApplication;
using Prometheus;
using IncidentResponseAPI.Services.Implementations.Handlers;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
Env.Load();

// Debug logging to verify environment variable loading
var connectionString = Environment.GetEnvironmentVariable("DefaultConnection")
    ?? throw new InvalidOperationException("The ConnectionString property has not been initialized.");

// Add services to the container.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddControllers();
//builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
builder.Services.AddScoped<IEventsRepository, EventsRepository>();
//builder.Services.AddScoped<IEventsService, EventsService>();
builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
builder.Services.AddScoped<ISensorsRepository, SensorsRepository>();
builder.Services.AddScoped<ISensorsService, SensorsService>();
builder.Services.AddScoped<IRecommendationsRepository, RecommendationsRepository>();
builder.Services.AddScoped<IRecommendationsService, RecommendationsService>();
builder.Services.AddScoped<IIncidentsRepository, IncidentsRepository>();
builder.Services.AddScoped<IIncidentsService, IncidentsService>();
builder.Services.AddScoped<IEventsProcessingService, EventsProcessingService>();
builder.Services.AddScoped<IConfigurationValidator, ConfigurationValidator>();
builder.Services.AddSingleton<GraphAuthProvider>();
builder.Services.AddScoped<IGraphAuthService, GraphAuthService>();
builder.Services.AddScoped<IIncidentDetectionService, IncidentDetectionService>();
builder.Services.AddSingleton<SecurityMetricsService>();
//builder.Services.AddScoped<ISensorHandler, EmailSensorHandler>();
builder.Services.AddScoped<EmailSensorHandler>();
builder.Services.AddScoped<TeamsSensorHandler>();
builder.Services.AddScoped<SharePointSensorHandler>();
builder.Services.AddScoped<ISensorHandlerFactory, SensorHandlerFactory>();
builder.Services.AddMetricServer(options => {
    options.Port = 9091;
});
//builder.Services.AddSignalR();
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

// Add the DbContext and SensorOrchestrator
builder.Services.AddDbContext<IncidentResponseContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
    }),
    ServiceLifetime.Scoped
);
builder.Services.AddSingleton<SensorsOrchestrator>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<SensorsOrchestrator>());

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policyBuilder => policyBuilder.WithOrigins("http://localhost:3001")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials());
});

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "IncidentResponseAPI", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    // Enable middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwagger();
    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
    // specifying the Swagger JSON endpoint.
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IncidentResponseAPI v1"));
}

// Use CORS policy
//app.UseCors("AllowSpecificOrigin");
app.UseCors("AllowReactApp");

app.MapHub<IncidentHub>("/incidentHub");

// Add prometheus metrics endpoint
app.UseMetricServer();
app.UseHttpMetrics();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();