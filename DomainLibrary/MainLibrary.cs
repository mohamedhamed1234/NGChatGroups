using DomainLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLibrary
{
    public class MainLibrary
    {
        public async Task<string> CreateGroup(Interfaces.IRepository repo, Models.GroupDataModel g)
        {
            string msg = "";

            g.id = g.Name.Replace(' ', '-');
            g.Name = g.Name.Replace(' ', '-');
            g.MemberCount = 1;
            msg = await repo.AddGroupAsync(g);

            UserDataModel user = await repo.GetUser(g.Owner);

            if (user == null)
            {
                return "Owner username does not exist";
            }

            user.Groups.Add(g.id);
            await repo.UpdateUserAsync(user);

            return msg;
        }

        public async Task<IEnumerable<Models.GroupDataModel>> GetGameGroupsAsync(Interfaces.IRepository repo, string game)
        {
            return await repo.GetGroupsAsync(game);
        }

        public async Task<UserGroupsDataModel> GetUserGroupsAsync(Interfaces.IRepository repo, string game, string user)
        {
            var userObj = await repo.GetUser(user);

            if (userObj == null)
            {
                UserGroupsDataModel empty = new UserGroupsDataModel
                {
                    UserName = "User does not exist"
                };

                return empty;
            }

            var gameGroups = await repo.GetGroupsAsync(game);

            UserGroupsDataModel userGroups = new UserGroupsDataModel { UserName = user, Game = game };

            userGroups.UserGroups = from g in gameGroups
                                    join ug in userObj.Groups on g.id equals ug
                                    select new GroupNameModel
                                    {
                                        Id = g.id,
                                        Name = g.Name
                                    };

            userGroups.OtherGroups = from g in gameGroups.Where(g => !userObj.Groups.Contains(g.id))
                                     select new GroupNameModel
                                     {
                                         Id = g.id,
                                         Name = g.Name
                                     };

            return userGroups;
        }

        public async Task<string> UserLogin(Interfaces.IRepository repo, string u, string country)
        {
            string msg = "";
            UserDataModel user = await repo.GetUser(u);

            if (user == null)
            {
                user = new UserDataModel
                {
                    id = u.ToLower(),
                    UserName = u,
                    Country = country,
                    Groups = new List<string>()
                };
            }

            msg = await repo.UpdateUserAsync(user);

            return msg;
        }

        public async Task<string> JoinGroup(Interfaces.IRepository repo, string u, string g)
        {
            string msg = "";

            string groupId = g;

            var group = await repo.GetGroupAsync(groupId);

            if (group == null)
            {
                msg = "group does not exist";
                return msg;
            }

            UserDataModel user = await repo.GetUser(u);

            if (user == null)
            {
                msg = "user does not exist";
                return msg;
            }

            if (group.MemberCount >= LibrarySettings.MAX_MEMBERS)
            {
                msg = group.Name + " reached maximum number of members";
            }
            else
            {
                // join group
                user.Groups.Add(groupId);

                await repo.UpdateUserAsync(user);

                group.MemberCount++;

                msg = u + " joined group " + group.Name;

                await repo.UpdateGroupAsync(group);
            }



            return msg;
        }

        public async Task<string> LeaveGroup(Interfaces.IRepository repo, string u, string g)
        {
            string msg = "";

            string groupId = g;

            var group = await repo.GetGroupAsync(groupId);

            if (group == null)
            {
                msg = "group does not exist";
                return msg;
            }

            UserDataModel user = await repo.GetUser(u);

            if (user == null)
            {
                msg = "user does not exist";
                return msg;
            }

            if (user.Groups.Contains(groupId))
            {
                // leave group
                user.Groups.Remove(groupId);

                await repo.UpdateUserAsync(user);

                group.MemberCount--;

                msg = u + " left group " + group.Name;

                await repo.UpdateGroupAsync(group);
            }




            return msg;
        }

        #region Messages
        public async Task<IEnumerable<Models.MessageDataModel>> GetGroupMessagesAsync(Interfaces.IRepository repo, string group)
        {
            return await repo.GetMessagesAsync(group);
        }

        public async Task<string> SendMessageToGroupAsync(Interfaces.IRepository repo, Models.MessageDataModel message)
        {
            return await repo.AddMessageAsync(message);
        }
        #endregion
    }
}
