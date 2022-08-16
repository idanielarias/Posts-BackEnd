using Posts.BackEnd.Data;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CORSPolicy",
        builder =>
        {
            builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithOrigins("http://localhost:3000", "https://posts-backend.azurestaticapps.net", "https://posts-backend.azurewebsites.net");

        });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swaggerGenOptions =>
{
    swaggerGenOptions.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Posts", Version = "v0.1.3" });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(swaggerUIOptions =>
{
    swaggerUIOptions.DocumentTitle = "Posts";
    swaggerUIOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API serving a simple Post model");
    swaggerUIOptions.RoutePrefix = String.Empty;
});

app.UseHttpsRedirection();

app.UseCors("CORSPolicy");

app.MapGet("/get-all-posts", async () => await PostsRepository.GetPostsAsync())
    .WithTags("Posts Endpoints");

app.MapGet("get-post-by-id/{postId}", async (int postId) =>
{
    Post postToReturn = await PostsRepository.GetPostByIdAsync(postId);
    if (postToReturn != null)
    {
        return Results.Ok(postToReturn);
    }
    else
    {
        return Results.BadRequest();
    }
}).WithTags("Posts Endpoints");

app.MapPost("/create-post", async (Post postToCreate) =>
{
    bool createSucessful = await PostsRepository.CreatePostAsync(postToCreate);
    
    if (createSucessful)
    {
        return Results.Ok("Create sucessful");
    }
    else
    {
        return Results.BadRequest();
    }
}).WithTags("Posts Endpoints");

app.MapPut("/update-post", async (Post postToUpdate) =>
{
    bool UpdateSucessful = await PostsRepository.UpdatePostAsync(postToUpdate);
    
    if (UpdateSucessful)
    {
        return Results.Ok("Update sucessful");
    }
    else
    {
        return Results.BadRequest();
    }
}).WithTags("Posts Endpoints");

app.MapDelete("/delete-post/{postId}", async (int postId) =>
{
    bool deleteSucessful = await PostsRepository.DeletePostAsync(postId);

    if (deleteSucessful)
    {
        return Results.Ok("Delete sucessful");
    }
    else
    {
        return Results.BadRequest();
    }
}).WithTags("Posts Endpoints");

app.Run();
