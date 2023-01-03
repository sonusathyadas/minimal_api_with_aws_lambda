
using TodoApi.Data;
using TodoApi.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TodoApi.ApisExtensions;

public static class TodoApiExtensions
{
    public static WebApplication MapTodoApi(this WebApplication app)
    {

        app.MapGet("/api/todos", async (TodoDb db) =>
            await db.Todos.ToListAsync())
            .Produces<Todo>(StatusCodes.Status200OK)
            .WithName("GetAllTodos")
            .WithTags("TodoGroup");

        app.MapGet("/api/todos/complete", async (TodoDb db) =>
            await db.Todos.Where(t => t.IsComplete).ToListAsync())
            .Produces<Todo>(StatusCodes.Status200OK)
            .WithName("GetCompletedTodos")
            .WithTags("TodoGroup");

        app.MapGet("/api/todos/{id}", async (int id, TodoDb db) =>
            await db.Todos.FindAsync(id) is Todo todo
                    ? Results.Ok(todo)
                    : Results.NotFound())
                    .Produces<Todo>(StatusCodes.Status200OK)
                    .Produces(StatusCodes.Status404NotFound)
                    .WithName("GetTodoById")
                    .WithTags("TodoGroup"); 

        app.MapPost("/api/todos", async (Todo todo, TodoDb db) =>
        {
            db.Todos.Add(todo);
            await db.SaveChangesAsync();
            return Results.Created($"/todos/{todo.Id}", todo);
        })
        .Produces<Todo>(StatusCodes.Status201Created)
        .WithName("AddTodoItem")
        .WithTags("TodoGroup");

        app.MapPut("/api/todos/{id}", async (int id, Todo inputTodo, TodoDb db) =>
        {
            var todo = await db.Todos.FindAsync(id);

            if (todo is null) return Results.NotFound();

            todo.Name = inputTodo.Name;
            todo.IsComplete = inputTodo.IsComplete;

            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .Produces<Todo>(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .WithName("UpdateTodoItem")
        .WithTags("TodoGroup");

        app.MapDelete("/api/todos/{id}", async (int id, TodoDb db) =>
        {
            if (await db.Todos.FindAsync(id) is Todo todo)
            {
                db.Todos.Remove(todo);
                await db.SaveChangesAsync();
                return Results.Ok(todo);
            }

            return Results.NotFound();
        })
        .Produces<Todo>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithName("DeleteTodoItem")
        .WithTags("TodoGroup");

        return app;
    }
}