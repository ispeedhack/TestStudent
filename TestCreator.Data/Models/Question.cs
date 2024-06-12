using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestCreator.Data.Models
{
    public class Question
    {
        public Question()
        {
                
        }

        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public int TestId { get; set; }
        [Required]
        public string Text { get; set; }
        public string Notes { get; set; }
        [DefaultValue(0)]
        public int Type { get; set; }
        [DefaultValue(0)]
        public int Flags { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public DateTime LastModificationDate { get; set; }

        [ForeignKey("TestId")]
        public virtual Test Test { get; set; }
        public virtual List<Answer> Answers { get; set; }
    }
}
