using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace TRAS.Hubs
{
    public class TestHub : Hub
    {
        public void SendMessage(string message)
        {
            Clients.All.newMessage(Context.User.Identity.Name + ": " + message);
        }
    }
}