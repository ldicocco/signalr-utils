using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

using Ldc.SignalR;

namespace SignalRDemo.Hubs
{
	public class DemoHub : HubExt
	{
		public void Hello()
		{
			Clients.All.hello();
		}
	}
}