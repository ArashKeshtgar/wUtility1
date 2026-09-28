using System.Security.Cryptography;
using System.Text;
using System.Threading.RateLimiting;
using SchemaSyncApi;

var builder = WebApplication.CreateBuilder(args);

// Every /api route requires the X-Api-Key header. Without it, /api/execute
// would run any SQL against any server reachable from this host for anyone
// who can reach this port. There is no fallback key: the app refuses to
// start without WUTILITY_API_KEY. Callers: Control Panel's wutility-adapter,
// and the Angular dev server's proxy (frontend/proxy.conf.js), which adds
// the header server-side so the key never ships to the browser.
var apiKey = builder.Configuration["WUTILITY_API_KEY"];
if (string.IsNullOrEmpty(apiKey) || apiKey.Length < 32)
{
    throw new InvalidOperationException(
        "WUTILITY_API_KEY must be set to a random value of at least 32 characters.");
}
var apiKeyBytes = Encoding.UTF8.GetBytes(apiKey);

// See DemoMode.cs: the public Azure deployment runs with Demo:Enabled=true.
var demo = builder.Configuration.GetSection("Demo").Get<DemoOptions>() ?? new DemoOptions();

builder.Services.AddOpenApi();
builder.Services.AddSingleton<SchemaReader>();
builder.Services.AddSingleton<SchemaDiffer>();
builder.Services.AddSingleton<ScriptGenerator>();
builder.Services.AddSingleton<ScriptExecutor>();
builder.Services.AddSingleton<DoctorsService>();
builder.Services.AddSingleton<DbConfigService>();
builder.Services.AddSingleton<ConnectionVault>();
builder.Services.AddSingleton<SiteConnectionsService>();
if (demo.Enabled)
{
    builder.Services.AddSingleton<DemoDatabases>();
    builder.Services.AddSingleton<DemoResetService>();
}

// Per-client-IP limits. On App Service the client IP comes from
// X-Forwarded-For, honoured via ASPNETCORE_FORWARDEDHEADERS_ENABLED=true.
// "heavy" covers the routes that do DDL or read whole schemas: the Azure SQL
// free offer has a monthly vCore budget and pauses the database when it runs
// out, so one visitor looping on them shouldn't be able to spend it.
const string HeavyPolicy = "heavy";
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
        RateLimitPartition.GetFixedWindowLimiter(
            ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions { PermitLimit = 120, Window = TimeSpan.FromMinutes(1) }));
    options.AddPolicy(HeavyPolicy, ctx =>
        RateLimitPartition.GetFixedWindowLimiter(
            ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions { PermitLimit = 10, Window = TimeSpan.FromMinutes(1) }));
});

const string AngularDevCors = "AngularDev";
builder.Services.AddCors(options =>
{
    options.AddPolicy(AngularDevCors, policy =>
        policy.WithOrigins("http://localhost:4300").AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(AngularDevCors);
app.UseRateLimiter();

// The built Angular app (copied into wwwroot at publish time) is served from
// the same origin as the API, so production needs no CORS and no proxy.
app.UseDefaultFiles();
app.UseStaticFiles();

// Liveness only (App Service health check). Deliberately doesn't touch SQL,
// so an auto-paused free-tier database doesn't mark the app unhealthy.
app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));

app.Use(async (context, next) =>
{
    // In demo mode the browser calls /api directly and can't hold a secret;
    // the dangerous routes are constrained instead (see DemoMode.cs).
    if (!demo.Enabled && context.Request.Path.StartsWithSegments("/api"))
    {
        var provided = Encoding.UTF8.GetBytes(context.Request.Headers["X-Api-Key"].ToString());
        // Constant-time comparison, so response timing doesn't leak the key.
        if (!CryptographicOperations.FixedTimeEquals(provided, apiKeyBytes))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Missing or invalid X-Api-Key header." });
            return;
        }
    }
    await next();
});

async Task<CompareResponse> CompareAsync(string sourceCs, string targetCs, SchemaReader reader, SchemaDiffer differ, ScriptGenerator generator)
{
    var source = await reader.ReadAsync(sourceCs);
    var target = await reader.ReadAsync(targetCs);
    var diffs = differ.Diff(source, target);
    return new CompareResponse(diffs, generator.Generate(diffs, source));
}

app.MapPost("/api/compare", async (CompareRequest req, IServiceProvider sp, SchemaReader reader, SchemaDiffer differ, ScriptGenerator generator) =>
{
    if (demo.Enabled)
    {
        var dbs = sp.GetRequiredService<DemoDatabases>();
        return Results.Ok(await CompareAsync(dbs.Source, dbs.Target, reader, differ, generator));
    }
    return Results.Ok(await CompareAsync(req.SourceConnectionString, req.TargetConnectionString, reader, differ, generator));
})
.WithName("CompareSchemas")
.RequireRateLimiting(HeavyPolicy);

