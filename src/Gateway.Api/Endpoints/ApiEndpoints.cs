namespace Gateway.Api.Endpoints;

public static class ApiEndpoints
{
    public static WebApplication MapGatewayEndpoints(this WebApplication app)
    {
        // unused for now, but we can add some custom endpoints here in the future if needed
        // Api Healtcheck, Metrics, etc.
        var group = app.MapGroup("/api/gateway")
            .WithTags("Gateway")
            .WithDescription("Gateway APIs");

        return app;
    }
}

