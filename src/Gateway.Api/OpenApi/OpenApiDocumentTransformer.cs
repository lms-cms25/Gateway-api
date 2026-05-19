﻿using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Gateway.Api.OpenApi;

public sealed class OpenApiDocumentTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();

        document.Info.Description = """
        ## Introduction

        The Gateway provides easy access to all APIs.

        """;

        return Task.CompletedTask;
    }
}