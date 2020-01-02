using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SignalRChat.Models;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {

        
        string loginUrl = "https://nextgamesgroupsapi.azurewebsites.net/api/user/UserLogin?user=";
        string getUserGroupsUrl = "https://nextgamesgroupsapi.azurewebsites.net/api/user/GetUserGroups?user=";
        string joinGroupUrl = "https://nextgamesgroupsapi.azurewebsites.net/api/user/JoinGroup";
        string leavGroupUrl = "https://nextgamesgroupsapi.azurewebsites.net/api/user/LeaveGroup";
        string getGroupMessagesUrl = "https://nextgamesmessagesapi.azurewebsites.net/api/Message/GetGroupMessages?g=";
        string sendMessageToGroupUrl = "https://nextgamesmessagesapi.azurewebsites.net/api/Message/SendToGroup";
        string createGroupUrl = "https://nextgamesgroupsapi.azurewebsites.net/api/group/CreateGroup";
        public async Task SendMessageToGroup(string groupName, string message)
        {
           
            //return Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId}: {message}");

            await Clients.Group(groupName).SendAsync("SendToGroup", $"{Context.GetHttpContext().Request.Query["UserName"]}: {message}", groupName);

            //calling send message to group API

            MessageViewModel messageVM = new MessageViewModel();
            messageVM.Content = message;
            messageVM.SenderName = Context.GetHttpContext().Request.Query["UserName"];
            messageVM.GroupName = groupName;
            messageVM.MessageDT = System.DateTime.Now;

            var messagePayload = JsonConvert.SerializeObject(messageVM);
            // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
            var messagehttpContent = new StringContent(messagePayload, Encoding.UTF8, "application/json");

            var messageGroupUri = new Uri(string.Format(sendMessageToGroupUrl, string.Empty));
            HttpClient messageClient = new HttpClient();
            var groupResponse = await messageClient.PostAsync(messageGroupUri, messagehttpContent);
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            //await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
            await Clients.Group(groupName).SendAsync("SendToGroup", $"{Context.ConnectionId}has joined the group {groupName}.", groupName);

            //calling Add to group API
            GroupViewModel group = new GroupViewModel();
            group.Name = groupName;
            group.Owner = Context.GetHttpContext().Request.Query["UserName"];

            var groupPayload = JsonConvert.SerializeObject(group);
            // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
            var grouphttpContent = new StringContent(groupPayload, Encoding.UTF8, "application/json");

            var createGroupUri = new Uri(string.Format(createGroupUrl, string.Empty));
            HttpClient groupClient = new HttpClient();
            var groupResponse = await groupClient.PostAsync(createGroupUri, grouphttpContent);


        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            //await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
            await Clients.Group(groupName).SendAsync("SendToGroup", $"{Context.GetHttpContext().Request.Query["UserName"]} is now online {groupName}.", groupName);

            //calling Join group API
            UserGroupActionModel joinGroup = new UserGroupActionModel();
            joinGroup.Group = groupName;
            joinGroup.User = Context.GetHttpContext().Request.Query["UserName"];

            var joinGroupPayload = JsonConvert.SerializeObject(joinGroup);
            // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
            var joinGrouphttpContent = new StringContent(joinGroupPayload, Encoding.UTF8, "application/json");

            var joineGroupUri = new Uri(string.Format(joinGroupUrl, string.Empty));
            HttpClient joinGroupClient = new HttpClient();
            var joinGroupResponse = await joinGroupClient.PostAsync(joineGroupUri, joinGrouphttpContent);

            if(joinGroupResponse.IsSuccessStatusCode)
            {

                //calling get group messages API
                getGroupMessagesUrl = getGroupMessagesUrl + groupName;

                var groupMessageUri = new Uri(string.Format(getGroupMessagesUrl, string.Empty));
                HttpClient getGroupMessagesClient = new HttpClient();
                var getGroupMessagesResponse = await getGroupMessagesClient.GetAsync(groupMessageUri);


                var messagesContent = await getGroupMessagesResponse.Content.ReadAsStringAsync();

                IEnumerable<MessageViewModel> groupMessagesVM = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<MessageViewModel>>(messagesContent.ToString()));

                foreach (MessageViewModel msgVM in groupMessagesVM)
                {

                    await Clients.Caller.SendAsync("PopulateMyGroupMessage", msgVM.GroupName, msgVM.SenderName, msgVM.Content);
                }

            }




        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("SendToGroup", $"{Context.ConnectionId} has left the group {groupName}.", groupName);

            //calling remove from group API
            UserGroupActionModel userGroup = new UserGroupActionModel();
            userGroup.Group = groupName;
            userGroup.User = Context.GetHttpContext().Request.Query["UserName"];

            var userGroupPayload = JsonConvert.SerializeObject(userGroup);
            // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
            var userGrouphttpContent = new StringContent(userGroupPayload, Encoding.UTF8, "application/json");

            var leaveGroupUri = new Uri(string.Format(leavGroupUrl, string.Empty));
            HttpClient leaveGroupClient = new HttpClient();
            var leaveGroupResponse = await leaveGroupClient.PostAsync(leaveGroupUri, userGrouphttpContent);


        }

        public Task SendPrivateMessage(string user, string message)
        {
            return Clients.User(user).SendAsync("ReceiveMessage", message);
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                await base.OnConnectedAsync();

                string userName = Context.GetHttpContext().Request.Query["UserName"];

                //calling login/register user api
                UserLogin user = new UserLogin();
                user.UserName = userName;
                var userLoginPayload = JsonConvert.SerializeObject(user);
                // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                var userLoginhttpContent = new StringContent(userLoginPayload, Encoding.UTF8, "application/json");
                
                loginUrl = loginUrl + userName;
                var loginUri = new Uri(string.Format(loginUrl, string.Empty));
                HttpClient loginClient = new HttpClient();
                var loginResponse = await loginClient.PostAsync(loginUri, userLoginhttpContent);


                //calling get user Groups and all groups api

                getUserGroupsUrl = getUserGroupsUrl + userName ;
                var getUserGroupsuri = new Uri(string.Format(getUserGroupsUrl, string.Empty));
                HttpClient getUserGroupsClient = new HttpClient();
                var getUserGroupsResponse = await getUserGroupsClient.GetAsync(getUserGroupsuri);

                UserViewModel userGroupsVM = new UserViewModel();

                if (getUserGroupsResponse.IsSuccessStatusCode)
                {
                    var content = await getUserGroupsResponse.Content.ReadAsStringAsync();

                    
                    userGroupsVM = await Task.Run(() => JsonConvert.DeserializeObject<UserViewModel>(content.ToString()));
                    string groupNames = "";
                    IEnumerable<GroupNameViewModel> OtherGroups = userGroupsVM.OtherGroups;
                    foreach(GroupNameViewModel f in OtherGroups)
                    {
                        groupNames = groupNames + f.Name + ",";
                    }
                    //await Clients.User(Context.ConnectionId).SendAsync("PopulateGroupList", groupNames);
                    await Clients.Caller.SendAsync("PopulateGroupList", groupNames);


                    IEnumerable<GroupNameViewModel> myGroups = userGroupsVM.UserGroups;
                    foreach (GroupNameViewModel f in myGroups)
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, f.Name);
                        await Clients.Caller.SendAsync("PopulateMyGroupList", f.Name);
                    }

                    foreach (GroupNameViewModel f in myGroups)
                    {
                        //calling get group messages API
                        string getGroupMessagesUrl2 = getGroupMessagesUrl + f.Name;

                        var groupMessageUri = new Uri(string.Format(getGroupMessagesUrl2, string.Empty));
                        HttpClient getGroupMessagesClient = new HttpClient();
                        var getGroupMessagesResponse = await getGroupMessagesClient.GetAsync(groupMessageUri);

                        //IEnumerable<MessageViewModel> groupMessagesVM = new IEnumerable<MessageViewModel>();
                        if (getGroupMessagesResponse.IsSuccessStatusCode)
                        {
                            var messagesContent = await getGroupMessagesResponse.Content.ReadAsStringAsync();

                            IEnumerable<MessageViewModel> groupMessagesVM = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<MessageViewModel>>(messagesContent.ToString()));

                            foreach(MessageViewModel msgVM in groupMessagesVM)
                            {
                                
                                 await Clients.Caller.SendAsync("PopulateMyGroupMessage", msgVM.GroupName,msgVM.SenderName,msgVM.Content);
                            }
                        }

                    }
                }


                await Clients.Caller.SendAsync("HideSpinner");

            }
            catch (Exception ex)
            {

            }
            
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
