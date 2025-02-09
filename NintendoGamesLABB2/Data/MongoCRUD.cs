using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using NintendoGamesLABB2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NintendoGamesLABB2.Data
{
    public class MongoCRUD
    {
        private readonly IMongoDatabase _database;

        public MongoCRUD(IConfiguration configuration)
        {
            var connectionString = configuration["MongoDB_ConnectionString"]; // Retrieve from configuration
            var databaseName = configuration["MongoDB_DatabaseName"]; // Retrieve from configuration

            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(databaseName))
            {
                throw new InvalidOperationException("MongoDB connection string or database name is not set in the environment variables.");
            }

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        // Get the collection by name
        public IMongoCollection<NintendoGame> GetCollection(string collectionName)
        {
            return _database.GetCollection<NintendoGame>(collectionName);
        }

        public async Task<List<NintendoGame>> AddGame(string collectionName, NintendoGame game)
        {
            var collection = _database.GetCollection<NintendoGame>(collectionName);

            // Ensure the Name exists in the Name collection
            var nameCollection = _database.GetCollection<BsonDocument>("Name");
            var nameExists = await nameCollection.AsQueryable().AnyAsync(n => n["name"] == game.Name);
            if (!nameExists)
            {
                await nameCollection.InsertOneAsync(new BsonDocument { { "name", game.Name } });
            }

            // Ensure the Genre exists in the Genre collection
            var genreCollection = _database.GetCollection<BsonDocument>("Genre");
            var genreExists = await genreCollection.AsQueryable().AnyAsync(g => g["name"] == game.Genre);
            if (!genreExists)
            {
                await genreCollection.InsertOneAsync(new BsonDocument { { "name", game.Genre } });
            }

            // Ensure the ReleaseYear exists in the ReleaseYear collection
            var releaseYearCollection = _database.GetCollection<BsonDocument>("ReleaseYear");
            var releaseYearExists = await releaseYearCollection.AsQueryable().AnyAsync(r => r["year"] == game.ReleaseYear);
            if (!releaseYearExists)
            {
                await releaseYearCollection.InsertOneAsync(new BsonDocument { { "year", game.ReleaseYear } });
            }

            // Ensure the Developer exists in the Developer collection
            var developerCollection = _database.GetCollection<BsonDocument>("Developer");
            var developerExists = await developerCollection.AsQueryable().AnyAsync(d => d["name"] == game.Developer);
            if (!developerExists)
            {
                await developerCollection.InsertOneAsync(new BsonDocument { { "name", game.Developer } });
            }

            // Insert the game into the Games collection (for storing all information)
            await collection.InsertOneAsync(game);

            // Return the updated list of all games
            return await collection.Find(_ => true).ToListAsync();
        }


        // Get all games
        public async Task<List<NintendoGame>> GetAllGames(string collectionName)
        {
            var collection = _database.GetCollection<NintendoGame>(collectionName);
            return await collection.Find(_ => true).ToListAsync();
        }

        // Get game by ObjectId
        public async Task<NintendoGame> GetGameById(string collectionName, ObjectId id)
        {
            var collection = _database.GetCollection<NintendoGame>(collectionName);
            return await collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        // Update a game by ObjectId
        public async Task<NintendoGame> UpdateGame(string collectionName, NintendoGame game)
        {
            var collection = _database.GetCollection<NintendoGame>(collectionName);
            await collection.ReplaceOneAsync(x => x.Id == game.Id, game);
            return game;
        }

        // Delete a game by ObjectId
        public async Task<string> DeleteGame(string collectionName, ObjectId id)
        {
            var collection = _database.GetCollection<NintendoGame>(collectionName);
            await collection.DeleteOneAsync(x => x.Id == id);
            return "Successfully deleted";
        }
    }
}
 