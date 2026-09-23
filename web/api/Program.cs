using SchemaSyncApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<SchemaReader>();
builder.Services.AddSingleton<SchemaDiffer>();
builder.Services.AddSingleton<ScriptGenerator>();
builder.Services.AddSingleton<ScriptExecutor>();
builder.Services.AddSingleton<DoctorsService>();
builder.Services.AddSingleton<DbConfigService>();
builder.Services.AddSingleton<ConnectionVault>();
builder.Services.AddSingleton<SiteConnectionsService>();

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

app.MapPost("/api/compare", async (CompareRequest req, SchemaReader reader, SchemaDiffer differ, ScriptGenerator generator) =>
{
    var source = await reader.ReadAsync(req.SourceConnectionString);
    var target = await reader.ReadAsync(req.TargetConnectionString);
    var diffs = differ.Diff(source, target);
    var script = generator.Generate(diffs, source);
    return Results.Ok(new CompareResponse(diffs, script));
})
.WithName("CompareSchemas");

app.MapPost("/api/execute", async (ExecuteRequest req, ScriptExecutor executor) =>
{
    var result = await executor.ExecuteAsync(req.TargetConnectionString, req.Script);
    return Results.Ok(result);
})
.WithName("ExecuteScript");

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

app.MapPost("/api/connections", async (AddSiteConnectionRequest req, SiteConnectionsService svc) =>
{
    var id = await svc.AddAsync(req);
    return Results.Ok(new { id });
})
.WithName("AddConnection");

app.MapPost("/api/connections/{id:int}/test", async (int id, SiteConnectionsService svc) => await svc.TestAsync(id))
    .WithName("TestConnection");

app.MapDelete("/api/connections/{id:int}", async (int id, SiteConnectionsService svc) =>
{
    await svc.DeleteAsync(id);
    return Results.Ok();
})
.WithName("DeleteConnection");

app.Run();
