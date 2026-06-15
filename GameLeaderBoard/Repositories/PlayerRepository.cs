using GameLeaderBoard.Data;
using GameLeaderBoard.Dtos;
using GameLeaderBoard.Models;
using GameLeaderBoard.Services;
using Microsoft.EntityFrameworkCore;

namespace GameLeaderBoard.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly GameDbContext _context;
        public PlayerRepository(GameDbContext context)
        {
            _context = context;
        }
        public async Task<Player> AddPlayerAsync(CreatePlayerDto createPlayerDto)
        {
            var player = new Player
            {
                UserName = createPlayerDto.UserName,
                Score = createPlayerDto.Score,
                CreateAt = DateTime.Now
            };
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
            return player;
        }

        public async Task<ApiRespnseService<Player>> UpdatePlayerAsync(int playerId, UpdatePlayerScoreDto updatePlayerDto)
        {
            var player = await _context.Players.FirstOrDefaultAsync(x => x.Id == playerId);
            if (player == null)
            {
                return new ApiRespnseService<Player>
                {
                    Success = false,
                    Message = $"Player with Id {playerId} not found"
                };
            }
            player.Score = updatePlayerDto.Score;
            await _context.SaveChangesAsync();
            return new ApiRespnseService<Player>
            {
                Success = true,
                Message = $"Player with Id {playerId} updated",
                Data = player
            };
        }

        public async Task<ApiRespnseService<int>> DeletePlayerAsync(int playerId)
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
            {
                return new ApiRespnseService<int>
                {
                    Success = false,
                    Message = $"Player with Id {playerId} not found"
                };
            }
            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
            return new ApiRespnseService<int>
            {
                Success = true,
                Message = $"Player with Id {playerId} deleted",
                Data = player.Id
            };
        }

        public async Task<List<Player>> GetAllPlayerAsync()
        {
            return await _context.Players.ToListAsync();
        }

        public async Task<List<Player>> GetLeaderBoardAsync()
        {
            var players = await _context.Players.OrderByDescending(x => x.Score)
                .Take(10)
                .ToListAsync();

            return players;
        }

        public Task<Player> GetPlayerByIdAsync(int playerId)
        {
            throw new NotImplementedException();
        }
    }
}
