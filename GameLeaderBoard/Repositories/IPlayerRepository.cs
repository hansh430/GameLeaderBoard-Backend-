using GameLeaderBoard.Dtos;
using GameLeaderBoard.Models;
using GameLeaderBoard.Services;

namespace GameLeaderBoard.Repositories
{
    public interface IPlayerRepository
    {
        Task<List<Player>>GetAllPlayerAsync();
        Task<Player>GetPlayerByIdAsync(int playerId);
        Task<Player>AddPlayerAsync(CreatePlayerDto createPlayerDto);
        Task<ApiRespnseService<Player>> UpdatePlayerAsync(int playerId, UpdatePlayerScoreDto updatePlayerDto);
        Task<ApiRespnseService<int>> DeletePlayerAsync(int playerId);
        Task<List<Player>> GetLeaderBoardAsync();
    }
}
