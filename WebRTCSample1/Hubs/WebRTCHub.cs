using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Owin;

namespace WebRTCSample1.Hubs
{
	public class WebRTCHub : Hub
	{
		public void Send(string message)
		{
			Clients.Others.newMessage(message);
		}
	}

	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			// Any connection or hub wire up and configuration should go here
			app.MapSignalR();
		}
	}
}