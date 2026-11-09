namespace Gateway.Api.Endpoints;

public static class ApiEndpoints
{
    public static WebApplication MapGatewayEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/api/gateway/health");
        return app;
    }
}