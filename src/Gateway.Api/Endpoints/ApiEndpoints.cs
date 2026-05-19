namespace Gateway.Api.Endpoints;

public static class ApiEndpoints
{
    public static void MapGatewayEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/gateway")
            .WithTags("Gateway")
            .WithDescription("How to use the Gateway");
    }
}
