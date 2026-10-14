using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using SinaHRM.Application.Abstractions.Persistence;
using SinaHRM.Application.ContractManagement.Interfaces;
using SinaHRM.Application.ContractManagement.Services;
using SinaHRM.Infrastracture;
using SinaHRM.Infrastracture.Data.Sql.Repositories;

var builder = WebApplication.CreateBuilder(args);
string conStr = string.Empty;
if (builder.Environment.IsDevelopment())
{
    conStr = builder.Configuration.GetConnectionString("DevelopmentConnection");
}
else
{
    conStr = builder.Configuration.GetConnectionString("SinaHRMDbContextConnection");
}

builder.Services.AddDbContext<SinaHRMDbContext>(c => c.UseSqlServer(conStr, c => c.UseCompatibilityLevel(110)));

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = false;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddMvc()
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SinaHRM API",
        Version = "v1"
    });
});


///IoC
builder.Services.AddScoped<IContractManagementRepository, ContractManegementRepository>();
builder.Services.AddScoped<IContractManagementService, ContractManagementService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "SinaHRM API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
