using System;
using System.Collections.Generic;
using System.Text;

namespace DomainLibrary.Models
{
    public class UserDataModel
    {
        public string id { get; set; }
        public string Country { get; set; }
        public string UserName { get; set; }
        public string SessionId { get; set; }
        public List<string> Groups { get; set; }
    }
}
