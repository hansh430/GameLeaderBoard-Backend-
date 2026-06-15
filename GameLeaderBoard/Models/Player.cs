using System.ComponentModel.DataAnnotations;

namespace GameLeaderBoard.Models
{
    public class Player
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string UserName { get; set; } = "";
        public int Score { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
