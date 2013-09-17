using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ldc.SignalR
{
	class Servers
	{
		private static volatile Servers _instance;
		private static object syncRoot = new Object();

		private readonly IDictionary<string, Server> _servers = null;

		public static Servers Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (syncRoot)
					{
						if (_instance == null)
						{
							_instance = new Servers();
						}
					}
				}

				return _instance;
			}
		}

		private Servers()
		{
			_servers = new Dictionary<string, Server>();
		}

		public void Add(Server server)
		{
			lock (syncRoot)
			{
				if (!_servers.ContainsKey(server.Name))
				{
					_servers.Add(server.Name, server);
				}
			}
		}

		public void Remove(string name)
		{
			lock (syncRoot)
			{
				if (_servers.ContainsKey(name))
				{
					_servers.Remove(name);
				}
			}
		}

		public Server Get(string name)
		{
			lock (syncRoot)
			{
				Server server = null;
				_servers.TryGetValue(name, out server);
				return server;
			}
		}
	}
}
