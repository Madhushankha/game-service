using GameService.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GameService.Tests
{
    [TestClass]
    public sealed class GameControllerTest : SharedTestBase
    {
        #region Variables and Properties

        private GamesController _gamesController;

        #endregion Variables and Properties

        #region Test Initialization

        [TestInitialize]
        public void TestInitialization()
        {
            var gameService = new GameService.Infrastructure.Services.GameService(_gameDbContext);

            _gamesController = new GamesController(gameService);

            _gameDbContext.Database.EnsureDeleted();
            _gameDbContext.Database.EnsureCreated();
        }

        #endregion Test Initialization

        #region GetById Tests

        [TestMethod]
        [DataRow("game_453aa6e6-a8bd-428a-ac0d-f2331cef0574", "Game2")]
        public async Task GetByIdAsync_ValidId_ReturnsOk(string id, string expectedName)
        {
            await AddGamesAsync();
            // Act
            var action = await _gamesController.GetById(id);

            // Assert
            var ok = action as OkObjectResult;
            Assert.IsNotNull(ok, "Expected OkObjectResult");
            var model = ok.Value as GameModel;
            Assert.AreEqual(id, model.Id);
            Assert.AreEqual(expectedName, model.Name);
        }

        [TestMethod]
        public async Task GetByIdAsync_InvalidId_ReturnsNotFound()
        {
            // Act
            var action = await _gamesController.GetById("nonexistent-id");

            // Assert
            Assert.IsInstanceOfType(action, typeof(NotFoundResult));
        }

        #endregion GetById Tests

        #region ListAll Tests

        [TestMethod]
        public async Task ListAll_ReturnsAllSeededGames()
        {
            // Act
            await AddGamesAsync();
            var action = await _gamesController.ListAll();

            // Assert
            var ok = action as OkObjectResult;
            Assert.IsNotNull(ok);
            var list = (ok.Value as IEnumerable<GameModel>).ToList();
            Assert.AreEqual(3, list.Count);
            CollectionAssert.AreEquivalent(
                new[] {
                "game_453aa6e6-a8bd-428a-ac0d-f2331cef0574",
                "game_123aa6e6-a8bd-428a-ac0d-f2331cef0525",
                "game_083aa6e6-a8bd-428a-ac0d-f2331cef0539"
                },
                list.Select(g => g.Id).ToArray()
            );
        }

        #endregion ListAll Tests

        #region Create Tests

        [TestMethod]
        public async Task Create_ValidGame_ReturnsOkAndPersists()
        {
            // Arrange
            var newGame = new GameModel
            {
                Id = "game_new",
                Name = "NewGame",
                Category = "CategoryX",
                ReleasedDate = DateTime.UtcNow,
                Price = 45m
            };

            // Act
            var action = await _gamesController.Create(newGame);

            // Assert
            var ok = action as OkObjectResult;
            Assert.IsNotNull(ok);
            Assert.AreEqual(newGame.Id, ((GameModel)ok.Value).Id);

            // verify persistence
            var inDb = await _gameDbContext.Games.FindAsync(newGame.Id);
            Assert.IsNotNull(inDb);
            Assert.AreEqual("NewGame", inDb.Name);
        }

        #endregion Create Tests

        #region Update Tests

        [TestMethod]
        public async Task Update_ValidChange_ReturnsNoContentAndUpdates()
        {
            // Arrange
            await AddGamesAsync();
            var toUpdate = await _gameDbContext.Games.FindAsync("game_123aa6e6-a8bd-428a-ac0d-f2331cef0525");
            toUpdate.Name = "UpdatedName";

            // Act
            var action = await _gamesController.Update(toUpdate.Id, toUpdate);

            // Assert
            Assert.IsInstanceOfType(action, typeof(NoContentResult));
            var reloaded = await _gameDbContext.Games.FindAsync(toUpdate.Id);
            Assert.AreEqual("UpdatedName", reloaded.Name);
        }

        [TestMethod]
        public async Task Update_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var game = new GameModel { Id = "game_x", Name = "X" };

            // Act
            var action = await _gamesController.Update("different_id", game);

            // Assert
            Assert.IsInstanceOfType(action, typeof(BadRequestResult));
        }

        #endregion Update Tests

        #region Delete Tests

        [TestMethod]
        public async Task Delete_ExistingGame_ReturnsNoContentAndRemoves()
        {
            // Act
            var action = await _gamesController.Delete("game_083aa6e6-a8bd-428a-ac0d-f2331cef0539");

            // Assert
            Assert.IsInstanceOfType(action, typeof(NotFoundResult));
            var exists = await _gameDbContext.Games.FindAsync("game_083aa6e6-a8bd-428a-ac0d-f2331cef0539");
            Assert.IsNull(exists);
        }

        [TestMethod]
        public async Task Delete_NonExistingGame_ReturnsNotFound()
        {
            // Act
            var action = await _gamesController.Delete("no_such_game");

            // Assert
            Assert.IsInstanceOfType(action, typeof(NotFoundResult));
        }

        #endregion Delete Tests

        [TestCleanup]
        public void TestCleanup()
        {
            _gamesController = null;
            _gameDbContext.Dispose();
        }
    }
}