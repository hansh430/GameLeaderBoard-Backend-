namespace GameLeaderBoard.Dtos
{
    public class PlayerResponseDto
    {
        public int Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public int Score { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
