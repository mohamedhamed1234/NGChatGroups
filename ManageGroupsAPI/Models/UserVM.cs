using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageGroupsAPI.Models
{
    public class GroupNameVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class UserVM
    {
        public string UserName { get; set; }

        public string Game { get; set; }

        public IEnumerable<GroupNameVM> UserGroups { get; set; }

        public IEnumerable<GroupNameVM> OtherGroups { get; set; }
    }

   
}
