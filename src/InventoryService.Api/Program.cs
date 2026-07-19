using InventoryService.Application;
using InventoryService.Infrastructure;
using InventoryService.Infrastructure.Messaging.Topology;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//to define the exhange when application starts
var initializer = app.Services.GetRequiredService<IRabbitMqTopologyInitializer>();
await initializer.InitializeAsync();

app.MapControllers();
//app.UseHttpsRedirection();

app.Run();
