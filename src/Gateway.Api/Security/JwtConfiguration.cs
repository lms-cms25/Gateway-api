using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Gateway.Api.Security;

public static class JwtConfiguration
{
    public static IServiceCollection AddJwtConfiguration(this IServiceCollection services)
    {

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "auth-api", // Hämtat från din token!
                ValidAudience = "microservices", // Hämtat från din token!
               // IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("VGgxcy0xNS1tWS00VXRoLVMzclYxQ0UtNTFnbjFuSzN5"))
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String("VGgxcy0xNS1tWS00VXRoLVMzclYxQ0UtNTFnbjFuSzN5"))
            };
        });

        // HÄR LÄGGER VI TILL ERA POLICIES!
        services.AddAuthorization(options =>
        {
            // Vi skapar en regel som heter "AdminOnly"
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole("Admin")); // Kräver att roll-claimet inuti token innehåller "Admin"

            // Om ni vill, kan ni även förbereda en för studenter här i framtiden:
            // options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
        });

        return services;
    }
}
