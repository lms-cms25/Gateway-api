using Gateway.Api.Endpoints;
using Gateway.Api.OpenApi;
using Gateway.Api.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCorsConfiguration();
builder.Services.AddOpenApiConfiguration();

var app = builder.Build();

app.UseCors("All");
app.UseHttpsRedirection();

app.MapOpenApiEndpoints();
app.MapGatewayEndpoints();

app.Run();