using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ldc.SignalR
{
	// Singleton
	sealed class PendingRequests
	{
		private static volatile PendingRequests _instance;
		private static object syncRoot = new Object();

		private readonly IDictionary<Guid, PendingRequest> _requests = null;

		public static PendingRequests Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (syncRoot)
					{
						if (_instance == null)
						{
							_instance = new PendingRequests();
						}
					}
				}

				return _instance;
			}
		}

		private PendingRequests()
		{
			_requests = new Dictionary<Guid, PendingRequest>();
		}

		public void Add(PendingRequest request)
		{
			lock (syncRoot)
			{
				_requests.Add(request.Id, request);
			}
		}

		public void Remove(Guid id)
		{
			lock (syncRoot)
			{
				_requests.Remove(id);
			}
		}

		public PendingRequest Get(Guid id)
		{
			lock (syncRoot)
			{
				PendingRequest request = null;
				_requests.TryGetValue(id, out request);
				return request;
			}
		}
	}
}
