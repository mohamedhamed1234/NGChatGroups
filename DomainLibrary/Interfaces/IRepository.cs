using DomainLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DomainLibrary.Interfaces
{
    public interface IRepository
    {
        Task<string> AddGroupAsync(GroupDataModel group);
        Task<GroupDataModel> GetGroupAsync(string gid);
        Task<string> UpdateGroupAsync(GroupDataModel group);

        Task<IEnumerable<GroupDataModel>> GetGroupsAsync(string game);

        Task<UserDataModel> GetUser(string user);

        Task<string> UpdateUserAsync(UserDataModel user);

        Task<IEnumerable<MessageDataModel>> GetMessagesAsync(string group);

        Task<string> AddMessageAsync(MessageDataModel message);
    }
}
