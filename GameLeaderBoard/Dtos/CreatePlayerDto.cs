using System.ComponentModel.DataAnnotations;

namespace GameLeaderBoard.Dtos
{
    public class CreatePlayerDto
    {
        [Required]
        public string UserName { get; set; } = "";
        public int Score { get; set; }
    }
}
