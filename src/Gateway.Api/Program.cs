using Gateway.Api.Endpoints;
using Gateway.Api.OpenApi;
using Gateway.Api.Security;

var builder = WebApplication.CreateBuilder(args);

// add configuration for YARP here, so it can be reloaded when the file changes
builder.Configuration.AddJsonFile("YarpRoutes/yarp.courses.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile("YarpRoutes/yarp.enrollment.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile("YarpRoutes/yarp.identity.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile("YarpRoutes/yarp.image.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile("YarpRoutes/yarp.notification.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile("YarpRoutes/yarp.profile.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile("YarpRoutes/yarp.reviews.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile("YarpRoutes/yarp.verification.json", optional: false, reloadOnChange: true);

builder.Services.AddCorsConfiguration();
builder.Services.AddOpenApiConfiguration();
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddJwtConfiguration();

var app = builder.Build();

app.UseCors("All");
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApiEndpoints();
app.MapGatewayEndpoints();
app.MapReverseProxy();

app.Run();