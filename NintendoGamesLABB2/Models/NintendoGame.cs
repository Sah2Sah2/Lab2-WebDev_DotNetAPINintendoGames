using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NintendoGamesLABB2.Models
{
    public class NintendoGame
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Genre { get; set; }
        public int ReleaseYear { get; set; }
        public string Developer { get; set; }
    }
}
