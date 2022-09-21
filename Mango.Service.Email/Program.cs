using Mango.Service.Email.DbContexts;
using Mango.Service.Email.Extensions;
using Mango.Service.Email.Messagin;
using Mango.Service.Email.Repository;
using Mango.Services.EmailAPI.Messagin;
using Mango.Services.OrderAPI.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Enter bearer token",
        Name = "Auth",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },
              Scheme = "oauth2",
              Name = "Bearer",
              In = ParameterLocation.Header,

            },
            new List<string>()
          }
        });
});

//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var optionBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
optionBuilder.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddScoped<IEmailRepository, EmailRepository>();
builder.Services.AddSingleton(new EmailRepository(optionBuilder.Options));
builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();
//builder.Services.AddSingleton<IRabbitMQEmailMessageSender, RabbitMQEmailMessageSender>();
builder.Services.AddHostedService<RabbitMQEmailConsumer>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseAzureServiceBusConsumer();

app.Run();
