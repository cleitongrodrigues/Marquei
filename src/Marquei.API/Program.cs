using Marquei.Application.Abstractions.Messaging;
using Marquei.Application.Tenants.CreateTenant;
using Marquei.Application.Tenants.GetTenantBySlug;
using Marquei.Domain.Common;
using Marquei.Domain.Tenants;
using Marquei.Infra.Persistence;
using Marquei.Infra.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
builder.Services.AddScoped<ITenantRepository, TenantRepository>();

builder.Services.AddScoped<
    ICommandHandler<CreateTenantCommand, Guid>,
    CreateTenantCommandHandler>();

builder.Services.AddScoped<
    IQueryHandler<GetTenantBySlugQuery, TenantPublicDto?>,
    GetTenantBySlugQueryHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (DomainException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
});

app.MapPost("/tenants", async (
    CreateTenantRequest request,
    ICommandHandler<CreateTenantCommand, Guid> handler) =>
{
    var id = await handler.HandleAsync(
        new CreateTenantCommand(request.Name, request.Slug));

    return Results.Created($"/tenants/{request.Slug}", new { id });
});

app.MapGet("/tenants/{slug}", async (
    string slug,
    IQueryHandler<GetTenantBySlugQuery, TenantPublicDto?> handler) =>
{
    var tenant = await handler.HandleAsync(new GetTenantBySlugQuery(slug));
    return tenant is null ? Results.NotFound() : Results.Ok(tenant);
});

app.Run();

public record CreateTenantRequest(string Name, string Slug);