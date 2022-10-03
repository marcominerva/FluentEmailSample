using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Fluent Email",
        Version = "v1",
        Description = "A Web API that uses FluentEmail to send e-mails, with the support for Sendinblue",

        Contact = new OpenApiContact
        {
            Name = "Marco Minerva"
        },

        License = new OpenApiLicense
        {
            Name = "Licenza MIT",
            Url = new Uri("https://it.wikipedia.org/wiki/Licenza_MIT"),
        }
    });

    options.UseAllOfToExtendReferenceSchemas();
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

builder.Services.AddFluentEmail("noreply@email.com")
    .AddRazorRenderer()
    //.AddSendGridSender(apiKey: "", sandBoxMode: true)
    .AddSendinblueSender(apiKey: "");

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

app.Run();