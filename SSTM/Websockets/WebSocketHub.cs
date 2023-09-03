using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace SSTM.Websockets
{
    public class WebSocketHub : Hub
    {
        public void OnConnect()
        {
            Clients.All.SstmClientConnection("ConnectionId", Context.ConnectionId);
            base.OnConnected();
        }       
        public void SendClientToServer(string name, string message)
        {

            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<WebSocketHub>();
            context.Clients.All.SendPushNotifyToClient(name, message);
        }
        public static void SendToSstmClient(string name, string message)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<WebSocketHub>();
            context.Clients.All.SendPushNotifyToClient(name, message);
        }
    }
}