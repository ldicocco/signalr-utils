using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace  Ldc.SignalR
{
	public class HubExt : Hub
	{
		public void _registerServer(string name, string interfaces)
		{
			var server = new Server {Name = name, Interface = interfaces, ConnectionId = Context.ConnectionId };
			Servers.Instance.Add(server);
		}

		public void _unregisterServer(string name)
		{
			Servers.Instance.Remove(name);
		}

		public void _result(Guid id, Object result)
		{
		}
	}
}
