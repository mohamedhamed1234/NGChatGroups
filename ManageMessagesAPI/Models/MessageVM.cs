using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageMessagesAPI.Models
{
    public class MessageVM
    {
        public string SenderName { get; set; }
        public string GroupName { get; set; }
        public DateTime MessageDT { get; set; }

        public string Content { get; set; }
    }
}
