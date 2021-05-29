//using Microsoft.AspNet.SignalR;
//using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Customer.Hubs
{
    public class CusHub: Microsoft.AspNetCore.SignalR.Hub
    {
   
        
        public static Microsoft.AspNetCore.SignalR.IHubContext<CusHub> HubContext;

        public CusHub(Microsoft.AspNetCore.SignalR.IHubContext<CusHub> hubContext)
        {
            HubContext = hubContext;
        }
        public static void Show()
        {
            // IHubContext context = GlobalHost.ConnectionManager.GetHubContext<CusHub>();
          //  await HubContext.Clients.All.SendAsync();
            HubContext.Clients.All.SendAsync("displayCustomer");
        }
        //public override async Task OnConnectedAsync()
        //{
        //    await HubContext.Clients.All.SendAsync();
        //}
    }
}
