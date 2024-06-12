using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestCreator.Data.Models
{
    public class Answer
    {
        public Answer()
        {
                
        }

        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public int QuestionId { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public int Value { get; set; }
        [DefaultValue(0)]
        public int Type { get; set; }
        [DefaultValue(0)]
        public int Flags { get; set; }
        public string Notes { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public DateTime LastModificationDate { get; set; }

        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }
    }
}
