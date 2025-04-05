using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PersistenceNet.Constants;
using PersistenceNet.Enuns;
using PersistenceNet.Structs;
using PersistenceNet.Test.Domain;
using PersistenceNet.Test.Domain.ViewModels;
using PersistenceNet.Test.Domain.Repositorys;
using PersistenceNet.Test.Domain.Services;
using PersistenceNet.Test.Domain.AppServices;
using PersistenceNet.Test.Domain.Mapper;
using PersistenceNet.CrossCutting.Logging;
using PersistenceNet.Test.Middleware;
using PersistenceNet.Interfaces;
using PersistenceNet.Test.Domain.Transaction;
using Asp.Versioning;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextPool<ContextTest>(options => {
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("connName"),
        b => b.MigrationsAssembly("PersistenceNet.Test")
    );
});

builder.Services.AddSingleton(typeof(IDatabaseContext), typeof(ContextTest));

builder.Services.AddHttpClient();

builder.Services.AddScoped(typeof(ITransactionWork), typeof(TransactionWork));
builder.Services.AddScoped(typeof(IClassificationRepository), typeof(ClassificationRepository));
builder.Services.AddScoped(typeof(IClassificationService), typeof(ClassificationService));
builder.Services.AddScoped(typeof(IClassificationAppService), typeof(ClassificationAppService));

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Logging.AddProvider(new FileLoggerProvider("Logs"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo()
    {
        Version = "v1",
        Title = "PersistenceNet.Test",
        Description = "APIs - Component responsible for managing data persistence.",
        Contact = new() { Name = "PersistenceNet.Test", Email = "text@persistencenet.test.com.br", Url = new Uri("https://persistencenet.test.com.br") }
    });
});
builder.Services.AddApiVersioning(option =>
{
    option.ReportApiVersions = true;
    option.AssumeDefaultVersionWhenUnspecified = true;
    option.DefaultApiVersion = new ApiVersion(1, 0);
});

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IMemoryCache, MemoryCache>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

app.MapPost("/classifications/createorreplace", async (
    IClassificationAppService classificationAppService,
    ClassificationViewModel classification) =>
{
    var operationReturn = await classificationAppService.CreateOrUpdateAsync(classification);

    return operationReturn.IsSuccess ? Results.Ok(operationReturn) : Results.BadRequest(operationReturn);
})
.Produces<OperationReturn>(StatusCodes.Status200OK)
.Produces<OperationReturn>(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status500InternalServerError)
.WithName("PostClassification")
.WithTags("Classification");

app.MapGet("/classifications/{id}", async (
    int id,
    IClassificationAppService classificationAppService) =>
{
    OperationReturn operationReturn = new() { EntityName = "Classification", ReturnType = ReturnTypeEnum.Empty, Key = $"{id}", Field = "id" };

    try
    {
        return Results.Ok(await classificationAppService.GetEntityByIdAsync(id));
    }
    catch(Exception ex)
    {
        operationReturn.Messages.Add(new() { ReturnType = ReturnTypeEnum.Empty, Code = Codes._WARNING, Text = ex.Message });
    }

    return Results.BadRequest(operationReturn);
})
.Produces<ClassificationViewModel>(StatusCodes.Status200OK)
.Produces<OperationReturn>(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status500InternalServerError)
.WithName("GetClassification")
.WithTags("Classification");

app.MapGet("/classifications/{page}/{pageSize}/descriptions/{name}", async (
    int page,
    int pageSize,
    string name,
    IClassificationAppService classificationAppService) =>
{
    OperationReturn operationReturn = new() { EntityName = "Classification", ReturnType = ReturnTypeEnum.Empty, Key = $"{name}", Field = "name" };

    try
    {
        return Results.Ok(await classificationAppService.Filter(find => find.Name!.Contains(name) && find.Active == ActiveEnum.S, page, pageSize));
    }
    catch (Exception ex)
    {
        operationReturn.Messages.Add(new() { ReturnType = ReturnTypeEnum.Empty, Code = Codes._WARNING, Text = ex.Message });
    }

    return Results.BadRequest(operationReturn);
})
.Produces<ClassificationViewModel[]>(StatusCodes.Status200OK)
.Produces<OperationReturn>(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status500InternalServerError)
.WithName("GetClassificationForName")
.WithTags("Classification");

app.MapGet("/classifications/{page}/{pageSize}", async (
    int page,
    int pageSize,
    IClassificationAppService classificationAppService) =>
{
    OperationReturn operationReturn = new() { EntityName = "Classification", ReturnType = ReturnTypeEnum.Empty };

    try
    {
        return Results.Ok(await classificationAppService.Paginate(page, pageSize));
    }
    catch (Exception ex)
    {
        operationReturn.Messages.Add(new() { ReturnType = ReturnTypeEnum.Empty, Code = Codes._WARNING, Text = ex.Message });
    }

    return Results.BadRequest(operationReturn);
})
.Produces<ClassificationViewModel[]>(StatusCodes.Status200OK)
.Produces<OperationReturn>(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status500InternalServerError)
.WithName("GetClassificationAll")
.WithTags("Classification");

app.Run();