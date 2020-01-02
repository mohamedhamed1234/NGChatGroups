using System;
using System.Collections.Generic;
using System.Text;

namespace DomainLibrary.Models
{
    public class MessageDataModel
    {
        public string id { get; set; }
        public string SenderName { get; set; }
        public string GroupName { get; set; }
        public DateTime MessageDT { get; set; }
        public string Content { get; set; }
    }
}
