using MongoDB.Bson;
using NintendoGamesLABB2.Data;
using NintendoGamesLABB2.Models;

namespace NintendoGamesLABB2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddAuthorization();

            // Swagger for API documentation
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Inject MongoCRUD with connection string from appsettings.json to not exponse my string
            builder.Services.AddSingleton<MongoCRUD>(serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                return new MongoCRUD(configuration);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            // POST method to add a new game
            app.MapPost("/game", async (NintendoGame game, MongoCRUD db) =>
            {
                var addedGame = await db.AddGame("Games", game);
                return Results.Ok(addedGame);
            });

            // GET all games
            app.MapGet("/games", async (MongoCRUD db) =>
            {
                var games = await db.GetAllGames("Games");
                return Results.Ok(games);
            });

            // GET game by ID 
            app.MapGet("/game/{id}", async (string id, MongoCRUD db) =>
            {
                // Convert the string ID to ObjectId
                if (!ObjectId.TryParse(id, out ObjectId objectId))
                {
                    return Results.BadRequest("Invalid ID format.");
                }

                var game = await db.GetGameById("Games", objectId);
                if (game == null)
                    return Results.NotFound($"Game with ID {id} not found.");
                return Results.Ok(game);
            });

            // UPDATE game details 
            app.MapPut("/game", async (NintendoGame updatedGame, MongoCRUD db) =>
            {
                var existingGame = await db.GetGameById("Games", updatedGame.Id);
                if (existingGame == null)
                    return Results.NotFound($"Game with ID {updatedGame.Id} not found.");

                var updated = await db.UpdateGame("Games", updatedGame);
                return Results.Ok(updated);
            });

            // DELETE game by ID 
            app.MapDelete("/game/{id}", async (string id, MongoCRUD db) =>
            {
                // Convert the string ID to ObjectId
                if (!ObjectId.TryParse(id, out ObjectId objectId))
                {
                    return Results.BadRequest("Invalid ID format.");
                }

                var existingGame = await db.GetGameById("Games", objectId);
                if (existingGame == null)
                    return Results.NotFound($"Game with ID {id} not found.");

                var deleted = await db.DeleteGame("Games", objectId);
                return Results.Ok(deleted);
            });

            app.Run();
        }
    }
}
