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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ContextTest>(options =>
{
    /* The example here uses MySQL, but can be changed... */
    options.UseMySql(builder.Configuration.GetConnectionString("connName")
        , ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("connName")));
});

builder.Services.AddScoped(typeof(IClassificationRepository), typeof(ClassificationRepository));
builder.Services.AddScoped(typeof(IClassificationService), typeof(ClassificationService));
builder.Services.AddScoped(typeof(IClassificationAppService), typeof(ClassificationAppService));

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Logging.ClearProviders();
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

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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
    var objReturn = await classificationAppService.GetEntityByIdAsync(id);

    if (objReturn is not null)
    {
        OperationReturn operationReturn = new() { EntityName = "Classification", ReturnType = ReturnTypeEnum.Empty, Key = $"{id}", Field = "id" };
        operationReturn.Messages.Add(new() { ReturnType = ReturnTypeEnum.Empty, Code = Codes._WARNING, Text = $"Classification '{id}' not found!" });
        return Results.BadRequest(operationReturn);
    }

    return Results.Ok(objReturn);
})
.Produces<ClassificationViewModel>(StatusCodes.Status200OK)
.Produces<OperationReturn>(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status500InternalServerError)
.WithName("GetClassification")
.WithTags("Classification");

app.MapGet("/classifications/{page}/{pageSize}", async (
    int page,
    int pageSize,
    IClassificationAppService classificationAppService) =>
{
    var listReturn = await classificationAppService.Paginate(page, pageSize);

    if (listReturn.IsNullOrZero())
    {
        OperationReturn operationReturn = new() { EntityName = "Classification", ReturnType = ReturnTypeEnum.Empty };
        operationReturn.Messages.Add(new() { ReturnType = ReturnTypeEnum.Empty, Code = Codes._WARNING, Text = "No classification was found!" });
        return Results.BadRequest(operationReturn);
    }

    return Results.Ok(listReturn);
})
.Produces<ClassificationViewModel[]>(StatusCodes.Status200OK)
.Produces<OperationReturn>(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status500InternalServerError)
.WithName("GetClassificationAll")
.WithTags("Classification");

app.Run();