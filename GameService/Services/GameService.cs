// Game.Infrastructure/Services/GameService.cs
using GameService.Model;
using GameService.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameService.Infrastructure.Services
{
    public class GameService : IGameService
    {
        private readonly GameDbContext _db;

        public GameService(GameDbContext db)
        {
            _db = db;
        }

        // CREATE
        public async Task<GameModel> CreateAsync(GameModel game)
        {
            game.Id = "game_" + Guid.NewGuid().ToString();
            _db.Games.Add(game);
            await _db.SaveChangesAsync();
            // detach so returned instance isn't tracked
            _db.Entry(game).State = EntityState.Detached;
            return game;
        }

        // READ (single)
        public async Task<GameModel?> GetByIdDetachedAsync(string id)
        {
            var game = await _db.Games.FindAsync(id);
            if (game != null)
                _db.Entry(game).State = EntityState.Detached;
            return game;
        }

        // READ (all)
        public async Task<IReadOnlyList<GameModel>> ListAllDetachedAsync()
        {
            var list = await _db.Games.AsNoTracking().ToListAsync();
            return list;
        }

        // UPDATE
        public async Task<bool> UpdateAsync(GameModel game)
        {
            var exists = await _db.Games.AnyAsync(g => g.Id == game.Id);
            if (!exists) return false;

            // attach & mark modified
            _db.Games.Attach(game);
            _db.Entry(game).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            // detach so no longer tracked
            _db.Entry(game).State = EntityState.Detached;
            return true;
        }

        // DELETE
        public async Task<bool> DeleteAsync(string id)
        {
            var game = await _db.Games.FindAsync(id);
            if (game == null) return false;

            _db.Games.Remove(game);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}