app.MapPost("/api/execute", async (ExecuteRequest req, IServiceProvider sp, SchemaReader reader, SchemaDiffer differ, ScriptGenerator generator, ScriptExecutor executor) =>
{
    if (demo.Enabled)
    {
        // Only the script the server would generate right now may run. The
        // client must send back exactly what it was shown, so "review, then
        // apply" still holds, but nothing caller-written ever executes.
        var dbs = sp.GetRequiredService<DemoDatabases>();
        var current = await CompareAsync(dbs.Source, dbs.Target, reader, differ, generator);
        if (!string.Equals(NormalizeScript(req.Script), NormalizeScript(current.Script), StringComparison.Ordinal))
        {
            return Results.Conflict(new { error = "In demo mode only the unmodified generated script can run. Compare again and apply it as shown." });
        }
        return Results.Ok(await executor.ExecuteAsync(dbs.Target, current.Script));
    }
    return Results.Ok(await executor.ExecuteAsync(req.TargetConnectionString, req.Script));
})
.WithName("ExecuteScript")
.RequireRateLimiting(HeavyPolicy);

static string NormalizeScript(string? script) => (script ?? "").Replace("\r\n", "\n").Trim();

app.MapGet("/api/demo", (IServiceProvider sp) =>
{
    if (!demo.Enabled) return new DemoInfo(false, null, null);
    var dbs = sp.GetRequiredService<DemoDatabases>();
    return new DemoInfo(true, dbs.SourceName, dbs.TargetName);
})
.WithName("GetDemoInfo");

if (demo.Enabled)
{
    app.MapPost("/api/demo/reset", async (DemoResetService svc) =>
    {
        await svc.ResetAsync();
        return Results.Ok();
    })
    .WithName("ResetDemo")
    .RequireRateLimiting(HeavyPolicy);
}

app.MapGet("/api/doctors", async (DoctorsService svc) => await svc.GetDoctorsAsync())
    .WithName("GetDoctors");

app.MapGet("/api/works", async (DoctorsService svc) => await svc.GetWorksAsync())
    .WithName("GetWorks");

app.MapGet("/api/doctor-price-groups", async (int? drCode, DoctorsService svc) => await svc.GetPriceGroupsAsync(drCode))
    .WithName("GetDoctorPriceGroups");

app.MapPost("/api/doctor-price-groups", async (UpsertDoctorPriceGroupRequest req, DoctorsService svc) =>
{
    var rowGd = await svc.AddPriceGroupAsync(req);
    return Results.Ok(new { rowGd });
})
.WithName("AddDoctorPriceGroup");

app.MapPut("/api/doctor-price-groups/{rowGd:int}", async (int rowGd, UpsertDoctorPriceGroupRequest req, DoctorsService svc) =>
{
    await svc.UpdatePriceGroupAsync(rowGd, req);
    return Results.Ok();
})
.WithName("UpdateDoctorPriceGroup");

app.MapDelete("/api/doctor-price-groups/{rowGd:int}", async (int rowGd, DoctorsService svc) =>
{
    await svc.DeletePriceGroupAsync(rowGd);
    return Results.Ok();
})
.WithName("DeleteDoctorPriceGroup");

app.MapGet("/api/db-config-settings", async (DbConfigService svc) => await svc.GetSettingsAsync())
    .WithName("GetDbConfigSettings");

app.MapGet("/api/modules", () => WuModules.All)
    .WithName("GetModules");

app.MapGet("/api/connections", async (string? module, SiteConnectionsService svc) => await svc.ListAsync(module))
    .WithName("GetConnections");

// Adding or testing a stored connection string makes this host connect to
// wherever the string points, so the vault is read-only in demo mode.
IResult DemoReadOnly() => Results.Json(
    new { error = "The connection vault is read-only in the public demo." },
    statusCode: StatusCodes.Status403Forbidden);

app.MapPost("/api/connections", async (AddSiteConnectionRequest req, SiteConnectionsService svc) =>
{
    if (demo.Enabled) return DemoReadOnly();
    var id = await svc.AddAsync(req);
    return Results.Ok(new { id });
})
.WithName("AddConnection");

app.MapPost("/api/connections/{id:int}/test", async (int id, SiteConnectionsService svc) =>
    demo.Enabled ? DemoReadOnly() : Results.Ok(await svc.TestAsync(id)))
    .WithName("TestConnection");

app.MapDelete("/api/connections/{id:int}", async (int id, SiteConnectionsService svc) =>
{
    if (demo.Enabled) return DemoReadOnly();
    await svc.DeleteAsync(id);
    return Results.Ok();
})
.WithName("DeleteConnection");

// Client-side routes (/schema-sync, /doctors, ...) get index.html, but only
// when the Angular build is present, i.e. in a published deployment.
if (File.Exists(Path.Combine(app.Environment.WebRootPath ?? "wwwroot", "index.html")))
{
    app.MapFallbackToFile("index.html");
}

app.Run();
