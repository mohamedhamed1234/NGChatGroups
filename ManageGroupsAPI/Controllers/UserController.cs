using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainLibrary;
using ManageGroupsAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ManageGroupsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        DomainLibrary.Interfaces.IRepository _repo;

        public UserController(DomainLibrary.Interfaces.IRepository repo)
        {
            _repo = repo;
        }
   

    [HttpGet]
    [Route("GetUserGroups")]
    public async Task<UserVM> GetUserGroups(string user)
    {
        MainLibrary core = new MainLibrary();
        var userObj = await core.GetUserGroupsAsync(_repo, TestData.GAME, user);

        UserVM userGroups = new UserVM
        {
            UserName = userObj.UserName,
            Game = userObj.Game,
            UserGroups = from ug in userObj.UserGroups select new GroupNameVM { Id = ug.Id, Name = ug.Name },
            OtherGroups = from og in userObj.OtherGroups select new GroupNameVM { Id = og.Id, Name = og.Name },
        };

        return userGroups;
    }

    [HttpPost]
    [Route("JoinGroup")]
    public async Task<MessageResponse> JoinGroup(UserGroupDataModel action)
    {
        MainLibrary core = new MainLibrary();

        var msg = await core.JoinGroup(_repo, action.User, action.Group);

        var response = new MessageResponse { Message = msg };

        return response;
    }

    [HttpPost]
    [Route("LeaveGroup")]
    public async Task<MessageResponse> LeaveGroup(UserGroupDataModel action)
    {
        MainLibrary core = new MainLibrary();

        var msg = await core.LeaveGroup(_repo, action.User, action.Group);

        var response = new MessageResponse { Message = msg };

        return response;
    }

    [HttpPost]
    [Route("UserLogin")]
    public async Task<MessageResponse> UserLogin(LoginRequest user)
    {
        MainLibrary core = new MainLibrary();

        var msg = await core.UserLogin(_repo, user.UserName, TestData.COUNTRY);

        var response = new MessageResponse { Message = msg };

        return response;
    }

    }

}