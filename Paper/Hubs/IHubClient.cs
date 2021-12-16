using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Paper.Hubs
{
   public interface IHubClient
    {
        Task BroadcastMessage(MessageInstance msg);
    }
}
