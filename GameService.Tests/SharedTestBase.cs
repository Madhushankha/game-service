using GameService.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Tests
{
    public class SharedTestBase
    {
        //protected readonly Mock<IConfigurationTests> _mockConfiguration;
        protected GameDbContext _gameDbContext;

        [TestInitialize]
        public void Setup()
        {
            // Create a unique in-memory database for each test
            var dbContextOptions = new DbContextOptionsBuilder<GameDbContext>()
                 .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                 .Options;
            _gameDbContext = new GameDbContext(dbContextOptions);
        }

        #region Public Methods

        public async Task AddGamesAsync()
        {
            foreach (var game in GetGames())
            {
                _gameDbContext.Games.Add(game);
            }
            await _gameDbContext.SaveChangesAsync();
        }

        #endregion Public Methods

        #region Private Methods

        private List<GameModel> GetGames()
        {
            return new List<GameModel>
    {
        new GameModel
        {
            Id           = "game_453aa6e6-a8bd-428a-ac0d-f2331cef0574",
            Name         = "Game2",
            Category     = "Category1",
            ReleasedDate = DateTime.Now,
            Price        = 150m
        },
        new GameModel
        {
            Id           = "game_123aa6e6-a8bd-428a-ac0d-f2331cef0525",
            Name         = "Game3",
            Category     = "Category2",
            ReleasedDate = DateTime.Now.AddDays(-30),
            Price        = 99.99m
        },
        new GameModel
        {
            Id           = "game_083aa6e6-a8bd-428a-ac0d-f2331cef0539",
            Name         = "Game4",
            Category     = "Category3",
            ReleasedDate = DateTime.Now.AddMonths(-1),
            Price        = 120m
        }
    };
        }

        #endregion Private Methods
    }
}