

using GameService.Model;

namespace GameService.Services
{
    public interface IGameService
    {
        // Create
        Task<GameModel> CreateAsync(GameModel game);

        // Read
        Task<GameModel?> GetByIdDetachedAsync(string id);
        Task<IReadOnlyList<GameModel>> ListAllDetachedAsync();

        // Update
        Task<bool> UpdateAsync(GameModel game);

        // Delete
        Task<bool> DeleteAsync(string id);
    }
}
