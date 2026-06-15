namespace GameLeaderBoard.Models
{
    public class RevokedToken
    {
        public int Id { get; set; }

        public string JwtId { get; set; }

        public DateTime RevokedAt { get; set; }
    }
}
