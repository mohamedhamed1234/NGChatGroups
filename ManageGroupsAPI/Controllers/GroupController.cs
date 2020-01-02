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
    public class GroupController : ControllerBase
    {

        DomainLibrary.Interfaces.IRepository _repo;

        public GroupController(DomainLibrary.Interfaces.IRepository repo)
        {
            _repo = repo;
        }


        [HttpGet]
        [Route("GetAllGroups")]
        public async Task<IEnumerable<GroupVM>> GetAllGroups(string game)
        {
            MainLibrary core = new MainLibrary();

            var result = await core.GetGameGroupsAsync(_repo, TestData.GAME);

            IEnumerable<GroupVM> groups = from res in result
                                                 select new GroupVM
                                                 {
                                                     Name = res.Name,
                                                     Owner = res.Owner,
                                                     Game = res.Game
                                                 };
            return groups;
        }

        [HttpPost]
        [Route("CreateGroup")]
        public async Task<MessageResponse> CreateGroup(GroupVM group)
        {
            MainLibrary core = new MainLibrary();

            DomainLibrary.Models.GroupDataModel g = new DomainLibrary.Models.GroupDataModel
            {
                Game = TestData.GAME,
                Name = group.Name,
                Owner = group.Owner
            };

            var msg = await core.CreateGroup(_repo, g);

            var response = new MessageResponse { Message = msg };
            return response;
        }
    }
}