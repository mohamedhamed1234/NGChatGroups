using System;
using System.Collections.Generic;
using System.Text;

namespace DomainLibrary.Models
{
    public class GroupDataModel
    {
        public string id { get; set; }
        public string Name { get; set; }
        public string Game { get; set; }
        public string Owner { get; set; }
        public int MemberCount { get; set; }
    }
}
