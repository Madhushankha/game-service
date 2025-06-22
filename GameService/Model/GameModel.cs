using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameService.Model
{
    public class GameModel
    {
        public string Id { get; set; } = "1";

        public string Name { get; set; } = null!;

        public string Category { get; set; } = null!;

        public DateTime ReleasedDate { get; set; }

        public decimal Price { get; set; }

        public DateTime CreatedAt { get; private set; }
    }
}