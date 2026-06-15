using GameLeaderBoard.Models;

namespace GameLeaderBoard.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
