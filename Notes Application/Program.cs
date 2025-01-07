using Elastic.Clients.Elasticsearch.Nodes;
using Elastic.Clients.Elasticsearch;
using Microsoft.EntityFrameworkCore;
using Nest;
using Notes_Application.Models.Data;
using Notes_Application.Models.Entities;
using Elastic.Clients.Elasticsearch.TransformManagement;

var builder = WebApplication.CreateBuilder(args);
var defaultIndex = builder.Configuration["Elasticsearch:Index"];

var elasticSettings = new ConnectionSettings(new Uri(builder.Configuration["Elasticsearch:Uri"]))
            .BasicAuthentication("elastic", "elastic12345")
    .DefaultIndex(defaultIndex)
    .ServerCertificateValidationCallback((o, cert, chain, errors) => true);
var elasticClient = new ElasticClient(elasticSettings);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<NotesDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddSingleton<IElasticClient>(elasticClient);

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<INoteService, NoteService>();

var createIndexResponse = elasticClient.Indices.Create(defaultIndex,
                index => index.Map<Note>(x => x.AutoMap())
            );


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
