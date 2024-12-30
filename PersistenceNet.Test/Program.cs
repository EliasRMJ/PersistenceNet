using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PersistenceNet.Constants;
using PersistenceNet.Enuns;
using PersistenceNet.Structs;
using PersistenceNet.Test.Domain;
using PersistenceNet.Test.Domain.Managers;
using PersistenceNet.Test.Domain.Views;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ContextTest>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("connName")
        , ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("connName")));
});
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

app.UseHttpsRedirection();

app.MapPost("/classification/createorreplace", async (
    ContextTest contextTest,
    ClassificationView classification) =>
{
    var classificationManager = new ClassificationManager(contextTest);
    var operationReturn = await classificationManager.CreateOrReplace(classification);

    return operationReturn.IsSuccess ? Results.Ok(operationReturn) : Results.BadRequest(operationReturn);
})
.Produces<OperationReturn>(StatusCodes.Status200OK)
.Produces<OperationReturn>(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status500InternalServerError)
.WithName("PostClassification")
.WithTags("Classification");

app.MapGet("/classification/{id}", async (
    int id,
    ContextTest contextTest) =>
{
    var classificationManager = new ClassificationManager(contextTest);
    var objReturn = await classificationManager.Get(id);

    if (objReturn.Id.Equals(0))
    {
        var operationReturn = new OperationReturn { EntityName = "Classification", ReturnType = ReturnTypeEnum.Empty, Key = $"{id}", Field = "id" };
        operationReturn.Messages.Add(new() { ReturnType = ReturnTypeEnum.Empty, Code = Codes._WARNING, Text = $"Classification '{id}' not found!" });
        return Results.BadRequest(operationReturn);
    }

    return Results.Ok(objReturn);
})
.Produces<ClassificationView>(StatusCodes.Status200OK)
.Produces<OperationReturn>(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status500InternalServerError)
.WithName("GetClassification")
.WithTags("Classification");

app.MapGet("/classification/all", async (
    ContextTest contextTest) =>
{
    var classificationManager = new ClassificationManager(contextTest);
    var listReturn = await classificationManager.List();

    if (listReturn.Length.Equals(0))
    {
        var operationReturn = new OperationReturn { EntityName = "Classification", ReturnType = ReturnTypeEnum.Empty };
        operationReturn.Messages.Add(new() { ReturnType = ReturnTypeEnum.Empty, Code = Codes._WARNING, Text = "No classification was found!" });
        return Results.BadRequest(operationReturn);
    }

    return Results.Ok(listReturn);
})
.Produces<ClassificationView[]>(StatusCodes.Status200OK)
.Produces<OperationReturn>(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status500InternalServerError)
.WithName("GetClassificationAll")
.WithTags("Classification");

app.Run();