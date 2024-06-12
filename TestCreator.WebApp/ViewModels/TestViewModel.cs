using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace TestCreator.WebApp.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class TestViewModel
    {
        public TestViewModel()
        {
                
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Text { get; set; }
        public string Notes { get; set; }

        [DefaultValue(0)]
        public int Type { get; set; }
        [DefaultValue(0)]
        public int Flags { get; set; }
        public int ViewCount { get; set; }
        public bool UserCanEdit { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}
