using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using NintendoGamesLABB2.Data;
using NintendoGamesLABB2.Models;

[Route("api/[controller]")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly IMongoCollection<NintendoGame> _gamesCollection;

    public GameController(MongoCRUD mongoCRUD)
    {
        _gamesCollection = mongoCRUD.GetCollection("Games");
    }

    // GET api/game?name=Stray
    [HttpGet]
    public async Task<ActionResult<NintendoGame>> GetGameByName([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest("Game name is required.");
        }

        var game = await _gamesCollection
            .Find(g => g.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            .FirstOrDefaultAsync();

        if (game == null)
        {
            return NotFound($"Game with name '{name}' not found.");
        }

        return Ok(game); // Return the game data if found
    }

    // GET api/game/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<NintendoGame>> GetGameById(string id)
    {
        if (!ObjectId.TryParse(id, out ObjectId objectId))
        {
            return BadRequest("Invalid ID format.");
        }

        var game = await _gamesCollection
            .Find(g => g.Id == objectId)
            .FirstOrDefaultAsync();

        if (game == null)
        {
            return NotFound($"Game with ID '{id}' not found.");
        }

        return Ok(game); // Return the game data if found
    }
}
