#region Usings

using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

#endregion

namespace TaskManagerBackend.Application.Utility;

public class SwaggerAuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        bool hasControllerAuthorize = context.MethodInfo.DeclaringType is not null &&
                                      context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                                                                      .OfType<AuthorizeAttribute>()
                                                                      .Any();
        bool hasMethodAuthorize = context.MethodInfo.GetCustomAttributes(true)
                                                    .OfType<AuthorizeAttribute>()
                                                    .Any();
        bool hasControllerAnonymous = context.MethodInfo.DeclaringType is not null && 
                                      context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                                                                      .OfType<AllowAnonymousAttribute>()
                                                                      .Any();
        bool hasMethodAnonymous = context.MethodInfo.GetCustomAttributes(true)
                                                    .OfType<AllowAnonymousAttribute>()
                                                    .Any();

        if (hasControllerAnonymous || hasMethodAnonymous ||
            !(hasControllerAuthorize || hasMethodAuthorize))
        {
            return;
        }

        operation.Responses.TryAdd(nameof(HttpStatusCode.Unauthorized),
                                   new OpenApiResponse
                                   {
                                       Description = HttpStatusCode.Unauthorized.GetDisplayName()
                                   });
        operation.Responses.TryAdd(nameof(HttpStatusCode.Forbidden),
                                   new OpenApiResponse
                                   {
                                       Description = HttpStatusCode.Forbidden.GetDisplayName()
                                   });

        OpenApiSecurityRequirement requirement = new();
        OpenApiSecurityScheme scheme = new()
                                       {
                                           Reference = new OpenApiReference
                                                       {
                                                           Id = "bearerAuth",
                                                           Type = ReferenceType.SecurityScheme
                                                       }
                                       };
        requirement.Add(scheme, new List<string>());
        operation.Security = new List<OpenApiSecurityRequirement>
                             {
                                 requirement
                             };
    }
}