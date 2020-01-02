using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainLibrary;
using ManageMessagesAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ManageMessagesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        DomainLibrary.Interfaces.IRepository _repo;

        public MessageController(DomainLibrary.Interfaces.IRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        [Route("GetGroupMessages")]
        public async Task<IEnumerable<MessageVM>> GetGroupMessages(string g)
        {
            MainLibrary core = new MainLibrary();

            var result = await core.GetGroupMessagesAsync(_repo, g);

            IEnumerable<MessageVM> messages = from res in result
                                                     select new MessageVM
                                                     {
                                                         SenderName = res.SenderName,
                                                         GroupName = res.GroupName,
                                                         Content = res.Content,
                                                         MessageDT = res.MessageDT
                                                     };
            return messages;
        }


        [HttpPost]
        [Route("SendToGroup")]
        public async Task<MsgResponse> SendToGroup(MessageVM message)
        {
            MainLibrary core = new MainLibrary();

            DomainLibrary.Models.MessageDataModel m = new DomainLibrary.Models.MessageDataModel
            {
                SenderName = message.SenderName,
                GroupName = message.GroupName,
                Content = message.Content,
                MessageDT = DateTime.Now,
                id = message.GroupName + DateTime.Now.ToString("yyyyMMddhhMIss")
            };

            var msg = await core.SendMessageToGroupAsync(_repo, m);

            var response = new MsgResponse { Message = msg };

            return response;
        }
    }
}