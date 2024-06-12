using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace TestCreator.WebApp.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ResultViewModel
    {
        public ResultViewModel()
        {
                
        }

        public int Id { get; set; }
        public int TestId { get; set; }
        public string Text { get; set; }
        public string Notes { get; set; }
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        [DefaultValue(0)]
        public int Type { get; set; }
        [DefaultValue(0)]
        public int Flags { get; set; }
        [JsonIgnore]
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}
