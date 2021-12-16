using ASC.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Paper.Hubs
{
    public class ChatHub:Hub<IHubClient>
    {
        public async Task SendMess(MessageInstance message)
        {
            await Clients.All.BroadcastMessage(message);
        }
    }
}
