using GameLeaderBoard.Data;
using GameLeaderBoard.Dtos;
using GameLeaderBoard.Models;
using GameLeaderBoard.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameLeaderBoard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger<PlayersController> _logger;
        public PlayersController(IPlayerRepository playerRepository, ILogger<PlayersController> logger)
        {
            _playerRepository = playerRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var players = await _playerRepository.GetAllPlayerAsync();
            return Ok(players);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("createPlayer")]
        public async Task<IActionResult> Create(CreatePlayerDto createPlayerDto)
        {
            var player = await _playerRepository.AddPlayerAsync(createPlayerDto);
            _logger.LogInformation("Player created");
            return Ok(player);
        }

        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard()
        {
            var players = await _playerRepository.GetLeaderBoardAsync();
            return Ok(players);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlayer(int id, UpdatePlayerScoreDto updatePlayerDto)
        {
            var response = await _playerRepository.UpdatePlayerAsync(id, updatePlayerDto);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);

        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            var response = await _playerRepository.DeletePlayerAsync(id);
            if(!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}
