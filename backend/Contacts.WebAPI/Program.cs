using AutoMapper;
using Contacts.WebAPI.Configurations.Options;
using Contacts.WebAPI.DTOs;
using Contacts.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using System.Net;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;

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

//builder.Services.Configure<CorsConfiguration>(builder.Configuration.GetSection("Cors"));

builder.Services.AddOptions<CorsConfiguration>()
    .Bind(builder.Configuration.GetSection("Cors"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        //var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>()!;

        var origins = new List<string>();

        builder.Configuration.Bind("Cors:Origins", origins);

        policyBuilder
            .WithOrigins(origins.ToArray())
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

app.MapGet("api/contacts", Results<Ok<IEnumerable<ContactDto>>, BadRequest> ([FromQuery] string ? search, 
    IContactsRepository repository, IMapper mapper) =>
{
    var contacts = repository.GetContacts(search);

    var contactsDto = mapper.Map<IEnumerable<ContactDto>>(contacts);

    return TypedResults.Ok(contactsDto);
});

app.MapGet("api/contacts/{id:int}", Results<Ok<ContactDetailsDto>, NotFound, BadRequest> (int id,
    IContactsRepository repository, IMapper mapper,
    ILogger<WebApplication> logger,
    IMemoryCache memoryCache) =>
{
    logger.LogInformation("Getting contact with id {id}", id);

    var cacheKey = $"Contacts-{id}";

    if (!memoryCache.TryGetValue<ContactDetailsDto>(cacheKey, out var contactDto))
    {
        logger.LogWarning("Contact with id {id} was not found in cache. Retrieving from database", id);

        var contact = repository.GetContact(id);

        if (contact is not null)
        {
            contactDto = mapper.Map<ContactDetailsDto>(contact);

            memoryCache.Set(cacheKey, contactDto, TimeSpan.FromSeconds(60));
        }
    }

    if (contactDto is null)
    {
        logger.LogError("Contact with id {id} was not found in database", id);

        return TypedResults.NotFound();
    }

    return TypedResults.Ok(contactDto);
});

app.Run();
