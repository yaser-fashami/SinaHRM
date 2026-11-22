using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using SinaHRM.Application.Abstractions.Persistence;
using SinaHRM.Application.ContractManagement.Interfaces;
using SinaHRM.Application.ContractManagement.Services;
using SinaHRM.Application.Integrations.WorkflowEngine.Services;
using SinaHRM.Infrastracture;
using SinaHRM.Infrastracture.Data.Sql.Repositories;
using SinaHRM.WebAPI.BackgroundServices;

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("RazorClient", policy =>
    {
        policy
            .WithOrigins("http://localhost:5062")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
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
builder.Services.AddSingleton<IPersianDatePeriodService, PersianDatePeriodService>();
builder.Services.AddSingleton<IHtmlTemplateService, HtmlTemplateService>();
builder.Services.AddScoped<IContractManagementRepository, ContractManegementRepository>();
builder.Services.AddScoped<IContractManagementService, ContractManagementService>();
builder.Services.AddScoped<IEmploymentContractRepository, EmploymentContractRepository>();
builder.Services.AddScoped<IWorkflowInboxRepository, WorkflowInboxRepository>();
builder.Services.AddScoped<WorkflowInboxService>();
builder.Services.AddHostedService<WorkflowInboxReaperHostedService>();

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

app.UseCors("RazorClient");

app.UseAuthorization();

app.MapControllers();

app.Run();
