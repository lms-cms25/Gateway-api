# Gateway API

API-gateway för Shiko LMS. Gatewayen samlar mikrotjänsterna bakom en gemensam adress och vidarebefordrar anrop till rätt tjänst med [YARP](https://microsoft.github.io/reverse-proxy/). Den kan också validera JWT-tokens som utfärdats av identity-tjänsten.

> **Obs:** Gatewayen användes inte fullt ut i projektet. Delar av frontend anropar mikrotjänsterna direkt, så alla anrop går inte via gatewayen.

## Teknik

- .NET 10 / ASP.NET Core
- YARP (`Yarp.ReverseProxy`) – reverse proxy
- JWT Bearer-autentisering
- OpenAPI + [Scalar](https://scalar.com/) för dokumentation
- Health checks

## Projektstruktur

```
src/Gateway.Api/
├── Endpoints/        # Gatewayens egna endpoints (health check)
├── OpenApi/          # OpenAPI-dokument och Scalar-UI
├── Security/         # JWT- och CORS-konfiguration
├── YarpRoutes/       # En routningsfil per mikrotjänst
├── appsettings.json
└── Program.cs
```

## Routes

Alla anrop skickas vidare med samma sökväg till respektive tjänst.

| Sökväg                     | Tjänst          | Kräver            |
|----------------------------|-----------------|-------------------|
| `/api/auth/**`             | Identity        | –                 |
| `/api/admin/**`            | Identity        | `AdminOnly`-policy |
| `/api/courses/**`          | Course          | –                 |
| `/api/enrollment/**`       | Enrollment      | –                 |
| `/api/image/**`            | Image           | –                 |
| `/api/notifications/**`    | Notifications   | –                 |
| `/api/profile/**`          | Profile         | –                 |
| `/api/reviews/**`          | Reviews         | –                 |
| `/api/Verification/**`     | Verification    | –                 |

Måladresserna finns i respektive fil under [`YarpRoutes/`](src/Gateway.Api/YarpRoutes). Filerna laddas med `reloadOnChange: true`, så ändringar slår igenom utan omstart.

### Gatewayens egna endpoints

| Sökväg                | Beskrivning            |
|-----------------------|------------------------|
| `/api/gateway/health` | Health check           |
| `/openapi/v1.json`    | OpenAPI-dokument       |
| `/docs`               | Scalar API-dokumentation |

## Autentisering

Gatewayen validerar JWT-tokens (issuer, audience, livslängd och signatur). Rollen läses från `ClaimTypes.Role`.

| Policy      | Krav              |
|-------------|-------------------|
| `AdminOnly` | Rollen `Admin`    |

En policy kopplas till en route med `"AuthorizationPolicy"` i YARP-filen, till exempel:

```json
"identity-admin": {
  "ClusterId": "identity",
  "Match": { "Path": "/api/admin/{**catch-all}" },
  "AuthorizationPolicy": "AdminOnly"
}
```

## Konfiguration

`Issuer` och `Audience` ligger i `appsettings.json`. **Signeringsnyckeln ska aldrig ligga i koden eller i git.**

| Nyckel             | Exempel          | Var                        |
|--------------------|------------------|----------------------------|
| `Jwt:Issuer`       | `auth-api`       | `appsettings.json`         |
| `Jwt:Audience`     | `microservices`  | `appsettings.json`         |
| `Jwt:SigningKey`   | *(base64-sträng)* | User secrets / miljövariabel |

`Jwt:SigningKey` måste vara samma nyckel som identity-tjänsten signerar med, base64-kodad. Saknas den startar inte appen.

### Lokalt (user secrets)

```bash
dotnet user-secrets init --project src/Gateway.Api
dotnet user-secrets set "Jwt:SigningKey" "<base64-nyckel>" --project src/Gateway.Api
```

### Azure / Docker (miljövariabel)

```
Jwt__SigningKey=<base64-nyckel>
```

## Kom igång

```bash
dotnet run --project src/Gateway.Api --launch-profile https
```

Appen startar på `https://localhost:7047` (och `http://localhost:5193`) och öppnar `/docs` i webbläsaren.

## Lägga till en ny tjänst

1. Skapa `src/Gateway.Api/YarpRoutes/yarp.<tjänst>.json`:

   ```json
   {
     "ReverseProxy": {
       "Routes": {
         "<tjänst>": {
           "ClusterId": "<tjänst>",
           "Match": { "Path": "/api/<tjänst>/{**catch-all}" }
         }
       },
       "Clusters": {
         "<tjänst>": {
           "Destinations": {
             "destination1": { "Address": "https://<tjänst-url>/" }
           }
         }
       }
     }
   }
   ```

2. Registrera filen i `Program.cs`:

   ```csharp
   builder.Configuration.AddJsonFile("YarpRoutes/yarp.<tjänst>.json", optional: false, reloadOnChange: true);
   ```

## CORS

Policyn `All` tillåter alla origins, headers och metoder. Det är praktiskt under utveckling, men bör begränsas till frontendens adress i produktion.
