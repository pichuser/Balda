using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Balda.Hubs
{
    public class TestHub : Hub
    {
        public Task Send(dynamic message)
        {
            return Clients.All.SendMessage("hhh");
        }

        public void Register(long userId)
        {
            Groups.Add(Context.ConnectionId, userId.ToString(CultureInfo.InvariantCulture));

        }

        public Task ToGroup(dynamic id, string message)
        {
            return Clients.Group(id.ToString()).SendMessage(message);
        }
    }
}
