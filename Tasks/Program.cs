using System.Reflection;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Taks.Core.Common;
using Tasks.API.Extensions;
using Tasks.API.Middlewares;
using Tasks.Infrastructure.Data;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


// Register DbContext
// builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SecondaryDbConnection")));

// builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseOracle(builder.Configuration.GetConnectionString("PrimaryDbConnection"), options => options.UseOracleSQLCompatibility("19")));
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseOracle(builder.Configuration.GetConnectionString("DockerDbConnection")));

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// Add caching services
builder.Services.AddMemoryCache();

// Register ILogger service
builder.Services.AddLogging(loggingBuilder =>
{
    _ = loggingBuilder.AddSeq(builder.Configuration.GetSection("SeqConfig"));
});

// Register Services
builder.Services.RegisterSecurityService(builder.Configuration);
builder.Services.RegisterServices();
builder.Services.RegisterMapperService();
builder.Services.AddAuthorization();

// API Versioning
builder.Services
    .AddApiVersioning()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

// Swagger Settings
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    options.OperationFilter<SwaggerDefaultValues>();

    // Define Bearer token security scheme
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: 'Bearer 12345abcdef'",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    // Add Bearer token as a requirement for all operations
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
            new string[] {}
        }
    });

});

WebApplication app = builder.Build();

// Database seeding
using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;
    ILoggerFactory loggerFactory = services.GetRequiredService<ILoggerFactory>();

    try
    {
        ApplicationDbContext context = services.GetRequiredService<ApplicationDbContext>();

        // Seed the database
        await ApplicationDbContextSeeds.SeedsAsync(services, loggerFactory);
    }
    catch (Exception ex)
    {
        ILogger<Program> logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI(options =>
    {
        options.DefaultModelsExpandDepth(-1);
        IReadOnlyList<Asp.Versioning.ApiExplorer.ApiVersionDescription> descriptions = app.DescribeApiVersions();

        // Build a swagger endpoint for each discovered API version
        foreach (Asp.Versioning.ApiExplorer.ApiVersionDescription description in descriptions)
        {
            string url = $"/swagger/{description.GroupName}/swagger.json";
            string name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
    });
}

app.UseStaticFiles();
app.UseRouting(); // Add this line to configure routing

app.UseAuthentication();
app.UseAuthorization();

#region Custom Middleware
app.UseMiddleware<RequestResponseLoggingMiddleware>();

#endregion

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers(); // Map your regular API controllers
});

app.Run();

