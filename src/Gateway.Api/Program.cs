using Gateway.Api.Endpoints;
using Gateway.Api.OpenApi;
using Gateway.Api.Security;

var builder = WebApplication.CreateBuilder(args);

// add configuration for YARP here, so it can be reloaded when the file changes
builder.Configuration.AddJsonFile("YarpRoutes/yarp.verification.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile("YarpRoutes/yarp.identity.json", optional: false, reloadOnChange: true);

builder.Services.AddCorsConfiguration();
builder.Services.AddOpenApiConfiguration();
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors("All");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApiEndpoints();
app.MapGatewayEndpoints();
app.MapReverseProxy();

app.Run();