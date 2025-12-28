using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StajPortal.Models.Entities
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SenderId { get; set; } = string.Empty;

        [Required]
        public string ReceiverId { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Subject { get; set; }

        [Required]
        [StringLength(5000)]
        public string Content { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public int? RelatedApplicationId { get; set; }

        // Navigation Properties
        [ForeignKey("SenderId")]
        public ApplicationUser Sender { get; set; } = null!;

        [ForeignKey("ReceiverId")]
        public ApplicationUser Receiver { get; set; } = null!;

        [ForeignKey("RelatedApplicationId")]
        public Application? RelatedApplication { get; set; }
    }
}

