using GameLeaderBoard.Models;
using Microsoft.EntityFrameworkCore;

namespace GameLeaderBoard.Data
{
    public class GameDbContext:DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

        public DbSet<Player> Players => Set<Player>();
        public DbSet<User> Users => Set<User>();
        public DbSet<RevokedToken> RevokedTokens => Set<RevokedToken>();
    }
}
