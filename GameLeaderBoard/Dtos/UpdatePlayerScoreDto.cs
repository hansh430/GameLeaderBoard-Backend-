using System.ComponentModel.DataAnnotations;

namespace GameLeaderBoard.Dtos
{
    public class UpdatePlayerScoreDto
    {
        [Required]
        public int Id { get; set; }
        [Range(0, int.MaxValue)]
        public int Score {  get; set; }
    }
}
