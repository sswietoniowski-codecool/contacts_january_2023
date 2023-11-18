using System.Net;
using System.Reflection;
using System.Text.Json;
using Contacts.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// add logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

// Add services to the container.
builder.Services.AddDbContext<ContactsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ContactsDb"));
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
});

builder.Services.AddScoped<IContactsRepository, ContactsRepository>();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>()!;

        policyBuilder
            .WithOrigins(origins)
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddControllers(configure =>
    {
        configure.CacheProfiles.Add("Any-60", 
            new CacheProfile
            {
                Location = ResponseCacheLocation.Any,
                Duration = 60
            });
    })
    .AddNewtonsoftJson(options =>
    {
        // required to prevent "Self referencing loop detected" error
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();

builder.Services.AddProblemDetails();

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
    configuration.ReadFrom.Services(services);
}, preserveStaticLogger: true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // should be added first:
    app.UseDeveloperExceptionPage();
}
else
{
    // should be added first:
    app.UseExceptionHandler(applicationBuilder =>
    {
        applicationBuilder.Run(async context =>
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            var problemDetails = new ProblemDetails
            {
                Title = "An unexpected error occurred!",
                Status = context.Response.StatusCode,
                Detail = "Please contact your system administrator!"
            };
            var problemDetailsJson = JsonSerializer.Serialize(problemDetails);
            // TODO: log the exception
            //await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
            await context.Response.WriteAsync(problemDetailsJson);
        });
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors();

app.UseResponseCaching();

app.MapControllers();

app.Run();
