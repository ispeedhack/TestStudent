using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestCreator.Data.Models
{
    public class Token
    {
        public Token()
        {
            
        }

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string ClientId { get; set; }

        public int Type { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Value { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public DateTime LastModificationDate { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
